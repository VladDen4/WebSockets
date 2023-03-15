using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SimpleServer
{
    class SimpleWebServer
    {
        private Socket server;

        public SimpleWebServer(int port)
        {
            server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.IP);
            server.Bind(new IPEndPoint(IPAddress.Loopback, port));
        }
        public void run()
        {
            server.Listen(10);
            Console.WriteLine("Сервер в ожидании запроса...");
            for (; ; )
            {
                Socket socket = server.Accept();
                HandleClientSession(socket);
            }
        }
        private String SocketRead(Socket socket)
        {
            StringBuilder result = new StringBuilder();
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
        private void SocketWrite(Socket socket, String str)
        {
            //Если не требуется русско-язычная кодировка
            System.Text.ASCIIEncoding encoding = new ASCIIEncoding();

            //Если требуется русско-язычная кодировка
            //Encoding encoding = Encoding.GetEncoding(1251);

            socket.Send(encoding.GetBytes(str));
            socket.Send(encoding.GetBytes("\r\n"));
        }
        private void HandleClientSession(Socket socket)
        {
            Console.WriteLine("**New Request**");
            String first = SocketRead(socket);
            Console.WriteLine(first);

            String line;
            do
            {
                line = SocketRead(socket);
                if (line != null)
                    Console.WriteLine(line);
            } while (line != null && line.Length > 0);

            SocketWrite(socket, "HTTP/1.1 200 OK");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "");
            SocketWrite(socket, "<h1>Hello!</h1>");
            //SocketWrite(socket, "<h1>Привет!</h1>");
            SocketWrite(socket, "");
            SocketWrite(socket, "");

            socket.Close();
        }

        static void Main(string[] args)
        {
            SimpleWebServer webServer =
                new SimpleWebServer(80);
            webServer.run();
        }
    }
}