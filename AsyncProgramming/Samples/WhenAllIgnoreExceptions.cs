using AsyncProgramming.Definitions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAllIgnoreExceptions : WhenAnyBase
    {
        public WhenAllIgnoreExceptions(HttpClient client) : base(client)
        {
        }

        protected override async Task ExecuteAsync()
        {
            List<string> urls = new List<string>();
            urls.AddRange(ExternalEndpoints.invalidClientProvider);
            urls.AddRange(ExternalEndpoints.validClientProviders);
            List<Task<HttpResponseMessage>> clientResponseTasks =
                base.RequestEndpointData(urls);
            foreach(var clientResponseTask in clientResponseTasks.Interleaved())
            {
                try
                {
                    await clientResponseTask;
                    List<Client> clients = await base.GetClientListFromRequests(clientResponseTask);
                    LogResults(clientResponseTask, clients);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Task {Task.CurrentId} at thread{Thread.CurrentThread.ManagedThreadId} thrown the following exception: {ex.Message}");
                }
            }
        }
    }
}
