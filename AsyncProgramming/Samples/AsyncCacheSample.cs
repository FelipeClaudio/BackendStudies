using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class AsyncCacheSample : WhenAnyBase
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncCache<string, List<Client>> _asyncCache;
        private const int NUM_REQUESTS = 3;
        public AsyncCacheSample(HttpClient client) : base(client)
        {
            _httpClient = client;
            _asyncCache = new AsyncCache<string, List<Client>>(GetClientsFromRequest);
        }

        //It' necessary to process 2-3 times without caching
        //And do the same processing with cache to observe the difference
        protected override async Task ExecuteAsync()
        {
            var url = ExternalEndpoints.validClientProviders[0]; //slowest endpoint
            await DoRequestWithoutCache(url).ConfigureAwait(false);
            await DoRequestWithCache(url).ConfigureAwait(false);
        }
        private async Task DoRequestWithoutCache(string url)
        {
            Console.WriteLine("Started requests without cache");
            for(int i = 0; i < NUM_REQUESTS; i++)
            {
                PrintThreadInfoForUrl(true, url);
                await GetClientsFromRequest(url).ConfigureAwait(false);
                PrintThreadInfoForUrl(false, url);
            }
            Console.WriteLine("Finished requests without cache");
            Console.WriteLine(Environment.NewLine);
        }

        private async Task DoRequestWithCache(string url)
        {
            Console.WriteLine("Started requests with cache");
            for(int i = 0; i < NUM_REQUESTS; i++)
            {
                PrintThreadInfoForUrl(true, url);
                await _asyncCache[url].ConfigureAwait(false);
                PrintThreadInfoForUrl(false, url);
            }
            Console.WriteLine("Finished requests with cache");
            Console.WriteLine(Environment.NewLine);
        }

        private async Task<List<Client>> GetClientsFromRequest(string url)
        {
            var result = await _httpClient.GetAsync(url).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<Client>>(await result.Content.ReadAsStringAsync());   
        }

        private void PrintThreadInfoForUrl(bool isStarting, string url)
        {
            if (isStarting)
            {
                Console.WriteLine($"request for url: {url} started at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            }
            else
            {
                Console.WriteLine($"request for url: {url} completed at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            }
                
            Console.WriteLine("Task={0}, Thread={1}",
                               Task.CurrentId,
                               Thread.CurrentThread.ManagedThreadId);
        }
    }
}
