using System.Collections;

namespace ColoredConsole
{
    class Writer
    {
        private static readonly Queue msgQueue = new ();
        private static bool isProcessing = false;

        public static void Log(string _text, LogStatus _color = LogStatus.Comment, bool _newline = true, bool _timestamp = true)
        {
            Message _msg = new (_text, _color, _newline, _timestamp);
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

            Console.Write(_msg.timestamp ? $"[{DateTime.Now:T}] " : "");

            Console.ForegroundColor = (ConsoleColor)_msg.color;
            Console.Write(_msg.text);
            Console.Write(_msg.newLine ? "\n" : "");
            Console.ResetColor();

            isProcessing = false;

            Write();
        }

        private class Message
        {
            public readonly string text;
            public readonly LogStatus color;
            public readonly bool newLine;
            public readonly bool timestamp;

            public Message(string _text, LogStatus _color = LogStatus.Default, bool _newLine = true, bool _timestamp = true)
            {
                text = _text;
                color = _color;
                newLine = _newLine;
                timestamp = _timestamp;
            }
        }
    }
}