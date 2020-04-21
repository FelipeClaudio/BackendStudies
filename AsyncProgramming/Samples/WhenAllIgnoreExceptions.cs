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
            List<Uri> uris = new List<Uri>();
            uris.AddRange(ExternalEndpoints.invalidClientProvider);
            uris.AddRange(ExternalEndpoints.validClientProviders);
            List<Task<HttpResponseMessage>> clientResponseTasks =
                base.RequestEndpointData(uris);
            foreach(var clientResponseTask in clientResponseTasks.Interleaved())
            {
                try
                {
                    await clientResponseTask.ConfigureAwait(false);
                    List<Client> clients = await base.GetClientListFromRequest(clientResponseTask).ConfigureAwait(false);
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
