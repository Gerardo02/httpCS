using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class HttpServer
    {
        private readonly TcpListener _listener;
        private readonly string _filePath;

        public HttpServer(IPAddress adress, int port, string filePath)
        {
            _filePath = filePath;
            _listener = new TcpListener(adress, port);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}");


            while(true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                var handler = new RequestHandler();
                await handler.HandleRequest(stream, _filePath);
            }
        }
    }
}