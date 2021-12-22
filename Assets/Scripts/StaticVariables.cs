using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticVariables
{

    //idioma en el que se esta jugando
    static string idioma = "es-ES";

    static bool pantallaCompleta = Screen.fullScreen;

    //true si juega con el raton, false si juega hablando
    static bool juegoRaton = true;

    //true si juega multijugador, false si juega en local(IA)
    static bool multijugador = false;

    //la partida que se esta jugando o viendo
    static Partida partida = null;

    //true si se esta viendo una partida ya jugada, false si se esta jugando una partida nueva
    static bool verPartida = false;

    //true si hay una animacion en curso, false si no
    static bool animacionEnCurso = false;

    //true si se esta diciendo algo, false si no
    static bool text2SpeechEnCurso = false;

    //true si estas diciendo algo, false si no
    static bool speech2TextEnCurso = false;

    //Dificultad de la IA si hay, por defecto está en 2 (Normal)
    static int dificultadIA = 10;

    public static void Reiniciar()
    {
        verPartida = false;
        animacionEnCurso = false;
        text2SpeechEnCurso = false;
        speech2TextEnCurso = false;
    }

    public static void setIdioma(string i)
    {
        idioma = i;
    }

    public static string getIdioma()
    {
        return idioma;
    }

    public static string getIdiomaTraduccion()
    {
        string[] aux = idioma.Split('-');
        if (aux[0].Equals("zh"))
            return "zh-Hans";
        return aux[0];
    }

    public static void setPantalla(bool b)
    {
        pantallaCompleta = b;
    }

    public static bool getPantalla()
    {
        return pantallaCompleta;
    }

    public static void setFinPartida(bool n)
    {
        partida.finPartida = n;
    }

    public static bool getFinPartida()
    {
        return partida.finPartida;
    }

    public static void setJuegoRaton(bool s)
    {
        juegoRaton = s;
    }

    public static bool getJuegoRaton()
    {
        return juegoRaton;
    }

    public static void setColorJugador(int c)
    {
        partida.colorJugador = c;
    }

    public static int getColorJugador()
    {
        return partida.colorJugador;
    }

    public static void setMultijugador(bool b)
    {
        multijugador = b;
    }

    public static bool getMultijugador()
    {
        return multijugador;
    }

    public static void setColorPierde(int n)
    {
        partida.pierde = n;
    }

    public static int getColorPierde()
    {
        return partida.pierde;
    }

    public static void setPartida(Partida p)
    {
        partida = p;
    }

    public static Partida getPartida()
    {
        return partida;
    }

    public static void setVerPartida(bool b)
    {
        verPartida = b;
    }

    public static bool getVerPartida()
    {
        return verPartida;
    }

    public static void setAnimacionEnCurso(bool b)
    {
        animacionEnCurso = b;
    }

    public static bool getAnimacionEnCurso()
    {
        return animacionEnCurso;
    }

    public static void setText2SpeechEnCurso(bool b)
    {
        text2SpeechEnCurso = b;
    }

    public static bool getText2SpeechEnCurso()
    {
        return text2SpeechEnCurso;
    }

    public static void setSpeech2TextEnCurso(bool b)
    {
        speech2TextEnCurso = b;
    }

    public static bool getSpeech2TextEnCurso()
    {
        return speech2TextEnCurso;
    }

    public static void setDificultadIA(int d)
    {
        dificultadIA = d;
    }

    public static int getDificultadIA()
    {
        return dificultadIA;
    }

}
