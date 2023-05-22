using Serilog;

var geocoding = new Geocoding();
var csv_export = new EXPORT_CSV();

string database_name;
string database_login;
string database_password;

var addr_dict = new Dictionary<long, Geo>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(@"..\..\..\..\geocoding_log_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Console.Clear();
int database_choice = ConsoleHelper.MultipleChoice(true, "0) SHDBO", "1) PLACEHOLDER");

if (database_choice == 0)
{
    var db = new DB_SHDBO();
    database_name = ""; //obfuscated for public sharing
    database_login = ""; //obfuscated for public sharing
    Console.Write($"\nEntrez le mot de passe pour la base '{database_name}': ");
    database_password = Console.ReadLine();

    string postgres_connection_string = db.Open(database_name, database_login, database_password);

    addr_dict = await db.LoadAddrFromDBAsync(postgres_connection_string);

    Console.Clear();
    Log.Information("CONNECTÉ À LA BASE SHDBO");
}

else if (database_choice == 1)
{
    ;
    Environment.Exit(0);
}

else
{
    Environment.Exit(0);
}


Log.Information($"DEBUT DU PROCESSUS DE GEOCODING");
var main_record = geocoding.StartGeocodingProcess(addr_dict);

Log.Information($"DEBUT DU PROCESSUS D'EXPORT");
csv_export.Export(main_record);

Console.WriteLine("Appuyez sur Entrée pour fermer ...");
Console.ReadLine();