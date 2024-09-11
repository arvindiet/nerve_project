using System;
using System.Net.Sockets;
using System.Text;

class TCPClient
{
    static void Main(string[] args)
    {
        try
        {
            string serverIP = "127.0.0.1";
            int port = 13000;

            TcpClient client = new TcpClient(serverIP, port);

            Console.WriteLine("Enter message to send (e.g., SetA-One):");
            string messageToSend = Console.ReadLine();

            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(messageToSend);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[256];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string serverMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {serverMessage}");
            }

            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
        }
    }
}
