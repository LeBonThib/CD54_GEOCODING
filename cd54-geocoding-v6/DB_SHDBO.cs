using Npgsql;
using Serilog;

class DB_SHDBO : IDatabaseManager
{
    private string postgres_connection_string = "";
    public string Open(string database_name, string database_user, string database_password)
    {
        postgres_connection_string = $"Host="";Username={database_user};Password={database_password};Database={database_name}";  //obfuscated for public sharing
        return postgres_connection_string;
    }

    public Dictionary<long, Geo> addr_dict = new();
    public async Task<Dictionary<long, Geo>> LoadAddrFromDBAsync(string connection_string)
    {
        long id;
        string addr;
        await using var data_source = NpgsqlDataSource.Create(connection_string);
        await using var command = data_source.CreateCommand($"SELECT id, voie, complement, code_postal FROM adresse LIMIT 200 OFFSET 1"); // enlever LIMIT et OFFSET en production
        try
        {
            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                id = reader.GetInt64(reader.GetOrdinal("id"));
                addr = $"{reader.GetString(reader.GetOrdinal("voie"))}{reader.GetString(reader.GetOrdinal("complement"))} {reader.GetString(reader.GetOrdinal("code_postal"))}";
                var geo = new Geo();
                geo.addr = addr;
                string addr_form = addr.Replace(",", "").Replace(" ", "+");
                geo.addr_formatted = addr_form;
                geo.coord_ban = "_";
                geo.accu_ban = "-1";
                geo.coord_arcgis = "_";
                geo.accu_arcgis = "-1";
                geo.coord_gmap = "_";
                geo.coord_osm = "_";
                addr_dict.Add(id, geo);
            }
        }
        catch (PostgresException ex)
        {
            Log.Fatal($"AUTHENTICATION ERROR: {ex.Message}");
            Environment.Exit(1);
        }
        return addr_dict;
    }
}