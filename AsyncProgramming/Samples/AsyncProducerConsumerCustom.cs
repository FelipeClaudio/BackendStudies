using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncProgramming.Samples
{
    internal class AsyncProducerConsumerCustom : WhenAnyBase
    {
        private readonly AsyncProducerConsumer<List<Client>> _asyncProducerConsumer = new AsyncProducerConsumer<List<Client>>();
        private List<Task<HttpResponseMessage>> _clientResponseTasks;

        public AsyncProducerConsumerCustom(HttpClient client) : base(client)
        {
        }

        protected override async Task ExecuteAsync()
        {
            _clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders);
            while (_clientResponseTasks.Count != 0)
            {
                //It is possible to consume before data is produced
                //In this case, a task is consumed
                var consumeTask = Consume();
                var produceTask = Produce();
                await Task.WhenAll(consumeTask, produceTask);
            }
        }

        private async Task Produce()
        {
            Task<HttpResponseMessage> finishedTask = await Task.WhenAny(_clientResponseTasks);
            List<Client> clients = await GetClientListFromRequests(finishedTask);
            Console.WriteLine("Inserted clients in the queue");
            
            LogResults(finishedTask, clients);
            _asyncProducerConsumer.Add(clients);
            _clientResponseTasks.Remove(finishedTask);
        }

        private async Task Consume()
        {
            List<Client> result = await _asyncProducerConsumer.Take();
            Console.WriteLine("Removing clients from queue");
            result.ForEach(r => Console.WriteLine($"Client name: {r.Name} age: {r.Age} at time {DateTime.Now.ToString(DateManipulation.dateFormat)}"));
            Console.WriteLine(Environment.NewLine);
        }
    }

    internal class AsyncProducerConsumer<T>
    {
        private readonly Queue<T> _collection = new Queue<T>();
        private readonly Queue<TaskCompletionSource<T>> _waiting = new Queue<TaskCompletionSource<T>>();

        public void Add(T item)
        {
            //Task completion Source is the producer side of Task
            TaskCompletionSource<T> taskCompletionSource = null;
            lock(_collection)
            {
                if(_waiting.Count > 0)
                {
                    taskCompletionSource = _waiting.Dequeue();
                }
                else
                {
                    _collection.Enqueue(item);
                }
            }
            if(taskCompletionSource != null)
            {
                //Sets producer's task response
                taskCompletionSource.TrySetResult(item);
            }
        }

        public Task<T> Take()
        {
            if(_collection.Count > 0)
            {
                return Task.FromResult(_collection.Dequeue());
            }
            //In this case, a producer is created and inserted into waiting queue
            //When an item is added to the queue, this task will be completed
            else
            {
                var taskCompletionSource = new TaskCompletionSource<T>();
                _waiting.Enqueue(taskCompletionSource);
                return taskCompletionSource.Task;
            }
        }
    }
}
