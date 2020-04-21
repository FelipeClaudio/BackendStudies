using AsyncProgramming.Definitions;
using AsyncProgramming.Samples;
using AsyncProgramming.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace AsyncProgramming
{
    internal class Program
    {

        //Stuff 1 starts to be processed before Stuff 2
        //but stuff 2 is way faster then stuff 1, so it finish first.
        //This shows that threads are running in concurrent (maybe parallel) way
        //Note: both actions use different tasks and threads
        //see sample codes below:
        //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netframework-4.8
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/
        //Note2: ConfigureAwait(false) is preferable whenever there is no need to run task in same context
        //It should be used in every await that meets requiment above
        //Anti-patterns examples: https://markheath.net/post/async-antipatterns
        public static void Main()
        {
            Console.WriteLine($"starting at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            Console.WriteLine(Environment.NewLine);

            var cancellationTokenSource = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            List<BaseSample> samples = new List<BaseSample>
            {
                new BasicProcessingInOrder(),
                new BasicProcessingOutOfOrder(),
                new CancellableProcessing(cancellationTokenSource),
                new WhenAnyRequestProcessing(httpClient),
                new WhenAnyCancelOnFirstSuccess(httpClient),
                new WhenAnyCancelOnFirstException(httpClient),
                new WhenAnyCancelAfterSomeDelay(httpClient),
                new WhenAnyRequestMaxConcurrency(httpClient),
                new WhenAnyEfficient(httpClient),
                new WhenAllOrFirstException(httpClient),
                new WhenAllIgnoreExceptions(httpClient),
                new AsyncCacheSample(httpClient),
                new AsyncProducerConsumerCustom(httpClient),
                new AsyncProducerConsumerBufferBlock(httpClient)
            };
            samples.ForEach(sample => sample.Execute());

            //Cancelation token is allready cancelled inside CancellableProcessing
            //Trying to run a task with cancelled token will throw an exception
            try
            {
                samples[2].Execute();
            }
            catch(Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"Exception thrown by CancellableProcessing: {ex.Message}");
            }

            //It is important to dispose cancellation token source
            cancellationTokenSource.Dispose();

            //Next steps are:
            //steps above taken from: https://docs.microsoft.com/pt-br/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern
            //async streams

            Console.ReadLine();
        }
    }
}
