using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace gRPCDemo
{
    public class GreeterService : gRPCDemo.DemoService.DemoServiceBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task SayMultipleHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            for (int i =0; i< 10; i++)
            {
                Thread.Sleep(2000);
                responseStream.WriteAsync(new HelloReply() { Message = $"{i}. Hello {request.Name}" });
            }
            return Task.CompletedTask;
        }

        public override async Task<HelloReply> SayOneHelloForMultipleNames(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var names = new List<string>();
            while (await requestStream.MoveNext())
            {
                names.Add(requestStream.Current.Name);
            }
            return await Task.FromResult(new HelloReply() { Message = $"Hello all {String.Join(", ", names)}" });
        }

        public override Task SayOneHelloInBothWays(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var names = new List<string>();
            while (requestStream.MoveNext().Result)
            {
                names.Add(requestStream.Current.Name);
            }
            Console.WriteLine($"Received the following names {String.Join(", ", names)}");
            for (int i = 0; i < 10; i++)
            {
                responseStream.WriteAsync(new HelloReply() { Message = $"{i}. Hello all {String.Join(", ", names)}" });
            }
            return Task.CompletedTask;
        }
    }
}
