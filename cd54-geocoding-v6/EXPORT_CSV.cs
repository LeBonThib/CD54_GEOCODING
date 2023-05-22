using Serilog;
using System.Diagnostics;

class EXPORT_CSV : IExportManager
{
    // conversion de chaque ligne du dictionnaire 'main_record' en une chaîne de caractère pour insertion dans un fichier CSV
    private static string ConvertGeoRowToLine(long id, Geo value)
    {
        string csv_line;
        csv_line = $"{id};{value.addr};{value.coord_ban};{value.accu_ban};{value.coord_arcgis};{value.accu_arcgis};{value.coord_gmap};{value.accu_gmap};{value.coord_osm}";
        return csv_line;
    }

    // création du fichier CSV et insertion de la chaîne de caractère créée à partir du dictionnaire 'main_record'
    public void Export(Dictionary<long, Geo> addr_dict)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        Stopwatch stopWatch = new();
        stopWatch.Start();

        using (StreamWriter writer = new($"../../../../geocoding_export_{timestamp}.csv"))
        {
            writer.WriteLine("id;addr;coord_ban;accu_ban;coord_arcgis;accu_arcgis;coord_gmap;accu_gmap;coord_osm");
            foreach (KeyValuePair<long, Geo> entry in addr_dict)
            {
                writer.WriteLine(ConvertGeoRowToLine(entry.Key, entry.Value));
            }
        }

        Log.Information($"FIN DU PROCESSUS D'EXPORT -- TIME: {(stopWatch.ElapsedMilliseconds) / 1000}s");
    }
}