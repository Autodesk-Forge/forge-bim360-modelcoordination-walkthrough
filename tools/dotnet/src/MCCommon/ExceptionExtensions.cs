using System;

namespace MCCommon
{
    public static class ExceptionExtensions
    {
        public static void LogToConsole(this Exception error)
        {
            if (error is AggregateException)
            {
                ((AggregateException)error).Handle(e =>
                {
                    Console.WriteLine(e.Message);

                    return true;
                });
            }
            else
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
