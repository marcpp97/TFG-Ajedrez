using System.Collections.Generic;
using UnityEngine;

namespace PiezasClases
{
    public class Peon : Pieza
    {

        public Peon(int c, Posicion p) : base(c, p)
        {
            nombre = "p";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {

            Ajedrez ar;
            if (!StaticVariables.getVerPartida())
                ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
            else
                ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

            Posicion enPas = ar.enPassant;
            int turno = ar.turno;

            listaPosDis = new List<Posicion>();
            //si el peon es blanco se mueve hacia adelante (y+)
            if (color == 0)
            {
                //movimiento peon hacia adelante una casilla
                if (posicion.y + 1 < 8 && tablero[posicion.x, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x, posicion.y + 1));

                //movimiento peon hacia adelante dos casillas solo cuando mueve por primera vez
                if (!hasMove && tablero[posicion.x, posicion.y + 1] == null && tablero[posicion.x, posicion.y + 2] == null) 
                { 
                    listaPosDis.Add(new Posicion(posicion.x, posicion.y + 2));
                }

                //movimiento peon come pieza enemiga hacia la derecha
                if (posicion.x + 1 < 8 && posicion.y + 1 < 8) // && tablero[posicion.x + 1, posicion.y + 1] != null
                {
                    if (tablero[posicion.x + 1, posicion.y + 1] != null)
                    {
                        if (tablero[posicion.x + 1, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 1));
                    }
                    else
                    {
                        if (enPas != null && enPas.isEqual(new Posicion(posicion.x + 1, posicion.y + 1)) && turno == this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 1));
                    }
                }

                //movimiento peon come pieza enemiga hacia la izquierda
                if (posicion.x - 1 >= 0 && posicion.y + 1 < 8)
                {
                    if (tablero[posicion.x - 1, posicion.y + 1] != null)
                    {
                        if (tablero[posicion.x - 1, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 1));
                    }
                    else
                    {
                        if (enPas != null && enPas.isEqual(new Posicion(posicion.x - 1, posicion.y + 1)) && turno == this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 1));
                    }
                }
            }

            //si el peon es negro se mueve hacia atras (y-)
            else
            {
                //movimiento peon hacia adelante una casilla
                if (posicion.y - 1 >= 0 && tablero[posicion.x, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x, posicion.y - 1));

                //movimiento peon hacia adelante dos casillas solo cuando mueve por primera vez
                if (!hasMove && tablero[posicion.x, posicion.y - 1] == null && tablero[posicion.x, posicion.y - 2] == null) 
                { 
                    listaPosDis.Add(new Posicion(posicion.x, posicion.y - 2));
                }

                //movimiento peon come pieza enemiga hacia la derecha
                if (posicion.x + 1 < 8 && posicion.y - 1 >= 0)
                {
                    if (tablero[posicion.x + 1, posicion.y - 1] != null)
                    {
                        if (tablero[posicion.x + 1, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 1));
                    }
                    else
                    {
                        if (enPas != null && enPas.isEqual(new Posicion(posicion.x + 1, posicion.y - 1)) && turno == this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 1));
                    }
                }

                //movimiento peon come pieza enemiga hacia la izquierda
                if (posicion.x - 1 >= 0 && posicion.y - 1 >= 0)
                {
                    if (tablero[posicion.x - 1, posicion.y - 1] != null)
                    {
                        if (tablero[posicion.x - 1, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 1));
                    }
                    else
                    {
                        if (enPas != null && enPas.isEqual(new Posicion(posicion.x - 1, posicion.y - 1)) && turno == this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 1));
                    }
                }
            }
            return listaPosDis;
        }

    }
}
