using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla
{

    Posicion posicion;
    GameObject casilla;

    public Casilla(Posicion pos, GameObject cas)
    {
        posicion = pos;
        casilla = cas;
    }

    public Posicion GetPosicion()
    {
        return posicion;
    }

    public GameObject GetCasilla()
    {
        return casilla;
    }

}
