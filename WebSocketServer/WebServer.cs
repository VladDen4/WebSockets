// Copyright (c) Vlad_Den <vladden500@gmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ColoredConsole;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer
{
    class Server
    {
        private static int Port;
        private static Socket socket;
        private static int receivedDataSize = 8192;
        private static byte[] receiveBuffer;
        private static int bytesTransferred;
        private static Encoding encoding = Encoding.ASCII;

        public static void Start(int _port)
        {
            Port = _port;

            Writer.Log("Starting WebServer server...");

            IPEndPoint localEndPoint = new (IPAddress.Any, Port);

            socket = new (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(localEndPoint);
            socket.Listen(100);

            socket.BeginAccept(receivedDataSize, new AsyncCallback(SocketConnectCallback), socket);

            Writer.Log($"Server started on {socket.LocalEndPoint}", LogStatus.Success);
        }

        private static void SocketConnectCallback(IAsyncResult _result)
        {
            Socket _client = socket.EndAccept(out receiveBuffer, out bytesTransferred, _result);
            _client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(SocketReceiveCallback), _client);
            socket.BeginAccept(receivedDataSize, new AsyncCallback(SocketConnectCallback), socket);

            Writer.Log($"Incoming connection from {_client.RemoteEndPoint}");

            _client.Send(encoding.GetBytes("HTTP/1.1 200 OK\r\n"));
            _client.Send(encoding.GetBytes("content-type: text/plain; charset=utf-8\r\n"));
            _client.Send(encoding.GetBytes("<h1>Hello!</h1>\r\n"));
        }

        private static void SocketReceiveCallback(IAsyncResult _result)
        {
            Socket _client = (Socket)_result.AsyncState;

            string stringTransferred = encoding.GetString(receiveBuffer, 0, bytesTransferred);

            Writer.Log($"{bytesTransferred} | {stringTransferred}");

            //byte[] sendBuffer = encoding.GetBytes("HTTP/1.0 200 OK\r\n");
            //_client.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
            //_client.Send(encoding.GetBytes("HTTP/1.0 200 OK\r\n"));
            //_client.Send(encoding.GetBytes("content-type: text/html; charset=utf-8"));
            //_client.Send(encoding.GetBytes(""));
            //_client.Send(encoding.GetBytes("What's up man"));

            SendMessage(socket, "HTTP/1.0 200 OK");
            SendMessage(socket, "content-type: text/html; charset=utf-8");
            SendMessage(socket, "");
            SendMessage(socket, "What's up man");
        }

        private static void SendCallback(IAsyncResult _result)
        {
            Socket _client = (Socket)_result.AsyncState;
            //_client.Shutdown(SocketShutdown.Send);
        }

        private static void SendMessage(Socket _socket, string _message)
        {
            ASCIIEncoding encoding = new();

            if (!SocketConnected(socket))
            {
                socket.Shutdown(SocketShutdown.Both);
                return;
            }

            socket.Send(encoding.GetBytes($"{_message}\r\n"));
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