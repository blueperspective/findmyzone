using findmyzone.IO;
using findmyzone.Resources;
using System;

namespace findmyzone.Cli
{
    class ConsoleReporter : IReporter
    {
        public void Info(string text, params string[] args)
        {
            Console.WriteLine(text, args);
        }

        public void Error(string text, params string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLineResetColor(text, args);
        }

        public void StartOp(string text, params string[] args)
        {
            Console.Write(text, args);
            Console.Out.Flush();
        }

        public void OpEndSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLineResetColor(" " + Messages.Ok);
        }

        public void OpEndError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLineResetColor(" " + Messages.Error);
        }

        private void WriteLineResetColor(string text, params string[] args)
        {
            Console.WriteLine(text, args);
            Console.ResetColor();
        }
    }
}
