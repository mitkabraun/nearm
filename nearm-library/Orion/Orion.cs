using System;
using System.Data.SqlClient;
using System.Diagnostics;

using nearm_library.Enums;
using nearm_library.Extensions;
using nearm_library.Helper;
using nearm_library.Orion.Entities;
using nearm_library.Resources;

namespace nearm_library.Orion;

public static partial class Orion
{
    public static OrionServer Server { get; private set; }
    public static OrionDatabase Database { get; private set; }

    private static readonly Stopwatch Stopwatch = new();

    public static void LoadServer(string name)
    {
        Server = null;
        Database = null;
        Stopwatch.Restart();
        try
        {
            Logger.Log($"[ {name} ] Загрузка списка баз данных ...");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя не можеть быть пустым.", nameof(name));

            Server = new OrionServer(name);

            if (!ServiceHelper.IsServiceRunning(Server.ServiceName))
                throw new InvalidOperationException($"Служба Windows: {Server.ServiceName} - не запущена");

            using var connection = new SqlConnection(Server.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(Queries.OrionServer, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
                Server.Databases.Add(reader.GoString(0));

            Logger.Debug(Server.Databases.Count > 0
                ? $"[ {name} ] Найдено баз данных: {Server.Databases.Count}"
                : $"[ {name} ] Баз данных не обнаружено");
            Logger.Log($"[ {name} ] Загрузка списка баз данных завершена за {Stopwatch.ElapsedTime()}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Logger.Error($"[ {name} ] Загрузка списка баз данных прервана");
        }
        finally
        {
            Stopwatch.Stop();
        }
    }

    public static void LoadDatabase(string name, string username = "sa", string password = "123456",
        LoadStrategy strategy = LoadStrategy.Auto)
    {
        Database = null;
        Stopwatch.Restart();
        try
        {
            Logger.Log($"[ {name} ] Загрузка базы данных...");

            if (Server == null)
                throw new InvalidOperationException("Перед загрузкой базы данных загрузите сервер.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя не можеть быть пустым.", nameof(name));

            Database = new OrionDatabase(Server, name, username, password, strategy);

            if (!Server.Databases.Contains(name))
                Logger.Error($"[ {name} ] База данных отсутствует в списке сервера");
            if (!ServiceHelper.IsServiceRunning(Database.Server.ServiceName))
                throw new InvalidOperationException($"Служба Windows: {Database.Server.ServiceName} - не запущена");

            if (Preload())
            {
                LoadData();
                ParseData();
            }

            Logger.Log($"[ {name} ] Загрузка базы данных завершена за {Stopwatch.ElapsedTime()}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Logger.Error($"[ {name} ] Загрузка базы данных прервана");
        }
        finally
        {
            Stopwatch.Stop();
        }
    }
}
