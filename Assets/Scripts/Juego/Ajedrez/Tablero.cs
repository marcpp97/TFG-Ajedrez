using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour
{

    Color colorBlanco;
    Color colorNegro;

    GameObject prefabCasilla;

    public Casilla[,] tableroCasillas;

    Posicion posOrigen;
    Posicion posDestino;

    Color colorMovBlanco;
    Color colorMovNegro;

    public void iniciar(Color c1, Color c2, GameObject pref)
    {

        colorBlanco = c1;
        colorNegro = c2;

        prefabCasilla = pref;

        tableroCasillas = new Casilla[8, 8];

        posOrigen = null;
        posDestino = null;

        colorMovBlanco = new Color(1f, 0.76f, 0.625f);
        colorMovNegro = new Color(0.566f, 0.371f, 0.195f);

        CrearTablero();

    }

    void CrearTablero()
    {
        for (int fila = 0; fila < 8; fila++)
        {
            for (int columna = 0; columna < 8; columna++)
            {
                bool esBlanco = (fila + columna) % 2 != 0;

                var colorCasilla = (esBlanco) ? colorBlanco : colorNegro;

                Vector2 posicion;

                if (StaticVariables.getColorJugador() == 0)
                    posicion = new Vector2(-3.5f + fila, -3.5f + columna);
                else
                    posicion = new Vector2(3.5f - fila, 3.5f - columna);

                tableroCasillas[fila, columna] = new Casilla(new Posicion(fila, columna), CrearCasilla(colorCasilla, posicion, fila + "" + columna));
            }
        }
    }

    GameObject CrearCasilla(Color color, Vector2 posicion, string name)
    {

        GameObject go = Instantiate(prefabCasilla, posicion, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b);
        go.transform.parent = this.gameObject.transform;
        go.AddComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        go.name = name;
        return go;

    }

    public Posicion getPosicion(GameObject casilla)
    {

        if(casilla != null)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (casilla == tableroCasillas[i, j].GetCasilla())
                        return tableroCasillas[i, j].GetPosicion();
                }
            }
        }

        return null;
    }

    public Vector3 getPosEsp(Posicion pos)
    {
        return tableroCasillas[pos.x, pos.y].GetCasilla().transform.position;
    }

    public void CambiarColor(Color c1, Color c2)
    {
        if (c1 != colorBlanco || c2 != colorNegro) {

            colorBlanco = c1;
            colorNegro = c2;

            for (int fila = 0; fila < 8; fila++)
            {
                for (int columna = 0; columna < 8; columna++)
                {
                    bool esBlanco = (fila + columna) % 2 != 0;

                    var colorCasilla = (esBlanco) ? colorBlanco : colorNegro;

                    tableroCasillas[fila, columna].GetCasilla().GetComponent<SpriteRenderer>().color = new Color(colorCasilla.r, colorCasilla.g, colorCasilla.b);
                }
            }
        }

    }

    public void marcarCasillasMovimiento(Posicion pO, Posicion pD)
    {
        cambiarColorCasillaMov(pO);
        cambiarColorCasillaMov(pD);

        posOrigen = pO;
        posDestino = pD;

    }

    public void cambiarCasillasMarcadasAColorOriginal()
    {
        if (posOrigen != null)
            cambiarColorCasillaAOrigen(posOrigen);
        if (posDestino != null)
            cambiarColorCasillaAOrigen(posDestino);
    }

    void cambiarColorCasillaAOrigen(Posicion p)
    {
        bool esBlanco = (p.x + p.y) % 2 != 0;

        var colorCasilla = (esBlanco) ? colorBlanco : colorNegro;

        tableroCasillas[p.x, p.y].GetCasilla().GetComponent<SpriteRenderer>().color = new Color(colorCasilla.r, colorCasilla.g, colorCasilla.b);
    }

    public void cambiarColorCasillaMov(Posicion p)
    {
        bool esBlanco = (p.x + p.y) % 2 != 0;

        var colorCasilla = (esBlanco) ? colorMovBlanco : colorMovNegro;

        tableroCasillas[p.x, p.y].GetCasilla().GetComponent<SpriteRenderer>().color = new Color(colorCasilla.r, colorCasilla.g, colorCasilla.b);
    }

    public void RecargarTablero()
    {
        foreach (var c in tableroCasillas)
        {
            Destroy(c.GetCasilla());
        }
        tableroCasillas = new Casilla[8,8];

        CrearTablero();
    }

}
