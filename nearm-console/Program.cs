using System;

using nearm_library;
using nearm_library.Orion;

namespace nearm_console;

internal static class Program
{
    private static void Main()
    {
        Logger.MessageEvent += Console.WriteLine;
        Logger.Start();
        Logger.Log("Start");

        Orion.LoadServer(".\\SQLSERVER2008");
        Orion.LoadDatabase("A1");

        Console.Read();

        Logger.Log("Stop");
        Logger.Stop();
    }
}
