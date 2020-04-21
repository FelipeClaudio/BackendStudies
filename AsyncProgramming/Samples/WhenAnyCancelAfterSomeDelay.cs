using AsyncProgramming.Definitions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyCancelAfterSomeDelay : WhenAnyBase
    {
        private CancellationTokenSource _cancellationTokenSource;
        public WhenAnyCancelAfterSomeDelay(HttpClient client) : base(client)
        {         
        }

        protected override Task ExecuteAsync()
        {
            //This will set timeout for the whole processing.
            //In this example, task delay was set to 2500 and 2 of 3 endpoints returns data in 1200 or less
            //but in order to finish processing, it take more time than it's necessary to gather data from endpoint.
            //First response comes only after 800ms (avg) after starting request. 
            //This might be due to multiple usage of httpclient by the sample
            //Second and third request are cancelled
            _cancellationTokenSource = new CancellationTokenSource(millisecondsDelay: 2500);
            var clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders, _cancellationTokenSource.Token);
            return ProcessRequest(clientResponseTasks);
        }

        protected override async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            try
            {
                while(clientResponseTasks.Count != 0)
                {
                    Task<HttpResponseMessage> finishedTask = await Task.WhenAny(clientResponseTasks);
                    List<Client> clients = await GetClientListFromRequest(finishedTask);

                    LogResults(finishedTask, clients);
                    clientResponseTasks.Remove(finishedTask);
                }
            }
            catch(Exception ex)
            {
                _cancellationTokenSource.Cancel();
                Console.WriteLine("There was an invalid request and all operations were cancelled");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
