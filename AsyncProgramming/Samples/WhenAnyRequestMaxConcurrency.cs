using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyRequestMaxConcurrency : WhenAnyBase
    {
        private const int MAX_CONCURRENCY = 2;
        private readonly HttpClient _client;
        public WhenAnyRequestMaxConcurrency(HttpClient client) : base(client)
        {
            _client = client;
        }

        protected override async Task ExecuteAsync()
        {
            int numTaskAdded = 0;
            var clientsInformationsRequest = new List<Task<HttpResponseMessage>>();
            int numOfValidEndpoints = ExternalEndpoints.validClientProviders.Count;
            Console.WriteLine(Environment.NewLine);
            while(numTaskAdded < MAX_CONCURRENCY && numTaskAdded < numOfValidEndpoints)
            {
                CreateNewRequestTask(numTaskAdded++, clientsInformationsRequest);
            }

            while(clientsInformationsRequest.Count > 0)
            {
                Task<HttpResponseMessage> clientsInformationTask = await Task.WhenAny(clientsInformationsRequest).ConfigureAwait(false);
                List<Client> clientsInformation = await base.GetClientListFromRequest(clientsInformationTask).ConfigureAwait(false);
                base.LogResults(clientsInformationTask, clientsInformation);
                clientsInformationsRequest.Remove(clientsInformationTask);
                if(numTaskAdded < numOfValidEndpoints)
                {
                    CreateNewRequestTask(numTaskAdded++, clientsInformationsRequest);
                }
            }
        }

        private void CreateNewRequestTask(int numTaskAdded, List<Task<HttpResponseMessage>> requests)
        {
            Uri uri = ExternalEndpoints.validClientProviders[numTaskAdded];
            Console.WriteLine($"Started {this.GetType().Name} to endpoint: {uri} at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
            requests.Add( _client.GetAsync(ExternalEndpoints.validClientProviders[numTaskAdded]));
        }
    }
}
