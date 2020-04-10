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
        private readonly CancellationTokenSource _cancellationTokenSource;
        public WhenAnyCancelAfterSomeDelay(HttpClient client) : base(client)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override Task ExecuteAsync()
        {
            var clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders, _cancellationTokenSource.Token);
            //Could not apply Timeout to WhenAny request
            //This will set timeout for the whole processing.
            //In this example, task delay was set to 2500 and 2 of 3 endpoints returns data in 1200 or less
            //The third request is cancelled
            SetTimeout();
            return ProcessRequest(clientResponseTasks);
        }

        private void SetTimeout()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2500);
                _cancellationTokenSource.Cancel();
            });
        }

        protected override async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            try
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
            catch(Exception ex)
            {
                _cancellationTokenSource.Cancel();
                Console.WriteLine("There was an invalid request and all operations were cancelled");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
