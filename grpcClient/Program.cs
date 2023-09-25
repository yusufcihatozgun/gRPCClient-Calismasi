using Grpc.Core;
using Grpc.Net.Client;

namespace grpcClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7119");
            var client = new Message.MessageClient(channel);

            while (true)
            {
                Console.WriteLine("GRPC RPC Türünü Seçin:");
                Console.WriteLine("1) Unary RPC Türü");
                Console.WriteLine("2) Server Streaming RPC Türü");
                Console.WriteLine("3) Client Streaming RPC Türü");
                Console.WriteLine("4) Bi Directional Streaming RPC Türü");
                Console.WriteLine("5) Çıkış");

                Console.Write("Seçiminiz: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        var responseUnary = await client.SendMessageUnaryAsync(new UnaryRequest { RequestMessage = "UnaryRequest" });
                        Console.Clear();
                        Console.WriteLine(responseUnary.ResponseMessage);
                        Console.WriteLine();
                        break;

                    case 2:
                        using (var call = client.SendMessageServerStreaming(new ServerStreamingRequest { RequestMessage = "ServerStreamingRequest" }))
                        {
                            Console.Clear();
                            await foreach (var response in call.ResponseStream.ReadAllAsync())
                            {
                                Console.WriteLine(response.ResponseMessage);
                            }
                            Console.WriteLine();
                        }
                        break;

                    case 3:
                        using (var call = client.SendMessageClientStreaming())
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                await call.RequestStream.WriteAsync(new ClientStreamingRequest { RequestMessage = "ClientStreamingRequest No : " + i });
                            }

                            await call.RequestStream.CompleteAsync();

                            Console.Clear();
                            Console.WriteLine(await call.ResponseAsync);
                            Console.WriteLine();
                        }
                        break;

                    case 4:
                        using (var call = client.SendMessageBiDirectionalStreaming())
                        {
                            for (int i = 0; i < 10 ; i++)
                            {
                                await call.RequestStream.WriteAsync(new BiDirectionalStreamingRequest { RequestMessage = "BiDirectionalStreamingRequest No : " + i });

                            }
                            await call.RequestStream.CompleteAsync();

                            Console.Clear();

                            await foreach (var response in call.ResponseStream.ReadAllAsync())
                            {
                                Console.WriteLine(response.ResponseMessage);
                                await Task.Delay(100);
                            }
                            Console.WriteLine();

                        }
                        break;

                    case 5:
                        return;

                    default:
                        Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }
    }
}
