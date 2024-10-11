using System.Net;
using System.Net.Sockets;

namespace TCPprotokol_del4.Models;

public class Client
{
    public IPEndPoint ipEndPoint { get; set; }

    public Client()
    {
        StartClient();
    }

    public void StartClient()
    {
        Console.WriteLine("Client is started!");

        var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 12000);
        using var client = new TcpClient();
        client.Connect(ipEndPoint);

        Console.WriteLine($"Is connected: {client.Connected}");

        using var streamReader = new StreamReader(client.GetStream());
        using var streamWriter = new StreamWriter(client.GetStream());
        streamWriter.AutoFlush = true;

        while (true)
        {
            Console.WriteLine(streamReader.ReadLine());

            var inputText = Console.ReadLine();
            streamWriter.WriteLine(inputText);

            var message = streamReader.ReadLine();
            Console.WriteLine(message);

            var numbers = Console.ReadLine();
            streamWriter.WriteLine(numbers);
        }
    }
}
