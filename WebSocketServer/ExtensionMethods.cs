using System.Net.Sockets;

namespace WebSocketServer
{
    public static class ExtensionMethods
    {
        public static bool IsConnected(this Socket _socket)
        {
            bool part1 = _socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (_socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}
