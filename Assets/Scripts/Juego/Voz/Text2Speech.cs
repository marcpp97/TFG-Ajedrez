using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading;

public class Text2Speech
{

    readonly string suscriptionKeySpeech = "13f35886137844358c400fb2fee1be74";
    readonly string region = "westeurope";

    SpeechConfig config;
    SpeechSynthesizer synthesizer;

    public Text2Speech()
    {

        config = SpeechConfig.FromSubscription(suscriptionKeySpeech, region);
        config.SpeechRecognitionLanguage = "es-ES";
        config.SpeechSynthesisLanguage = StaticVariables.getIdioma();
        configurarSpeechConfigFromLanguage();

        synthesizer = new SpeechSynthesizer(config);
    }

    void configurarSpeechConfigFromLanguage()
    {
        switch (StaticVariables.getIdioma())
        {
            case "es-ES":
                config.SpeechSynthesisVoiceName = "es-ES-ElviraNeural";
                break;
            case "ca-ES":
                config.SpeechSynthesisVoiceName = "ca-ES-AlbaNeural";
                break;
            case "en-US":
                config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                break;
            case "it-IT":
                config.SpeechSynthesisVoiceName = "it-IT-IsabellaNeural";
                break;
            case "zh-CN":
                config.SpeechSynthesisVoiceName = "zh-CN-YunyangNeural";
                break;
            default:
                break;
        }
    }

    public void empezarSpeechSimple(string s)
    {
        if (!s.Equals(""))
        {
            realizarSpeech(s);
        }
    }

    public void empezarSpeechComplejo(int color, string pieza, string posOrigen, string posDestino)
    {
        string n = "";
        if (color == 0)
            n += "Blanco ha movido ";
        else
            n += "Negro ha movido ";

        switch (pieza)
        {
            case "p":
                n += "peon ";
                break;
            case "r":
                n += "rey ";
                break;
            case "c":
                n += "caballo ";
                break;
            case "a":
                n += "alfil ";
                break;
            case "t":
                n += "torre ";
                break;
            case "d":
                n += "dama ";
                break;
            default:
                n += "";
                break;
        }

        n += posOrigen + " " + posDestino;

        realizarSpeech(n);
    }

    void Speech(string s)
    {
        Thread trd = new Thread(() => realizarSpeech(s));
        trd.IsBackground = true;
        trd.Start();
    }

    async void realizarSpeech(string s)
    {

        string trad = s;
        if (!StaticVariables.getIdioma().Equals("es-ES"))
        {
            trad = await Translator.traducirTextoDelOriginal(s, "es", StaticVariables.getIdiomaTraduccion());
        }
        await synthesizer.StartSpeakingTextAsync(trad);

    }

    public string getFraseAleatoria(int i)
    {
        int r = UnityEngine.Random.Range(0, 100);

        string result;

        if (r <= 25) //El 25% de las veces dirá algo
        {
            var res = FrasesConversacionIA.FrasesAleatorias(i);
            result = res[UnityEngine.Random.Range(0, res.Length - 1)];
        }
        else
            result = null;

        return result;

    }

    public void pararSpeechs()
    {
        var result = synthesizer.StopSpeakingAsync();
        result.Wait();
        StaticVariables.setText2SpeechEnCurso(false);
    }

}
