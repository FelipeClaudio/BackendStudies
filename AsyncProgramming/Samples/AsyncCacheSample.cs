using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class AsyncCacheSample : WhenAnyBase
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncCache<Uri, List<Client>> _asyncCache;
        private const int NUM_REQUESTS = 3;
        public AsyncCacheSample(HttpClient client) : base(client)
        {
            _httpClient = client;
            _asyncCache = new AsyncCache<Uri, List<Client>>(GetClientsFromRequest);
        }

        //It' necessary to process 2-3 times without caching
        //And do the same processing with cache to observe the difference
        protected override async Task ExecuteAsync()
        {
            Uri uri = ExternalEndpoints.validClientProviders[0]; //slowest endpoint
            await DoRequestWithoutCache(uri).ConfigureAwait(false);
            await DoRequestWithCache(uri).ConfigureAwait(false);
        }
        private async Task DoRequestWithoutCache(Uri uri)
        {
            Console.WriteLine("Started requests without cache");
            for(int i = 0; i < NUM_REQUESTS; i++)
            {
                PrintThreadInfoForUrl(true, uri);
                await GetClientsFromRequest(uri).ConfigureAwait(false);
                PrintThreadInfoForUrl(false, uri);
            }
            Console.WriteLine("Finished requests without cache");
            Console.WriteLine(Environment.NewLine);
        }

        private async Task DoRequestWithCache(Uri uri)
        {
            Console.WriteLine("Started requests with cache");
            for(int i = 0; i < NUM_REQUESTS; i++)
            {
                PrintThreadInfoForUrl(true, uri);
                await _asyncCache[uri].ConfigureAwait(false);
                PrintThreadInfoForUrl(false, uri);
            }
            Console.WriteLine("Finished requests with cache");
            Console.WriteLine(Environment.NewLine);
        }

        private async Task<List<Client>> GetClientsFromRequest(Uri uri)
        {
            var result = await _httpClient.GetAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<Client>>(await result.Content.ReadAsStringAsync().ConfigureAwait(false));   
        }

        private void PrintThreadInfoForUrl(bool isStarting, Uri uri)
        {
            if (isStarting)
            {
                Console.WriteLine($"request for url: {uri} started at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
            }
            else
            {
                Console.WriteLine($"request for url: {uri} completed at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
            }
                
            Console.WriteLine("Task={0}, Thread={1}",
                               Task.CurrentId,
                               Thread.CurrentThread.ManagedThreadId);
        }
    }
}
