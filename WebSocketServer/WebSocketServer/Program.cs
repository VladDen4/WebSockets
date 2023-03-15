using ColoredConsole;
using WebServer;
using SimpleServer;

namespace WebSocketTest
{
    class Program
    {
        private static int DefaultPort = 80;
        private static bool _isRunning = false;

        private static void Main(string[] args)
        {
            Console.Title = "WebSocketTest";

            Writer.Log("Program started");

            _isRunning = true;

            SelectServer();
            //Start(2);

            while (_isRunning) { }
        }

        private static void SelectServer()
        {
            Writer.Log("1) WebSerer");
            Writer.Log("2) SimpleWebSerer");
            Writer.Log("Select a server to run: ", _newline: false);

            _ = int.TryParse(Console.ReadLine(), out int _result);
            Start(_result);
        }

        private static void Start(int _select)
        {
            switch (_select)
            {
                case 1:
                    Console.Title = "WebSocketTest - WebServer";
                    Server.Start(DefaultPort);
                    break;
                case 2:
                    Console.Title = "WebSocketTest - SimpleWebServer";
                    SimpleWebServer.Start(DefaultPort);
                    break;
                default:
                    Writer.Log("Please select a server", LogStatus.Warning);
                    SelectServer();
                    break;
            }
        }
    }
}
