using Serilog;
using System.Net;
using System.Text.Json.Nodes;

class API_BAN : IAPIManager
{
    private Dictionary<long, Geo> main_record = null;

    public string Request(string addr) //Base Adresse Nationale (https://adresse.data.gouv.fr/api-doc/adresse)
    {
        string longitude;
        string latitude;
        string addr_coordinates;
        string accuracy;
        string return_string;
        try
        {
            // requête http à l'api et réponse en json
            string request_body = $"https://api-adresse.data.gouv.fr/search/?q={addr}";
            string response_body = new WebClient().DownloadString(request_body);
            try
            {
                // parsing de la réponse en json
                dynamic json = JsonNode.Parse(response_body);
                // validation
                if (json["features"].Count != 0)
                {
                    longitude = json["features"][0]["geometry"]["coordinates"][0].ToString();
                    latitude = json["features"][0]["geometry"]["coordinates"][1].ToString();
                    accuracy = json["features"][0]["properties"]["score"].ToString();
                    addr_coordinates = $"{latitude},{longitude}";

                    Log.Information($"[BAN][OK] -- {addr}");
                    return_string = $"{addr_coordinates}|{accuracy}";
                }

                else
                {
                    Log.Error($"[BAN][NOT OK] -- {addr}");
                    return_string = $"{0}|{0}";
                }
            }

            catch (System.Text.Json.JsonException ex)
            {
                Log.Error($"[BAN][NOT OK] -- {addr} -- ERROR: {ex.Message}");
                return_string = $"{0}|{0}";
            }
        }

        catch (WebException ex)
        {
            Log.Error($"[BAN][NOT OK] -- {addr} -- ERROR: {ex.Message}");
            return_string = $"{0}|{0}";
        }

        return return_string;
    }
}