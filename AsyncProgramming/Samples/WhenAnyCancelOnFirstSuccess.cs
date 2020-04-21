using AsyncProgramming.Definitions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyCancelOnFirstSuccess : WhenAnyBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        public WhenAnyCancelOnFirstSuccess(HttpClient client) : base(client)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override Task ExecuteAsync()
        {
            var clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders, _cancellationTokenSource.Token);
            return ProcessRequest(clientResponseTasks);
        }

        protected override async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            Task<HttpResponseMessage> finishedTask = await Task.WhenAny(clientResponseTasks).ConfigureAwait(false);
            List<Client> clients = await GetClientListFromRequest(finishedTask).ConfigureAwait(false);
            LogResults(finishedTask, clients);
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            clientResponseTasks.Remove(finishedTask);
            Console.WriteLine($"All request were cancelled after first success");

            try
            {
                var cancelledResponse = await clientResponseTasks[0].ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                Console.WriteLine("No other task will execute because first one is finished");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
