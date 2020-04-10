using AsyncProgramming.Definitions;
using AsyncProgramming.Samples;
using AsyncProgramming.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        public static async Task Main(string[] args)
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
                new WhenAnyCancelOnFirstSucess(httpClient),
                new WhenAnyCancelOnFirstException(httpClient),
                new WhenAnyCancelAfterSomeDelay(httpClient)
            };

            samples.ForEach(sample => sample.Execute());

            //Cancelation token is allready cancelled inside CancellableProcessing
            //Trying to run a task with cancelled token will throw an exception
            try
            {
                samples[2].Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"Exception thrown by cancellable task: {ex.Message}");
            }

            //It is important to dispose cancellation token source
            cancellationTokenSource.Dispose();

            //Next steps are:
            //WhenAny that cancel request after some delay time - not finished yet
            //WhenAny with maximum concurrent threads
            //Efficent WhenAny
            //WhenAllOrFirstException
            //Async cache
            //Async producer consumer
            //steps taken from: https://docs.microsoft.com/pt-br/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern
            //async streams
            Console.ReadLine();
        }
    }
}
