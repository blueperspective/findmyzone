using System;

namespace findmyzone
{
    class ConsoleReporter : IReporter
    {
        public void Info(string text, params string[] args)
        {
            Console.WriteLine(text, args);
        }

        public void StartOp(string text, params string[] args)
        {
            Console.Write(text, args);
            Console.Out.Flush();
        }

        public void OpEndSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteEnd(Messages.Ok);
        }

        public void OpEndError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteEnd(Messages.Error);
        }

        private void WriteEnd(string text)
        {
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
