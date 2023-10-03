using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mod3
{
    class Client
    {
        private TcpClient _tcpClient;
        private int _quotesGet = 0;
        private Task _task;

        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public TcpClient getClient()
        {
            return _tcpClient;
        }

        public void setTask(Task task)
        {
            _task = task;
        }

        public void addQuote()
        {
            _quotesGet += 1;
        }

        public int getQuotes()
        {
            return _quotesGet;
        }
    }
    
    internal class Program
    {
        private static int _quoteLimit = 5;
        private static TcpListener _server;
        private static List<Task> _clients = new List<Task>();
        private static List<string> quotes = new List<string>()
        {
            "Quote one\n",
            "Quote two\n",
            "Quote three\n",
            "Quote four\n",
            "Quote five\n",
        };

        private static Dictionary<string, string> users = new Dictionary<string, string>()
        {
            {"test", "test"}
        };
        private static Random rnd = new Random();
        
        static async Task Main(string[] argv)
        {
            IPAddress ip = IPAddress.Loopback;
            int port = 8080;
            _server = new TcpListener(ip, port);

            try
            {
                _server.Start();
                Console.WriteLine("Server launched!");
                await getClients();
            }
            catch (Exception e)
            {
                
            }
        }

        private static async Task receiveDataFromClient(Client client)
        {
            while (true)
            {
                using (StreamReader reader = new StreamReader(client.getClient().GetStream(), leaveOpen:true))
                {
                    if (await reader.ReadLineAsync() == "GET")
                    {
                        using (StreamWriter writer = new StreamWriter(client.getClient().GetStream(), leaveOpen:true))
                        {
                            await writer.WriteLineAsync(quotes[rnd.Next(0, quotes.Count)]);
                            
                            client.addQuote();
                            if (client.getQuotes() == _quoteLimit)
                            {
                                client.getClient().Close();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static async Task<bool> checkUser(TcpClient client)
        {
            using (StreamReader reader = new StreamReader(client.GetStream(), leaveOpen:true))
            {
                string str = await reader.ReadLineAsync();
                string[] credentials = str.Split(":"); // username:password
                return users.ContainsKey(credentials[0]) && users[credentials[0]] == credentials[1];
            }
        }
        
        private static async Task getClients()
        {
            while (true)
            {
                TcpClient client = await _server.AcceptTcpClientAsync();
                // check username and password
                if (!(await checkUser(client)))
                {
                    client.Close();
                    continue;
                }
                
                Client c = new Client(client);
                Task task = new Task(() => receiveDataFromClient(c));
                task.Start();
                c.setTask(task);
                _clients.Add(task);
            
                Console.WriteLine("Client connected!");
            }
        }
    }
}