using AsyncProgramming.Definitions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class WhenAnyEfficient : WhenAnyBase
    {
        public WhenAnyEfficient(HttpClient client) : base(client)
        {
        }

        protected override async Task ExecuteAsync()
        {
            List<Task<HttpResponseMessage>> clientResponseTasksTemp = base.RequestEndpointData(ExternalEndpoints.validClientProviders);
            List<Task<HttpResponseMessage>> clientResponseTasks = clientResponseTasksTemp.Interleaved().ToList();
            //uses only one continuation for each task because tasks are itereated "manually"
            //All request are executed asynchronouly, but this part below is locked by awaitable method
            //The first task to complete continues with defined continuation and returns result
            foreach (var clientResponse in clientResponseTasks)
            {
                var clients = await base.GetClientListFromRequests(clientResponse);
                base.LogResults(clientResponse, clients);
            };
        }
    }

    //Example taken from:
    //https://docs.microsoft.com/pt-br/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern
    //When any creates a continuation for each task in list. When o call it N times, it creates N² continuations
    //It can be a problem for large set
    public static class WhenAnyOperations
    {
        public static IEnumerable<Task<T>> Interleaved<T>(this IEnumerable<Task<T>> tasks)
        {
            var inputTasks = tasks.ToList();
            var sources = (from _ in Enumerable.Repeat(0, tasks.Count())
                           select new TaskCompletionSource<T>()).ToList();
            int taskIndex = -1;

            inputTasks.ForEach(inputTask =>
            {
                inputTask.ContinueWith(completed =>
                {
                    var source = sources[Interlocked.Increment(ref taskIndex)];
                    if(completed.IsFaulted)
                    {
                        source.TrySetException(completed.Exception.InnerException);
                    }
                    else if(completed.IsCanceled)
                    {
                        source.TrySetCanceled();
                    }
                    else
                    {
                        source.TrySetResult(completed.Result);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously, //Continuation is created in same thread as antecedent thread
                TaskScheduler.Default);

            });
            return sources.Select(s => s.Task);
        }
    }
}
