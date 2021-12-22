using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Threading;
using System;
using UnityEngine;

public class Speech2Text
{
    //Speech
    readonly string suscriptionKeySpeech = "13f35886137844358c400fb2fee1be74";
    readonly string region = "westeurope";

    string idioma;
    string idiomaTraduccion = "es";

    TranslationRecognizer translatorSpeech;

    //LUIS
    readonly string appId = "aed2ee40-1989-4ab2-abc0-a9bf2e8fd10d";
    readonly string predictionKey = "6690bf3c121f4a90b081509b41af6b20";
    readonly string predictionEndpoint = "https://tfgluis.cognitiveservices.azure.com/";

    public bool estaEscuchando;

    public string[] resultado;

    #region constructor
    public Speech2Text()
    {
        idioma = StaticVariables.getIdioma();

        //Speech to Text
        AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        //Se utiliza la opción de traducción directamente al castellano
        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(suscriptionKeySpeech, region);
        speechTranslationConfig.SpeechRecognitionLanguage = idioma;
        speechTranslationConfig.AddTargetLanguage(idiomaTraduccion);

        translatorSpeech = new TranslationRecognizer(speechTranslationConfig, audioConfig);
        estaEscuchando = false;
    }
    #endregion

    public void Record()
    {
        estaEscuchando = true;
        Thread trd = new Thread(startRecording);
        trd.IsBackground = true;
        trd.Start();
    }

    void startRecording()
    {
        string[] aux = null;
        try
        {
            var result = translatorSpeech.RecognizeOnceAsync();
            result.Wait();
            
            string resultadoTraduccion = "";

            if (result.Result.Reason == ResultReason.TranslatedSpeech)
            {
                if (result.Result.Translations.Count == 1)
                {
                    result.Result.Translations.TryGetValue(idiomaTraduccion, out string res);

                    resultadoTraduccion = res;
                }

            }

            Debug.Log("Speech: " + resultadoTraduccion);

            if (resultadoTraduccion.Equals(""))
                return;

            resultadoTraduccion = resultadoTraduccion.Replace(".", "");
            resultadoTraduccion = resultadoTraduccion.Replace(",", "");

            if (resultadoTraduccion.Length == 4 && char.IsLetter(resultadoTraduccion[0]) && char.IsDigit(resultadoTraduccion[1]))
            {
                aux = createPosiciones4Length(resultadoTraduccion);
            }
            else
            {
                resultadoTraduccion = MakeRequest(predictionKey, predictionEndpoint, appId, resultadoTraduccion);
                //Mira si es comando o posicion
                aux = checkTipoResultado(resultadoTraduccion);
            }
        }
        catch(Exception)
        {
            aux = new string[] { "error" };
        }

        resultado = aux;
        estaEscuchando = false;
    }

    static string MakeRequest(string predictionKey, string predictionEndpoint, string appId, string ord)
    {

        var client = new HttpClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);

        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", predictionKey);

        queryString["query"] = ord;
        queryString["verbose"] = "true";
        queryString["show-all-intents"] = "true";
        queryString["staging"] = "false";
        queryString["timezoneOffset"] = "0";

        var predictionEndpointUri = string.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", predictionEndpoint, appId, queryString);

        var response = client.GetAsync(predictionEndpointUri);

        response.Wait();

        var strResponseContent = response.Result.Content.ReadAsStringAsync();

        strResponseContent.Wait();

        return strResponseContent.Result;

    }

    string[] checkTipoResultado(string orden)
    {
        string[] aux = null;

        JObject jObject = JObject.Parse(orden);
        JToken jPrediction = jObject["prediction"];

        if (jPrediction["topIntent"].ToString().Equals("Orden"))
            aux = createPosiciones(orden);
        else
        {
            if (jPrediction["topIntent"].ToString().Equals("Comando"))
                aux = catchComando(jPrediction);
            if (jPrediction["topIntent"].ToString().Equals("Promocionar"))
                aux = catchPromocionarPieza(jPrediction);
        }

        /*JToken jIntents = jPrediction["intents"];

        string resultIntent = jPrediction[0].ToString();

        Debug.Log(resultIntent);

        if (float.Parse(resultIntent) <= 0.75f)
            aux = null;*/

        string s = "LUIS:";

        foreach (var st in aux)
        {
            s = s + " " + st;
        }

        Debug.Log(s);

        return aux;
    }

    string[] createPosiciones(string orden)
    {

        string x = "";
        string y = "";

        JObject jObject = JObject.Parse(orden);
        JToken jPrediction = jObject["prediction"];
        
        JToken jEntities = jPrediction["entities"];

        JToken jX = jEntities["x"];

        JArray jXArray = (JArray)jEntities["x"];

        if (jXArray.Count == 1)
            x = jX[0][0].ToString();
        else
            x = jX[0][0].ToString() + " " + jX[1][0].ToString();

        JToken jY = jEntities["y"];

        JArray jYArray = (JArray)jEntities["y"];

        if (jYArray.Count == 1)
            y = jY[0][0].ToString();
        else
            y = jY[0][0].ToString() + " " + jY[1][0].ToString();

        string[] aux = {"Orden", jObject["query"].ToString(), x, y };

        return aux;
    }

    string[] createPosiciones4Length(string orden)
    {

        string[] aux = new string[3];

        if (char.IsLetter(orden[0]) && char.IsLetter(orden[2]) && char.IsDigit(orden[1]) && char.IsDigit(orden[3]))
        {
            aux[0] = orden;
            aux[1] = orden[0] + " " + orden[2];
            aux[2] = orden[1] + " " + orden[3];
        }
        else
            return null;

        return aux;
    }

    string[] catchComando(JToken jPrediction)
    {
        string[] aux = null;

        JToken jEntities = jPrediction["entities"];

        foreach (JProperty j in jEntities)
        {
            if (j.Name.Equals("Abandonar"))
            {
                aux = new string[]{"Comando", "Abandonar"};
            }
            if(j.Name.Equals("CambiarTablero"))
            {
                aux = new string[] { "Comando", "Tablero" };
            }
            if (j.Name.Equals("MostrarTiempo"))
            {
                aux = new string[] { "Comando", "Tiempo" };
            }
            if(j.Name.Equals("Fallados"))
            {
                JToken jFallados = jEntities["Fallados"];
                
                aux = new string[] { "Comando", "Fallados", jFallados[0][0].ToString() };
            }
            if(j.Name.Equals("Tiempo_extra"))
            {
                JToken jTiempo = jEntities["Tiempo_extra"][0];

                JToken jMin = jTiempo["numero"];

                aux = new string[] { "Comando", "Tiempo_extra", jMin[0].ToString() };

            }
        }

        return aux;
    }

    string[] catchPromocionarPieza(JToken jPrediction)
    {

        string[] aux = null;

        JToken jEntities = jPrediction["entities"];

        foreach (JProperty j in jEntities)
        {

            if (j.Name.Equals("PiezasPromocion"))
            {
                JToken jPiezasPromocion = jEntities["PiezasPromocion"];

                aux = new string[] { "PiezasPromocion", jPiezasPromocion[0][0].ToString() };
            }

        }

        return aux;
    }

}

