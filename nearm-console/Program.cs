using System;

using nearm_library;
using nearm_library.Enums;
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
        Orion.LoadDatabase("A1", "sa", "123456", LoadStrategy.Auto);

        Console.Read();

        Logger.Log("Stop");
        Logger.Stop();
    }
}
