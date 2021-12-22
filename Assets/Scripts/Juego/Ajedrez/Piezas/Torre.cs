using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiezasClases
{
    public class Torre : Pieza
    {
        public Torre(int c, Posicion p) : base(c, p)
        {
            nombre = "t";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {
            listaPosDis = new List<Posicion>();

            int auxx = posicion.x;
            int auxy = posicion.y;

            //movimiento hacia arriba (y+)
            auxy++;
            while (auxy < 8)
            {

                if (tablero[posicion.x, auxy] == null) listaPosDis.Add(new Posicion(posicion.x, auxy));
                else
                {
                    if (tablero[posicion.x, auxy].color != this.color) listaPosDis.Add(new Posicion(posicion.x, auxy));
                    break;
                }

                auxy++;

            }

            auxy = posicion.y;

            //movimiento hacia la derecha (x+)
            auxx++;
            while (auxx < 8)
            {

                if (tablero[auxx, posicion.y] == null) listaPosDis.Add(new Posicion(auxx, posicion.y));
                else
                {
                    if (tablero[auxx, posicion.y].color != this.color) listaPosDis.Add(new Posicion(auxx, posicion.y));
                    break;
                }
                auxx++;
            }

            auxx = posicion.x;

            //movimiento hacia abajo (y-)
            auxy--;
            while (auxy >= 0)
            {

                if (tablero[posicion.x, auxy] == null) listaPosDis.Add(new Posicion(posicion.x, auxy));
                else
                {
                    if (tablero[posicion.x, auxy].color != this.color) listaPosDis.Add(new Posicion(posicion.x, auxy));
                    break;
                }

                auxy--;

            }

            //movimiento hacia la izquierda (x-)
            auxx--;
            while (auxx >= 0)
            {

                if (tablero[auxx, posicion.y] == null) listaPosDis.Add(new Posicion(auxx, posicion.y));
                else
                {
                    if (tablero[auxx, posicion.y].color != this.color) listaPosDis.Add(new Posicion(auxx, posicion.y));
                    break;
                }
                auxx--;
            }
            return listaPosDis;
        }
    }
}
