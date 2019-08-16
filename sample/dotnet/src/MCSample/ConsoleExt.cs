using System;
using System.Threading.Tasks;

namespace MCSample
{
    public static class ConsoleExt
    {
        public static void DoConsoleAction(Action test, string description)
        {
            Console.Write(description);

            try
            {
                test();

                WriteRight("OK  ", ConsoleColor.Green);
            }
            catch
            {
                WriteRight("FAIL", ConsoleColor.Red);

                throw;
            }
            finally
            {
                Console.WriteLine();
            }
        }

        public static async Task DoConsoleAction(Func<Task> test, string description)
        {
            Console.Write(description);

            try
            {
                await test();

                WriteRight("OK  ", ConsoleColor.Green);
            }
            catch
            {
                WriteRight("FAIL", ConsoleColor.Red);

                throw;
            }
            finally
            {
                Console.WriteLine();
            }
        }

        public static void WriteRight(string text, ConsoleColor foreground = ConsoleColor.White)
        {
            var current = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = foreground;
                Console.CursorLeft = Console.BufferWidth - text.Length;
                Console.Write(text);
            }
            finally
            {
                Console.ForegroundColor = current;
            }
        }
    }
}
