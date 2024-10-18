using System;

using nearm_library;

namespace nearm_console;

internal static class Program
{
    private static void Main()
    {
        Logger.MessageEvent += Console.WriteLine;
        Logger.Start();
        Logger.Log("Start");


        Console.Read();

        Logger.Log("Stop");
        Logger.Stop();
    }
}
