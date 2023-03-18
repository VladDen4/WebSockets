// Copyright (c) Vlad_Den <vladden500@gmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ColoredConsole;
using WebServer;
using SimpleServer;

namespace WebSocketTest
{
    class Program
    {
        private static int DefaultPort = 80;

        private static void Main(string[] args)
        {
            Console.Title = "WebSocketTest";

            Writer.Log("Program started");

            if (int.TryParse(Environment.GetEnvironmentVariable("ServerToStart"), out int _result))
            {
                StartServer(_result);
            }
            else
            {
                SelectServer();
            }
        }

        private static void SelectServer()
        {
            Writer.Log("1) WebSerer");
            Writer.Log("2) SimpleWebSerer");
            Writer.Log("Select a server to run: ", _newline: false);

            _ = int.TryParse(Console.ReadLine(), out int _result);
            StartServer(_result);
        }

        private static void StartServer(int _select)
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
