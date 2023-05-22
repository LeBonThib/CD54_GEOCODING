using Serilog;
using System.Net;
using System.Text.Json.Nodes;

public class API_GMAP : IAPIManager
{
    public string Request(string addr) //Google Maps (https://console.cloud.google.com/google/maps-apis/api-list?project=cd54-geoloc)
    {
        string longitude;
        string latitude;
        string location_type;
        string addr_coordinates;
        string return_string;
        //TODO ENLEVER LE TOKEN PERSO AVANT DE LIVRER
        string gmaps_token = ""; // get your own token 
        try
        {
            // requête http à l'api et réponse en json
            string request_body = $"https://maps.googleapis.com/maps/api/geocode/json?address={addr}&key={gmaps_token}";
            string response_body = new WebClient().DownloadString(request_body);
            try
            {
                // parsing de la réponse en json
                dynamic json = JsonNode.Parse(response_body);
                // validation
                if (json["results"].Count != 0)
                {
                    longitude = json["results"][0]["geometry"]["location"]["lng"].ToString();
                    latitude = json["results"][0]["geometry"]["location"]["lat"].ToString();
                    location_type = json["results"][0]["geometry"]["location_type"].ToString();
                    addr_coordinates = $"{latitude},{longitude}";

                    Log.Information($"[GMAP][OK] -- {addr}");
                    return_string = $"{addr_coordinates}|{location_type}";
                }

                else
                {
                    Log.Error($"[GMAP][NOT OK] -- {addr}");
                    return_string = $"{0}|{0}";
                }
            }

            catch (System.Text.Json.JsonException ex)
            {
                Log.Error($"[GMAP][NOT OK] -- {addr} -- ERROR: {ex.Message}");
                return_string = $"{0}|{0}";
            }
        }

        catch (WebException ex)
        {
            Log.Error($"[GMAP][NOT OK] -- {addr} -- ERROR: {ex.Message}");
            return_string = $"{0}|{0}";
        }

        return return_string;
    }
}