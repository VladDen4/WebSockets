// Copyright (c) Vlad_Den <vladden500@gmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ColoredConsole;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Drawing;
using WebSocketServer;

namespace SimpleServer
{
    class SimpleWebServer
    {
        private static Socket Server;
        private static int Port;

        private static readonly string serverName = "SimpleWebServer";
        private static readonly string htmlFilePath = @"..\..\..\..\index.html";
        private static readonly string jpgFilePath = @"..\..\..\..\cat.jpg";

        public static void Start(int _port)
        {
            Port = _port;

            Writer.CreateLogFile(serverName);

            Writer.Log($"Starting {serverName} server...");

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Server.Bind(new IPEndPoint(IPAddress.Any, Port));

            Server.Listen(10);
            Writer.Log($"Server started on {Server.LocalEndPoint}", LogStatus.Success);

            while (true)
            {
                Socket socket = Server.Accept();
                HandleClientSession(socket);
            }
        }

        private static string SocketRead(Socket socket)
        {
            StringBuilder result = new ();
            byte[] buffer = new byte[1];

            while (socket.Receive(buffer) > 0)
            {
                char ch = (char)buffer[0];
                if (ch == '\n')
                    break;
                if (ch != '\r')
                    result.Append(ch);
            }

            return result.ToString();
        }

        private static void SocketWrite(Socket socket, string message, Image img = null)
        {
            ASCIIEncoding encoding = new ();

            if (!socket.IsConnected())
            {
                socket.Shutdown(SocketShutdown.Both);
                return;
            }

            if (img != null)
            {
                socket.Send(ImageToByteArray(img));
                return;
            }

            socket.Send(encoding.GetBytes($"{message}\r\n"));
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        private static void HandleClientSession(Socket socket)
        {
            Writer.Log($"Incoming connection from {socket.RemoteEndPoint}", LogStatus.Info);
            string first = SocketRead(socket);
            Writer.Log(first, LogStatus.Info);

            string line;
            do
            {
                line = SocketRead(socket);
                if (line != null)
                    Writer.Log(line, _toFile: false);
            } while (line != null && line.Length > 0);

            SocketWrite(socket, "HTTP/1.1 200 OK");
            SocketWrite(socket, "content-type: image/jpg");
            SocketWrite(socket, "");
            //SocketWrite(socket, "Load check\r\n");
            SocketWrite(socket, "123", Image.FromFile(jpgFilePath));
            //SocketWrite(socket, File.ReadAllText(htmlFilePath));

            socket.Close();
        }
    }
}