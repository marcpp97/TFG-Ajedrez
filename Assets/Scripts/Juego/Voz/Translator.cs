using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Translator
{

    private static readonly string subscriptionKey = "bf37631510de444eb301d6ef3639e86a";
    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
    private static readonly string region = "westeurope";

    public static async Task<string> traducirTextoDelOriginal(string s, string idiomaOriginal, string idiomaTraducir)
    {

        string route = string.Format("/translate?api-version=3.0&from={0}&to={1}",idiomaOriginal, idiomaTraducir);

        object[] body = new object[] { new { Text = s } };
        var requestBody = JsonConvert.SerializeObject(body);

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
            // Build the request.
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", region);

            // Send the request and get response.
            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            // Read response as a string.
            string result = await response.Content.ReadAsStringAsync();
            return convertirJSONaString(result);
        }

    }

    static string convertirJSONaString(string s)
    {
        s = s.Remove(0, 1);
        s = s.Remove(s.Length - 1, 1);

        JObject jObject = JObject.Parse(s);
        JToken jTranslations = jObject["translations"];
        JToken jText = jTranslations[0]["text"];

        return jText.ToString();

    }

}
