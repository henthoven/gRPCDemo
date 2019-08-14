using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gRPCClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int Port = 50051;
            const string Host = "127.0.0.1";

            var channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            var client = new gRPCDemo.DemoService.DemoServiceClient(channel);

            // Unair
            await DoUnair(client);

            // Server streaming RPC
            await DoStreaminRPC(client);

            // Client streaming RPC
            await DoClientStreaming(client);

            // Bidirectional streaming RPC
            await DoBidirectional(client);

            Console.ReadKey();
        }

        private static async Task DoBidirectional(gRPCDemo.DemoService.DemoServiceClient client)
        {
            Console.WriteLine("Druk op een toets voor bidirectional operatie SayOneHelloInBothWays");
            Console.ReadKey();
            var biderctionalStreamingCall = client.SayOneHelloInBothWays();

            await biderctionalStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "M" });
            await biderctionalStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "Vesper Lynd" });
            await biderctionalStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "Madeleine Swann" });
            await biderctionalStreamingCall.RequestStream.CompleteAsync();
            while (await biderctionalStreamingCall.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
            {
                Console.WriteLine($"Server response {biderctionalStreamingCall.ResponseStream.Current.Message}");
            }
            Console.WriteLine();
        }

        private static async Task DoClientStreaming(gRPCDemo.DemoService.DemoServiceClient client)
        {
            Console.WriteLine("Druk op een toets voor Client streaming operatie SayOneHelloForMultipleNames");
            Console.ReadKey();
            var clientStreamingCall = client.SayOneHelloForMultipleNames();
            await clientStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "Monneypenny" });
            await clientStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "Q" });
            await clientStreamingCall.RequestStream.WriteAsync(new gRPCDemo.HelloRequest { Name = "James Bond" });
            await clientStreamingCall.RequestStream.CompleteAsync();

            Console.WriteLine($"Response is { clientStreamingCall.ResponseAsync.Result.Message }");
            Console.WriteLine();
        }

        private static async Task DoStreaminRPC(gRPCDemo.DemoService.DemoServiceClient client)
        {
            Console.WriteLine("Druk op een toets voor Server streaming operatie SayMultipleHello");
            Console.ReadKey();
            var serverStreamingCall = client.SayMultipleHello(new gRPCDemo.HelloRequest { Name = "James Bond" });
            while (await serverStreamingCall.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
            {
                Console.WriteLine($"Server response {serverStreamingCall.ResponseStream.Current.Message}");
            }
            Console.WriteLine();
        }

        private static async Task DoUnair(gRPCDemo.DemoService.DemoServiceClient client)
        {
            Console.WriteLine("Druk op een toets voor Unaire operatie SayHello");
            Console.ReadKey();
            var call = await client.SayHelloAsync(new gRPCDemo.HelloRequest { Name = "James Bond" });
            Console.WriteLine($"Response is { call.Message }");
            Console.WriteLine();
        }
    }
}
