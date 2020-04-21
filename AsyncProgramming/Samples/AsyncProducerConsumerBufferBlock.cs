using AsyncProgramming.Definitions;
using AsyncProgramming.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AsyncProgramming.Samples
{
    internal class AsyncProducerConsumerBufferBlock : WhenAnyBase
    {
        private readonly BufferBlock<List<Client>> _asyncProducerConsumer = new BufferBlock<List<Client>>();
        private List<Task<HttpResponseMessage>> _clientResponseTasks;

        public AsyncProducerConsumerBufferBlock(HttpClient client) : base(client)
        {
        }

        protected override async Task ExecuteAsync()
        {
            _clientResponseTasks = base.RequestEndpointData(ExternalEndpoints.validClientProviders);
            while(_clientResponseTasks.Count != 0)
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
            _asyncProducerConsumer.Post(clients);
            _clientResponseTasks.Remove(finishedTask);
        }

        private async Task Consume()
        {
            List<Client> result = await _asyncProducerConsumer.ReceiveAsync();
            Console.WriteLine("Removing clients from queue");
            result.ForEach(r => Console.WriteLine($"Client name: {r.Name} age: {r.Age} at time {DateTime.Now.ToString(DateManipulation.dateFormat)}"));
            Console.WriteLine(Environment.NewLine);
        }
    }
}
