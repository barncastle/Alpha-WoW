using System;

namespace Common.Logging
{
    public class Log
    {
        static public void Message()
        {
            SetLogger(LogType.DEFAULT, "");
        }

        static public void Message(LogType type, string text, params object[] args)
        {
            SetLogger(type, text, args);
        }

        static void SetLogger(LogType type, string text, params object[] args)
        {
            switch (type)
            {
                case LogType.NORMAL:
                    Console.ForegroundColor = ConsoleColor.Green;
                    text = text.Insert(0, "System: ");
                    break;
                case LogType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    text = text.Insert(0, "Error: ");
                    break;
                case LogType.DUMP:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.INIT:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogType.MISC:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case LogType.CMD:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.DEBUG:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    break;
            }

            if (type.Equals(LogType.INIT) | type.Equals(LogType.DEFAULT))
                Console.WriteLine(text, args);
            else if (type.Equals(LogType.DUMP) || type.Equals(LogType.CMD))
                Console.Write(text, args);
            else
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + text, args);
        }
    }
}
