using AsyncProgramming.Utils;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming.Definitions
{
    internal class SimpleTask
    {
        public Task ExecuteSimpleTask(int taskId, int delayTimeMs)
        {
            Console.WriteLine($"Started doing simple task {taskId} at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)} with {delayTimeMs}ms delay");
            return Task.Run(() =>
            {
                Thread.Sleep(delayTimeMs);
                Console.WriteLine($"did simple {taskId} at {DateTime.Now.ToString(DateManipulation.dateFormat, CultureInfo.InvariantCulture)}");
                Console.WriteLine("Task={0}, Thread={1}",
                                   Task.CurrentId,
                                   Thread.CurrentThread.ManagedThreadId);
            });
        }
    }
}
