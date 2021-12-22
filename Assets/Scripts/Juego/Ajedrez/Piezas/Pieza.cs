using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiezasClases
{
    public abstract class Pieza
    {

        public string nombre { get; set; }

        //0 es blancas y 1 negras
        public int color { get; }
        public Posicion posicion { get; set; }

        //es true si ha movido por lo menos una vez
        public bool hasMove { get; set; }

        public List<Posicion> listaPosDis;

        public GameObject pieza { get; set; }

        public Pieza(int c, Posicion p)
        {

            color = c;
            posicion = p;
            hasMove = false;

        }

        public void CrearPiezaGameObject(Vector3 posEs, Sprite spr, Transform padre)
        {
            pieza = new GameObject();

            posEs.z = posEs.z - 1;
            pieza.transform.position = posEs;
            pieza.transform.parent = padre;
            pieza.AddComponent<SpriteRenderer>().sprite = spr;
            pieza.transform.localScale = new Vector3(0.3f,0.3f,0);
            pieza.name = nombre + " " + color;

        }

        public abstract List<Posicion> getPosDis(Pieza[,] tablero);

    }
}

