using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using System;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    class BasicProcessingOutOfOrder : BaseSample
    {
        private readonly SimpleTask _simpleTask;

        internal BasicProcessingOutOfOrder()
        {
            _simpleTask = new SimpleTask();
        }

        protected override async Task ExecuteAsync()
        {
            Console.WriteLine("Basic async processing out of order");
            var simpleTask1 = _simpleTask.ExecuteSimpleTask(1, TASK_1_DELAY);
            var simpleTask2 = _simpleTask.ExecuteSimpleTask(2, TASK_2_DELAY);

            //It is possible to get stuff2 return before stuff1 finishs its processing
            await simpleTask2;
            Console.WriteLine($"Finished waiting for simple task 2 at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            await simpleTask1;
            Console.WriteLine($"Finished Waiting for simple task 1 at {DateTime.Now.ToString(DateManipulation.dateFormat)}");
            Console.WriteLine(Environment.NewLine);
        }
    }
}
