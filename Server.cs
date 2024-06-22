using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_http_server.src
{
    public class Server
    {
        static async Task Main(string[] args)
        {
            var ipAdress = IPAddress.Any;
            var port = 4221;
            string filePath = "";

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i] == "--directory" && i + 1 < args.Length)
                {
                    if(args[i + 1].Length == 0) break;
                    filePath = args[i + 1];
                    break;
                }
            }

            var server = new HttpServer(ipAdress, port, filePath);
            await server.StartAsync();

        }
    }
}

