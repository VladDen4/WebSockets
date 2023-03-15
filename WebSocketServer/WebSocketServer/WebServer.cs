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

            IPEndPoint localEndPoint = new (IPAddress.Loopback, Port);

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

            byte[] sendBuffer = encoding.GetBytes("HTTP/1.0 OK\r\n");
            //_client.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
            _client.Send(encoding.GetBytes("HTTP/1.0 OK\r\n"));
        }

        private static void SendCallback(IAsyncResult _result)
        {
            Socket _client = (Socket)_result.AsyncState;
            //_client.Shutdown(SocketShutdown.Send);
        }
    }
}