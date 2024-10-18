using System;

namespace nearm_library;

public class LoggerMessage
{
    public DateTime Time { get; } = DateTime.Now;
    public int Thread { get; } = System.Threading.Thread.CurrentThread.ManagedThreadId;
    public LoggerLevel Level { get; set; } = LoggerLevel.Log;
    public string File { get; set; }
    public string Method { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Message))
            return string.Empty;

        return
            $"{Time:dd-MM-yyyy HH:mm:ss} - " +
            $"{Level.ToString(),-5} - " +
            $"{Thread.ToString(),2} - " +
            $"{File}.{Method}() - " +
            $"{Message}";
    }
}
