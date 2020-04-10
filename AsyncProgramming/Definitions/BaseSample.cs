using System.Threading.Tasks;

namespace AsyncProgramming.Definitions
{
    abstract class BaseSample
    {
        protected const int TASK_1_DELAY = 100;
        protected const int TASK_2_DELAY = 300;
        //Block async execution, such that samples don't run out of order
        public void Execute()
        {
            this.ExecuteAsync().Wait();
        }

        protected abstract Task ExecuteAsync();
    }
}
