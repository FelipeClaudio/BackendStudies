using AsyncProgramming.Definitions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyCancelOnFirstException : WhenAnyBase, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        public WhenAnyCancelOnFirstException(HttpClient client) : base(client)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override Task ExecuteAsync()
        {
            var endpoints = new List<Uri>();
            endpoints.AddRange(ExternalEndpoints.invalidClientProvider);
            endpoints.AddRange(ExternalEndpoints.validClientProviders);
            var clientResponseTasks = base.RequestEndpointData(endpoints, _cancellationTokenSource.Token);
            return ProcessRequest(clientResponseTasks);
        }

        protected override async Task ProcessRequest(List<Task<HttpResponseMessage>> clientResponseTasks)
        {
            try
            {
                await base.ProcessRequest(clientResponseTasks).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                Console.WriteLine("There was an invalid request and all operations were cancelled");
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
