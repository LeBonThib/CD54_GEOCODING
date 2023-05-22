using Serilog;
using System.Diagnostics;

public struct Geo
{
    public string addr;
    public string addr_formatted;
    public string coord_ban;
    public string accu_ban;
    public string coord_arcgis;
    public string accu_arcgis;
    public string coord_gmap;
    public string accu_gmap;
    public string coord_osm;
}

public class Geocoding
{
    public Dictionary<long, Geo> main_record = new();
    public Dictionary<long, Geo> StartGeocodingProcess(Dictionary<long, Geo> addr_dict)
    {
        var ban = new API_BAN();
        var arcgis = new API_ARCGIS();
        var gmap = new API_GMAP();
        var osm = new API_OSM();

        // on démarre un timer pour avoir une métrique de performance
        Stopwatch stopWatch = new();
        stopWatch.Start();

        foreach (KeyValuePair<long, Geo> entry in addr_dict)
        {
            long id = entry.Key;
            Geo geo = entry.Value;
            string addr = geo.addr_formatted;
            string ban_result = "";
            string arcgis_result = "";
            string gmap_result = "";
            string osm_result = "";


            // chaque requête se lance sur un thread unique et attend la complétion de toutes les requêtes avant de lancer le lot suivant
            Parallel.Invoke(() => ban_result = ban.Request(addr),
                            () => arcgis_result = arcgis.Request(addr),
                            () => gmap_result = gmap.Request(addr),
                            () => osm_result = osm.Request(addr));

            // on remplit le dictionaire 'main_record' avec le résultat des différentes requêtes
            geo.coord_ban = ban_result.Split('|')[0];
            geo.accu_ban = ban_result.Split('|')[1];
            geo.coord_arcgis = arcgis_result.Split("|")[0];
            geo.accu_arcgis = arcgis_result.Split('|')[1];
            geo.coord_gmap = gmap_result.Split("|")[0];
            geo.accu_gmap = gmap_result.Split("|")[1];
            geo.coord_osm = osm_result.Split("|")[0];
            main_record[id] = geo;
        }

        Log.Information($"FIN DU PROCESSUS DE GEOCODING -- TIME: {(stopWatch.ElapsedMilliseconds) / 1000}s");
        return main_record;
    }
}