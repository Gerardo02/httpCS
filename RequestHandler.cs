using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class RequestHandler
    {
        public async Task HandleRequest(NetworkStream stream, string filePath)
        {
            var request = await ReadRequestAsync(stream);

            var parsedRequest = ParseRequest(request);

            var response = ProcessRequest(parsedRequest, filePath);

            await WriteResponseAsync(stream, response);

        }

        private async Task<string> ReadRequestAsync(NetworkStream stream)
        {
            var buffer = new byte[2048];
            var requestData = new StringBuilder();
            int bytesRead;
            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                requestData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while(bytesRead == buffer.Length);

            return requestData.ToString();
        }

        private (string method, string path, string parameters) ParseRequest(string request)
        {
            var deStructuredRequest = request.Split("Host: ");

            string method = deStructuredRequest[0].Split(" ")[0];
            string path = deStructuredRequest[0].Split(" ")[1];
            string parameters = "Host: " + deStructuredRequest[1];

            return (method, path, parameters);
        }

        private byte[] ProcessRequest((string method, string path, string parameters) request, string filePath)
        {
            string response;
            var endpoints = request.path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var endpointsQty = endpoints.Length;

            if(endpointsQty == 0)
            {
                return Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\nContent-Length: 6\r\n\r\n200 OK");
            }

            if(endpoints[0] == "echo" && endpointsQty > 1 && request.method == "GET" && endpoints[1].Length > 0)
            {
                Console.WriteLine("entro echo");
                response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {endpoints[1].Length}\r\n\r\n{endpoints[1]}";

                if(endpointsQty > 2)
                {
                    string body = "";

                    for(int i = 1; i < endpointsQty; i++)
                    {
                        body += $"{endpoints[i]}/";
                    }
                    body = body.Substring(0, body.Length - 1);
                    response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {body.Length}\r\n\r\n{body}";
                }
            }else if(endpoints[0] == "user-agent" && endpointsQty == 1)
            {
                Console.WriteLine("agent");
                string[] requestSplitted = request.parameters.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int indexOfHeader = 0;

                for(int i = 0; i < requestSplitted.Length; i++)
                {
                    if(requestSplitted[i].IndexOf("User-Agent: ") == 0)
                    {
                        indexOfHeader = i;
                        break;
                    }
                }
                string headerContent = requestSplitted[indexOfHeader].Split(" ")[1];
                response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {headerContent.Length - 1}\r\n\r\n{headerContent}";
            }else if(endpoints[0] == "files" && endpointsQty == 2 && request.method == "GET")
            {
                string? requestedFilePath;
                if(filePath.Length == 0) return Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\nContent-Length: 17\r\n\r\nPath not provided");

                var fileHandler = new FileHandler(filePath);

                requestedFilePath = fileHandler.lookForFile(endpoints[1]);

                if(requestedFilePath == null) return Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\nContent-Length: 13\r\n\r\n404 Not Found");

                string fileContent = fileHandler.readFile(requestedFilePath);

                response = $"HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\nContent-Length: {fileContent.Length}\r\n\r\n{fileContent}";
            }else
            {
                response = "HTTP/1.1 404 Not Found\r\nContent-Length: 13\r\n\r\n404 NOT FOUND";
            }

            return Encoding.UTF8.GetBytes(response);
        }
        
        private async Task WriteResponseAsync(NetworkStream stream, byte[] response)
        {
            await stream.WriteAsync(response, 0, response.Length);
        }

    }
}