using Serilog;
using System.Net;
using System.Text.Json.Nodes;

public class API_ARCGIS : IAPIManager
{
    public string Request(string addr) //Arcgis (https://developers.arcgis.com/documentation/mapping-apis-and-services/tools/developer-dashboard/)
    {
        string longitude;
        string latitude;
        string addr_coordinates;
        string accuracy;
        string return_string;
        string arcgis_token = ""; // get your own token 
        try
        {
            // requête http à l'api et réponse en json
            string request_body = $"https://geocode-api.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates?address={addr}&f=json&token={arcgis_token}";
            string response_body = new WebClient().DownloadString(request_body);
            try
            {
                // parsing de la réponse en json
                dynamic json = JsonNode.Parse(response_body);
                // validation
                if (json["candidates"].Count != 0)
                {
                    longitude = json["candidates"][0]["location"]["x"].ToString();
                    latitude = json["candidates"][0]["location"]["y"].ToString();
                    accuracy = json["candidates"][0]["score"].ToString();
                    addr_coordinates = $"{latitude},{longitude}";

                    Log.Information($"[ARCGIS][OK] -- {addr}");
                    return_string = $"{addr_coordinates}|{accuracy}";
                }

                else
                {
                    Log.Error($"[ARCGIS][NOT OK] -- {addr}");
                    return_string = $"{0}|{0}";
                }
            }

            catch (System.Text.Json.JsonException ex)
            {
                Log.Error($"[ARCGIS][NOT OK] -- {addr} -- ERROR: {ex.Message}");
                return_string = $"{0}|{0}";
            }
        }

        catch (WebException ex)
        {
            Log.Error($"[ARCGIS][NOT OK] -- {addr} -- ERROR: {ex.Message}");
            return_string = $"{0}|{0}";
        }

        return return_string;
    }
}