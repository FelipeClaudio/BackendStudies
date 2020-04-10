using AsyncProgramming.Definitions;
using System;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class BasicProcessingInOrder : BaseSample
    {
        private readonly SimpleTask _simpleTask;

        internal BasicProcessingInOrder()
        {
            _simpleTask = new SimpleTask();
        }


        protected override async Task ExecuteAsync()
        {
            Console.WriteLine("Basic async processing in order");
            var simpleTask1 = _simpleTask.ExecuteSimpleTask(1, TasksDelay.TASK_1_DELAY);
            var simpleTask2 = _simpleTask.ExecuteSimpleTask(2, TasksDelay.TASK_2_DELAY);

            //stuff 2 still finishes before stuff1 but its result is only available after stuff1 is finished
            await simpleTask1;
            Console.WriteLine($"Finished waiting for simple task 1 at {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
            await simpleTask2;
            Console.WriteLine($"Finished waiting for simple task 2 at {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
            Console.WriteLine(Environment.NewLine);
        }
    }
}
