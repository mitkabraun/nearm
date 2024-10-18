using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace nearm_library;

public static class Logger
{
    public static event Action<LoggerMessage> MessageEvent;

    private static readonly BlockingCollection<LoggerMessage> Queue = [];
    private static readonly string Filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

    private static string name;
    private static Task task;
    private static string currentFilename;

    public static void Start(string loggerName = null)
    {
        name = loggerName ?? "nearm";
        task = Task.Run(Loop);
    }

    public static void Stop()
    {
        try
        {
            Log(string.Empty);
            Queue.CompleteAdding();
            task.Wait();
            task.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при остановке: {ex.Message}");
        }
    }

    public static void Log(string message, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        Log(LoggerLevel.Log, message, file, method);
    }

    public static void Debug(string message, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        Log(LoggerLevel.Debug, message, file, method);
    }

    public static void Debug(Exception exception, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        if (exception is AggregateException aggregateException)
            foreach (var ex in aggregateException.InnerExceptions)
                Debug(ex, file, method);
        else
            Debug($"{exception.GetType().Name}: {exception.Message}", file, method);
    }

    public static void Error(string message, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        Log(LoggerLevel.Error, message, file, method);
    }

    public static void Error(Exception exception, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        if (exception is AggregateException aggregateException)
            foreach (var ex in aggregateException.InnerExceptions)
                Error(ex, file, method);
        else
            Error($"{exception.GetType().Name}: {exception.Message}", file, method);
    }

    private static void Log(LoggerLevel level, string message, string file, string method)
    {
        Queue.Add(new LoggerMessage
        {
            Level = level,
            File = file == null ? "Unknown" : Path.GetFileNameWithoutExtension(file),
            Method = method,
            Message = message.Replace(Environment.NewLine, " ")
        });
    }

    private static void Loop()
    {
        try
        {
            if (!Directory.Exists(Filepath))
                Directory.CreateDirectory(Filepath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при создании директории: {ex.Message}");
        }
        try
        {
            currentFilename = GetFilename();
            var streamWriter = new StreamWriter(currentFilename, true);
            foreach (var message in Queue.GetConsumingEnumerable())
            {
                MessageEvent?.Invoke(message);

                if (currentFilename != GetFilename())
                {
                    currentFilename = GetFilename();
                    streamWriter.Close();
                    streamWriter.Dispose();
                    streamWriter = new StreamWriter(currentFilename, true);
                }
                try
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при записи в файл: {ex.Message}");
                }
            }
            streamWriter.Close();
            streamWriter.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при работе с сообщениями: {ex.Message}");
        }
    }

    private static string GetFilename()
    {
        return Path.Combine(Filepath, $"{DateTime.Now:yyyyMMdd}_{name}.log");
    }
}
