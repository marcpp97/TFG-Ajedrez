using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Partida
{
    public string nombre;
    public List<(string, int, int)> movimientosPartida;
    public int relojBlanco;
    public int relojNegro;
    public int colorJugador;
    //false si no ha acabado y true si ha acabado
    public bool finPartida;
    //0 si pierde blanco, 1 si pierde negro, 2 si es empate , -1 si no hay conexion y -2 si no se consigue crear Stockfish
    public int pierde;

    public Partida(string n)
    {
        nombre = n;
        movimientosPartida = new List<(string, int, int)>();
        relojBlanco = 60 * 10;
        relojNegro = 60 * 10;
        colorJugador = ColorAleatorio();
        finPartida = false;
        pierde = -1;
    }

    public Partida(string n, List<(string, int, int)> f, int rb, int rn, int c, bool fin, int p)
    {
        nombre = n;
        movimientosPartida = f;
        relojBlanco = rb;
        relojNegro = rn;
        colorJugador = c;
        finPartida = fin;
        pierde = p;
    }

    int ColorAleatorio()
    {
        System.Random rd = new System.Random();
        int n = rd.Next(0, 100);
        if (n <= 50)
            return 0;
        return 1;
    }

}
