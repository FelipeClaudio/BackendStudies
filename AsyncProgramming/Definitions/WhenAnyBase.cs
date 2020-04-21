using AsyncProgramming.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Definitions
{
    internal abstract class WhenAnyBase : BaseSample
    {
        private readonly HttpClient _client;
        public WhenAnyBase(HttpClient client)
        {
            _client = client;
        }

        //Start parallel tasks with no cancellation
        protected List<Task<HttpResponseMessage>> RequestEndpointData(List<Uri> uris)
        {
            return uris.Select(uri =>
                    {
                        Console.WriteLine($"Started {this.GetType().Name} to endpoint: {uri} at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
                        return _client.GetAsync(uri);
                    }).ToList();
        }

        protected List<Task<HttpResponseMessage>> RequestEndpointData(List<Uri> uris, CancellationToken cancellationToken)
        {
            return uris.Select(uri =>
            {
                Console.WriteLine($"Started {this.GetType().Name} to endpoint: {uri} at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
                return _client.GetAsync(uri, cancellationToken);
            }).ToList();
        }


        protected virtual async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            while(clientResponseTasks.Count != 0)
            {
                Task<HttpResponseMessage> finishedTask = await Task.WhenAny(clientResponseTasks).ConfigureAwait(false);
                List<Client> clients = await GetClientListFromRequest(finishedTask).ConfigureAwait(false);

                LogResults(finishedTask, clients);
                clientResponseTasks.Remove(finishedTask);
            }
        }

        protected async Task<List<Client>> GetClientListFromRequest(Task<HttpResponseMessage> finishedTask)
        {
            var clientJson = await finishedTask.ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<Client>>(await clientJson.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        protected void LogResults(Task<HttpResponseMessage> finishedTask, List<Client> clients)
        {
            clients.ForEach(client => Console.WriteLine($"client: {client.Name} has age: {client.Age}"));
            Console.WriteLine($"thread with id {finishedTask.Id} finished at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
            Console.WriteLine(Environment.NewLine);
        }

        //Note: When one endpoint is not working, it will throw an exception
    }
}
