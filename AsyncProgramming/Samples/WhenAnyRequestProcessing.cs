using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyRequestProcessing : WhenAnyBase
    {
        public WhenAnyRequestProcessing(HttpClient client) : base(client)
        {
        }

        protected override Task ExecuteAsync()
        {
            var clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders);
            return base.ProcessRequest(clientResponseTasks);
        }
    }
}
