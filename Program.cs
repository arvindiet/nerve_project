using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TCPServer
{
    static Dictionary<string, Dictionary<string, int>> collectionToMatch = new Dictionary<string, Dictionary<string, int>>()
    {
        {"SetA", new Dictionary<string, int>{{"One", 1}, {"Two", 2}}},
        {"SetB", new Dictionary<string, int>{{"Three", 3}, {"Four", 4}}},
        {"SetC", new Dictionary<string, int>{{"Five", 5}, {"Six", 6}}},
        {"SetD", new Dictionary<string, int>{{"Seven", 7}, {"Eight", 8}}},
        {"SetE", new Dictionary<string, int>{{"Nine", 9}, {"Ten", 10}}}
    };

    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            int port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("Server started and listening...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        finally
        {
            server?.Stop();
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[256];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received: {clientMessage}");

        string[] parts = clientMessage.Split('-');
        if (parts.Length == 2)
        {
            string setName = parts[0];
            string keyName = parts[1];

            if (collectionToMatch.ContainsKey(setName) && collectionToMatch[setName].ContainsKey(keyName))
            {
                int value = collectionToMatch[setName][keyName];
                for (int i = 0; i < value; i++)
                {
                    string timeMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    byte[] msg = Encoding.ASCII.GetBytes(timeMessage);
                    await stream.WriteAsync(msg, 0, msg.Length);
                    Console.WriteLine($"Sent: {timeMessage}");
                    await Task.Delay(1000); 
                }
            }
            else
            {
                string emptyMessage = "EMPTY";
                byte[] msg = Encoding.ASCII.GetBytes(emptyMessage);
                await stream.WriteAsync(msg, 0, msg.Length);
                Console.WriteLine("Sent: EMPTY");
            }
        }
        client.Close();
    }
}
