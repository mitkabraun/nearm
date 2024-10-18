using System.Diagnostics;

namespace nearm_library.Extensions;

internal static class StopwatchExtension
{
    public static string ElapsedTime(this Stopwatch stopwatch)
    {
        var result = "";
        if (stopwatch.Elapsed.Hours > 0)
        {
            result += stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            result += " часов";
        }
        else if (stopwatch.Elapsed.Minutes > 0)
        {
            result += stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
            result += " минут";
        }
        else if (stopwatch.Elapsed.Seconds > 0)
        {
            result += stopwatch.Elapsed.ToString(@"s\.fff");
            result += " секунд";
        }
        else
        {
            result += stopwatch.Elapsed.ToString("fff");
            result += " миллисекунд";
        }
        return result;
    }
}
