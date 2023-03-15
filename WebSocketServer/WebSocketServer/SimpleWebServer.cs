using ColoredConsole;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SimpleServer
{
    class SimpleWebServer
    {
        private static Socket Server;
        private static int Port;

        public static void Start(int _port)
        {
            Port = _port;

            Writer.Log("Starting SimpleWebServer server...");

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Server.Bind(new IPEndPoint(IPAddress.Loopback, Port));

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
        private static void SocketWrite(Socket socket, string str)
        {
            ASCIIEncoding encoding = new ();

            if (!SocketConnected(socket))
            {
                socket.Shutdown(SocketShutdown.Both);
                return;
            }

            socket.Send(encoding.GetBytes(str));
            socket.Send(encoding.GetBytes("\r\n"));
        }
        private static void HandleClientSession(Socket socket)
        {
            Writer.Log($"Incoming connection from {socket.RemoteEndPoint}");
            string first = SocketRead(socket);
            Writer.Log(first);

            string line;
            do
            {
                line = SocketRead(socket);
                if (line != null)
                    Writer.Log(line);
            } while (line != null && line.Length > 0);

            SocketWrite(socket, "HTTP/1.1 200 OK");
            //SocketWrite(socket, "content-type: text/plain; charset=utf-8");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "<h1>Hello!</h1>");
            SocketWrite(socket, "");
            SocketWrite(socket, "");

            socket.Close();
        }

        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}