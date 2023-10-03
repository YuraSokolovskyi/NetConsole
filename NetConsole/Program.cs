using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int port = 8080;
            string ip = "127.0.0.1";
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(endPoint);
                socket.Listen();
                Console.WriteLine("Server started. Waiting for connections...");
                Socket client = await socket.AcceptAsync();
                Console.WriteLine("Client connected. Waiting for data...");
                byte[] data = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = await client.ReceiveAsync(data, SocketFlags.None);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (client.Available > 0);
                Console.WriteLine(builder.ToString());

                byte[] time = Encoding.UTF8.GetBytes(DateTime.Now.ToLongTimeString());
                await client.SendAsync(time, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}