using System.Threading.Tasks;

namespace AsyncProgramming.Definitions
{
    abstract class BaseSample
    {
        //Block async execution, such that samples don't run out of order
        public void Execute()
        {
            this.ExecuteAsync().Wait();
        }

        protected abstract Task ExecuteAsync();
    }
}
