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
        Stopwatch.Restart();
        try
        {
            Server = new OrionServer(name);
            Database = null;
            Logger.Log($"[ {Server.Name} ] Загрузка списка баз данных ...");

            if (!ServiceHelper.IsServiceRunning(Server.ServiceName))
                throw new InvalidOperationException($"Служба Windows: {Server.ServiceName} - не запущена");

            using var connection = new SqlConnection(Server.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(Queries.OrionServer, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
                Server.Databases.Add(reader.GoString(0));

            Logger.Debug(Server.Databases.Count > 0
                ? $"[ {Server.Name} ] Найдено баз данных: {Server.Databases.Count}"
                : $"[ {Server.Name} ] Баз данных не обнаружено");
            Logger.Log($"[ {Server.Name} ] Загрузка списка баз данных завершена за {Stopwatch.ElapsedTime()}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Logger.Error($"[ {Server.Name} ] Загрузка списка баз данных прервана");
        }
        finally
        {
            Stopwatch.Stop();
        }
    }

    public static void LoadDatabase(string name, string username, string password, LoadStrategy strategy)
    {
        Stopwatch.Restart();
        try
        {
            Database = new OrionDatabase(Server, name, username, password, strategy);
            Logger.Log($"[ {Database.Name} ] Загрузка базы данных...");

            if (!Server.Databases.Contains(name))
                Logger.Error($"База данных {name} отсутствует в списке сервера");
            if (!ServiceHelper.IsServiceRunning(Database.Server.ServiceName))
                throw new InvalidOperationException($"Служба Windows: {Database.Server.ServiceName} - не запущена");

            if (Preload())
            {
                LoadData();
                ParseData();
            }

            Logger.Log($"[ {Database.Name} ] Загрузка базы данных завершена за {Stopwatch.ElapsedTime()}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Logger.Error($"[ {Database.Name} ] Загрузка базы данных прервана");
        }
        finally
        {
            Stopwatch.Stop();
        }
    }
}
