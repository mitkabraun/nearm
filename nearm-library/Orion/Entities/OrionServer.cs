using System.Collections.Generic;

namespace nearm_library.Orion.Entities;

public class OrionServer(string name)
{
    public string Name { get; } = name;
    public List<string> Databases { get; } = [];

    public string ServiceName => Name.IndexOf('\\') >= 0
        ? $"MSSQL${Name.Split('\\')[1]}"
        : $"MSSQL${Name}";

    public string ConnectionString =>
        $"server={Name};" +
        $"database=master;" +
        $"integrated security=true;" +
        $"timeout=5";
}
