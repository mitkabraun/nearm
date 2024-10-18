using System;

using nearm_library.Enums;

namespace nearm_library.Orion.Entities;

public class OrionDatabase(OrionServer server, string name, string username, string password, LoadStrategy strategy)
{
    public OrionServer Server { get; } = server;
    public string Name { get; } = name;
    public string Username { get; } = username;
    public string Password { get; } = password;
    public LoadStrategy LoadStrategy { get; set; } = strategy;

    public string ConnectionStringMaster =>
        $"server={Server.Name};" +
        $"database=master;" +
        $"integrated security=true;" +
        $"timeout=5";
    public string ConnectionString =>
        $"server={Server.Name};" +
        $"database={Name};" +
        $"uid={Username};" +
        $"pwd={Password};" +
        $"multipleactiveresultsets=true;" +
        $"timeout=5";

    public int Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Path { get; set; }
    public string Filename { get; set; }
    public int Filesize { get; set; }
    public int Tables { get; set; }
    public Version Version { get; set; }
}
