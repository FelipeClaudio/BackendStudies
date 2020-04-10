using AsyncProgramming.Definitions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class CancellableProcessing : BaseSample
    {
        private const int NUM_ITERATIONS_BEFORE_CANCELLING = 5;
        private readonly CancellationTokenSource _cancellationTokenSource;
        internal CancellableProcessing(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        protected override Task ExecuteAsync()
        {
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            return Task.Run(() =>
            {
                int i = 1;

                //Cancellation is not automatic. It is necessary to implement mechanism to cancel task manually
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (i == NUM_ITERATIONS_BEFORE_CANCELLING)
                        _cancellationTokenSource.Cancel();

                    System.Console.WriteLine($"Iteration {i} in cancelation thread. It will be cancelled in {NUM_ITERATIONS_BEFORE_CANCELLING}th iteration");
                    i++;
                }
                if(cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Task cancelled");
                    Console.WriteLine(Environment.NewLine);
                }
                //cancellationToken prevents task to execute if cancelationtoken is cancelled
            }, cancellationToken);
        }
    }
}
