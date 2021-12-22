using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiezasClases
{
    public class Rey : Pieza
    {

        public Rey(int c, Posicion p) : base(c, p)
        {
            nombre = "r";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {
            Ajedrez ar;
            if (!StaticVariables.getVerPartida())
                ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
            else
                ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

            bool enrCort = ar.enroques[this.color,0];
            bool enrLarg = ar.enroques[this.color, 1];

            listaPosDis = new List<Posicion>();

            //movimiento hacia adelante
            if (posicion.y + 1 < 8 && tablero[posicion.x, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x, posicion.y + 1));
            else if (posicion.y + 1 < 8 && tablero[posicion.x, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x, posicion.y + 1));

            //movimiento hacia adelante a la derecha
            if (posicion.x + 1 < 8 && posicion.y + 1 < 8 && tablero[posicion.x + 1, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 1));
            else if (posicion.x + 1 < 8 && posicion.y + 1 < 8 && tablero[posicion.x + 1, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 1));

            //movimiento hacia la derecha
            if (posicion.x + 1 < 8 && tablero[posicion.x + 1, posicion.y] == null) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y));
            else if (posicion.x + 1 < 8 && tablero[posicion.x + 1, posicion.y].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y));

            //movimiento hacia atras a la derecha
            if (posicion.x + 1 < 8 && posicion.y - 1 >= 0 && tablero[posicion.x + 1, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 1));
            else if (posicion.x + 1 < 8 && posicion.y - 1 >= 0 && tablero[posicion.x + 1, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 1));

            //movimiento hacia atras
            if (posicion.y - 1 >= 0 && tablero[posicion.x, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x, posicion.y - 1));
            else if (posicion.y - 1 >= 0 && tablero[posicion.x, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x, posicion.y - 1));

            //movimiento hacia atras a la izquierda
            if (posicion.x - 1 >= 0 && posicion.y - 1 >= 0 && tablero[posicion.x - 1, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 1));
            else if (posicion.x - 1 >= 0 && posicion.y - 1 >= 0 && tablero[posicion.x - 1, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 1));

            //movimiento hacia la izquierda
            if (posicion.x - 1 >= 0 && tablero[posicion.x - 1, posicion.y] == null) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y));
            else if (posicion.x - 1 >= 0 && tablero[posicion.x - 1, posicion.y].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y));

            //movimiento hacia adelante a la izquierda
            if (posicion.x - 1 >= 0 && posicion.y + 1 < 8 && tablero[posicion.x - 1, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 1));
            else if (posicion.x - 1 >= 0 && posicion.y + 1 < 8 && tablero[posicion.x - 1, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 1));
            
            //enroque largo (dirección dama)
            if(enrLarg && tablero[posicion.x - 1, posicion.y] == null && tablero[posicion.x - 2, posicion.y] == null && tablero[posicion.x - 3, posicion.y] == null)
                listaPosDis.Add(new Posicion(posicion.x - 2, posicion.y));

            //enroque corto (dirección rey)
            if (enrCort && tablero[posicion.x + 1, posicion.y] == null && tablero[posicion.x + 2, posicion.y] == null)
                listaPosDis.Add(new Posicion(posicion.x + 2, posicion.y));
            
            return listaPosDis;
        }
    }
}
