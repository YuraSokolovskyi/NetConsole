using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var port = 8080;
            var ip = "127.0.0.1";
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // send request
                await socket.ConnectAsync(endPoint);
                Console.WriteLine("Press any button to get time: ");
                Console.ReadKey();
                Console.Clear();
                // byte[] data = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(Encoding.UTF8.GetBytes("GET"), SocketFlags.None);
                
                // get response
                byte[] data = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = await socket.ReceiveAsync(data, SocketFlags.None);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (socket.Available > 0);
                Console.WriteLine(builder.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }
    }
}