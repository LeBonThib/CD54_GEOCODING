using Serilog;
using System.Net;
using System.Text.Json.Nodes;

public class API_OSM : IAPIManager
{
    public string Request(string addr) //Open Street Map (https://nominatim.org/release-docs/latest/api/Search/)
    {
        string longitude;
        string latitude;
        string addr_coordinates;
        string return_string;
        WebClient client = new();
        var user_agent_pool = new RandomUserAgent();
        try
        {
            string user_agent = user_agent_pool.user_agent_selector();
            client.Headers.Add("User-Agent", user_agent);
            // requête http à l'api et réponse en json
            string request_body = $"https://nominatim.openstreetmap.org/search?q={addr}&format=json";
            string response_body = client.DownloadString(request_body);
            try
            {
                // parsing de la réponse en json
                dynamic json = JsonNode.Parse(response_body);
                // validation
                if (json.ToString() is not "[]")
                {
                    longitude = json[0]["lon"].ToString();
                    latitude = json[0]["lat"].ToString();
                    addr_coordinates = $"{latitude},{longitude}";

                    Log.Information($"[OSM][OK] -- {addr}");
                    return_string = $"{addr_coordinates}|N/A";
                }

                else
                {
                    Log.Error($"[OSM][NOT OK] -- {addr}");
                    return_string = $"{0}|{0}";
                }
            }

            catch (System.Text.Json.JsonException ex)
            {
                Log.Error($"[OSM][NOT OK] -- {addr} -- ERROR: {ex.Message}");
                return_string = $"{0}|{0}";
            }
        }

        catch (WebException ex)
        {
            Log.Error($"[OSM][NOT OK] -- {addr} -- ERROR: {ex.Message}");
            return_string = $"{0}|{0}";
        }

        return return_string;
    }
}