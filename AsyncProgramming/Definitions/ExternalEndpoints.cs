using System.Collections.Generic;

namespace AsyncProgramming.Definitions
{
    //Endpoints mocked using mockoon
    internal static class ExternalEndpoints
    {
        public static readonly List<string> validClientProviders = new List<string>
        {
            "http://localhost:3001/clients",
            "http://localhost:3002/clients",
            "http://localhost:3003/clients"
        };
        public static readonly List<string> invalidClientProvider = new List<string>
        {
            "http://localhost:3004/clients"
        };
    }
}
