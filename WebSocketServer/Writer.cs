using System.Collections;

namespace ColoredConsole
{
    class Writer
    {
        private static readonly Queue msgQueue = new ();
        private static bool isProcessing = false;
        private static string logPath = @".\logs\";

        private class Message
        {
            public readonly string text;
            public readonly LogStatus color;
            public readonly bool newLine;
            public readonly bool timestamp;
            public readonly bool toFile;

            public Message(string _text, LogStatus _color = LogStatus.Default, bool _newLine = true, bool _timestamp = true, bool _toFile = false)
            {
                text = _text;
                color = _color;
                newLine = _newLine;
                timestamp = _timestamp;
                toFile = _toFile;
            }
        }

        public static void CreateLogFile(string _serverName)
        {
            logPath += $@"{_serverName}\";

            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            logPath += $@"{DateTime.Now:yy.MM.dd-HH.mm.ss}.log";

            if (!File.Exists(logPath))
                using (var logFile = File.Create(logPath)) {}
        }

        public static void Log(string _text, LogStatus _color = LogStatus.Comment, bool _newline = true, bool _timestamp = true, bool toFile = true)
        {
            Message _msg = new (_text, _color, _newline, _timestamp, toFile);
            msgQueue.Enqueue(_msg);

            Write();
        }

        private static void Write()
        {
            if (isProcessing) return;

            isProcessing = true;

            if (msgQueue.Count < 1)
            {
                isProcessing = false;
                return;
            }

            Message _msg = (Message)msgQueue.Dequeue();

            string currentTime = DateTime.Now.ToString("T");

            Console.Write(_msg.timestamp ? $"[{currentTime}] " : "");

            Console.ForegroundColor = (ConsoleColor)_msg.color;
            Console.Write(_msg.text);
            Console.Write(_msg.newLine ? "\n" : "");

            Console.ResetColor();

            if (File.Exists(logPath) && _msg.toFile) File.AppendAllText(logPath, $"[{currentTime}] {_msg.text}\n");

            isProcessing = false;

            Write();
        }
    }
}