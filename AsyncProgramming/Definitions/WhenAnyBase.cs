using AsyncProgramming.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        protected List<Task<HttpResponseMessage>> RequestEndpointData(List<string> urls)
        {
            return urls.Select(url =>
                    {
                        Console.WriteLine($"Started {this.GetType().Name} to endpoint: {url} at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
                        return _client.GetAsync(url);
                    }).ToList();
        }

        protected List<Task<HttpResponseMessage>> RequestEndpointData(List<string> urls, CancellationToken cancellationToken)
        {
            return urls.Select(url =>
            {
                Console.WriteLine($"Started {this.GetType().Name} to endpoint: {url} at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
                return _client.GetAsync(url, cancellationToken);
            }).ToList();
        }


        protected virtual async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            //Process task as the 
            while(clientResponseTasks.Count != 0)
            {
                //Returns finished task
                Task<HttpResponseMessage> finishedTask = await Task.WhenAny(clientResponseTasks);
                List<Client> clients = await GetClientListFromRequests(finishedTask);

                LogResults(finishedTask, clients);
                clientResponseTasks.Remove(finishedTask);
            }
        }

        protected async Task<List<Client>> GetClientListFromRequests(Task<HttpResponseMessage> finishedTask)
        {
            var clientJson = await finishedTask;
            return JsonConvert.DeserializeObject<List<Client>>(await clientJson.Content.ReadAsStringAsync());
        }

        protected void LogResults(Task<HttpResponseMessage> finishedTask, List<Client> clients)
        {
            clients.ForEach(client => Console.WriteLine($"client: {client.name} has age: {client.age}"));
            Console.WriteLine($"thread with id {finishedTask.Id} finished at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            Console.WriteLine(Environment.NewLine);
        }

        //Note: When one endpoint is not working, it will throw an exception
    }
}
