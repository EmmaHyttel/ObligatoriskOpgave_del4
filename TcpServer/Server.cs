using System.Net;
using System.Net.Sockets;

namespace TcpServer;

public class Server
{
    private TcpListener _listener;
    private IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 12000);

    private readonly Dictionary<string, Action<StreamWriter, int[]>> _commands;
    public Server()
    {
        _commands = new Dictionary<string, Action<StreamWriter, int[]>>
        {
            { "random", GetRandomNumber },
            { "add", AddTwoNumbers },
            { "subtract", SubstractTwoNumbers }
        };
    }

    public void StartServer()
    {
        _listener = new TcpListener(_ipEndPoint);
        _listener.Start();
        Console.WriteLine("Server is started!");


        while (true)
        {
            try
            {
                using var client = _listener.AcceptTcpClient();

                using var streamReader = new StreamReader(client.GetStream());
                using var streamWriter = new StreamWriter(client.GetStream()) { AutoFlush = true };


                streamWriter.WriteLine("Write a command: Random, add or subtract");
                while (true)
                {
                    var message = streamReader.ReadLine();
                    if (message == null) break;

                    var command = message.ToLower();
                    if (_commands.ContainsKey(command))
                    {
                        streamWriter.WriteLine("Input two numbers... (Example: 1 10)");
                        var numbers = GetIntArrayFromStream(streamReader, streamWriter);
                        if (numbers != null)
                        {
                            _commands[command].Invoke(streamWriter, numbers);
                        }
                    }
                    else
                    {
                        streamWriter.WriteLine("Unknown command. Try again!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }
    }

    private static void SubstractTwoNumbers(StreamWriter streamWriter, int[] numbers)
    {
        var numbersSubtracted = numbers.First() - numbers.Last();
        streamWriter.WriteLine($"The result is: {numbersSubtracted}");
    }

    private static void AddTwoNumbers(StreamWriter streamWriter, int[] numbers)
    {
        var numbersAdded = numbers.Sum();
        streamWriter.WriteLine($"The result is: {numbersAdded}");
    }

    private static void GetRandomNumber(StreamWriter streamWriter, int[] numbers)
    {
        var randomNumber = Random.Shared.Next(numbers.Min(), numbers.Max());
        streamWriter.WriteLine($"Your random number is: {randomNumber}");
    }

    private static int[] GetIntArrayFromStream(StreamReader streamReader, StreamWriter streamWriter)
    {
        try
        {
            var input = streamReader.ReadLine();
            var splitString = input?.Split(" ");
            if (splitString != null && splitString.Length == 2)
            {
                var arrayNumbers = new[] { Convert.ToInt32(splitString[0]), Convert.ToInt32(splitString[1]) };
                return arrayNumbers;
            }
            streamWriter.WriteLine("Invalid input. Please enter two numbers!");
        }
        catch (Exception)
        {
            streamWriter.WriteLine("Error parsing numbers. Please try again!");
        }
        return null;
    }
}
