using AsyncProgramming.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAllOrFirstException : WhenAnyBase, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        //Note: This example doesn't return any result because an exception is always thrown by invalid endpoint
        public WhenAllOrFirstException(HttpClient client) : base(client)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync()
        {
            try
            {
                List<Uri> uris = new List<Uri>();
                uris.AddRange(ExternalEndpoints.invalidClientProvider);
                uris.AddRange(ExternalEndpoints.validClientProviders);
                List<Task<HttpResponseMessage>> clientResponseTasks =
                    base.RequestEndpointData(uris, _cancellationTokenSource.Token);

                await Task.WhenAll(clientResponseTasks).ConfigureAwait(false);

                //It's now possible to get results from all request as all task are completed
                //Note: .Foreach doesn't work as expected because it's returns is void
                //and void doesn't doesn't work well with async.
                //However, foreach will return a Task and allow proper error handling
                foreach (var request in clientResponseTasks)
                {
                    List<Client> clients = await base.GetClientListFromRequest(request).ConfigureAwait(false);
                    LogResults(request, clients);
                }                
            }
            catch(Exception ex)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                Console.WriteLine($"A task in {this.GetType().Name} thrown exception: {ex.Message}");
            }
        }
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
