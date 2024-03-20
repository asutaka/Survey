using System;

namespace Survey.Utils
{
    public static class LogM
    {
        public static void Log(string mes)
        {
            Console.WriteLine($"{DateTime.Now}: {mes}");
        }

        public static void Start()
        {
            Console.WriteLine($"{DateTime.Now}: START");
        }

        public static void Stop()
        {
            Console.WriteLine($"{DateTime.Now}: STOP");
        }
    }
}
