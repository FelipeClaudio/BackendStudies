using System;
using System.Collections.Generic;

namespace AsyncProgramming.Definitions
{
    //Endpoints mocked using mockoon
    internal static class ExternalEndpoints
    {
        public static readonly List<Uri> validClientProviders = new List<Uri>
        {
            new Uri("http://localhost:3001/clients"),
            new Uri("http://localhost:3002/clients"),
            new Uri("http://localhost:3003/clients")
        };
        public static readonly List<Uri> invalidClientProvider = new List<Uri>
        {
            new Uri("http://localhost:3004/clients")
        };
    }
}
