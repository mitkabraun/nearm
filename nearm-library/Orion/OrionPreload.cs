using System;
using System.Data.SqlClient;
using System.IO;

using nearm_library.Enums;
using nearm_library.Extensions;
using nearm_library.Resources;

namespace nearm_library.Orion;

public static partial class Orion
{
    private static bool Preload()
    {
        using (var connection = new SqlConnection(Database.ConnectionStringMaster))
        {
            connection.Open();
            using (var command = new SqlCommand(Queries.OrionDatabase_1, connection))
            {
                command.Parameters.AddWithValue("Name", Database.Name);
                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Database.Id = reader.GoInt32(0);
                    Database.CreateTime = reader.GoDateTime(1);
                }
            }
            using (var command = new SqlCommand(Queries.OrionDatabase_2, connection))
            {
                command.Parameters.AddWithValue("Id", Database.Id.ToString());
                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    var fullname = reader.GoString(0).Replace(@"\\", "\\");
                    Database.Path = Path.GetDirectoryName(fullname);
                    Database.Filename = Path.GetFileName(fullname);
                    Database.Filesize = reader.GoInt32(1);
                }
            }
        }
        using (var connection = new SqlConnection(Database.ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(Queries.OrionDatabase_3, connection))
            {
                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Database.Tables = reader.GoInt32(0);
                }
            }
            using (var command = new SqlCommand(Queries.OrionDatabase_4, connection))
            {
                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    var ver = reader.GoString(0);
                    if (Version.TryParse(ver, out var version))
                    {
                        Database.Version = version;
                        if (Database.LoadStrategy == LoadStrategy.Auto)
                            Database.LoadStrategy = Database.Version >= new Version(1, 20, 3, 8)
                                ? LoadStrategy.New
                                : LoadStrategy.Old;
                    }
                    else
                        throw new InvalidOperationException($"Неудалось получить версию базы данных из этого: {ver}");
                }
            }
        }

        Logger.Debug($"[ {Database.Name} ] Предзагрузка завершена, версия {Database.Version}, таблиц {Database.Tables}");
        return true;
    }
}
