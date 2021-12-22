using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiezasClases
{
    public class Dama : Pieza
    {
        public Dama(int c, Posicion p) : base(c, p)
        {
            nombre = "d";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {
            listaPosDis = new List<Posicion>();
            listaPosDis = MovTorre(tablero);
            listaPosDis.AddRange(MovAlfil(tablero));
            return listaPosDis;
        }

        List<Posicion> MovTorre(Pieza[,] tablero)
        {
            List<Posicion> aux = new List<Posicion>();

            int auxx = posicion.x;
            int auxy = posicion.y;

            //movimiento hacia arriba (y+)
            auxy++;
            while (auxy < 8)
            {

                if (tablero[posicion.x, auxy] == null) aux.Add(new Posicion(posicion.x, auxy));
                else
                {
                    if (tablero[posicion.x, auxy].color != this.color) aux.Add(new Posicion(posicion.x, auxy));
                    break;
                }

                auxy++;

            }

            auxy = posicion.y;

            //movimiento hacia la derecha (x+)
            auxx++;
            while (auxx < 8)
            {

                if (tablero[auxx, posicion.y] == null) aux.Add(new Posicion(auxx, posicion.y));
                else
                {
                    if (tablero[auxx, posicion.y].color != this.color) aux.Add(new Posicion(auxx, posicion.y));
                    break;
                }
                auxx++;
            }

            auxx = posicion.x;

            //movimiento hacia abajo (y-)
            auxy--;
            while (auxy >= 0)
            {

                if (tablero[posicion.x, auxy] == null) aux.Add(new Posicion(posicion.x, auxy));
                else
                {
                    if (tablero[posicion.x, auxy].color != this.color) aux.Add(new Posicion(posicion.x, auxy));
                    break;
                }

                auxy--;

            }

            //movimiento hacia la izquierda (x-)
            auxx--;
            while (auxx >= 0)
            {

                if (tablero[auxx, posicion.y] == null) aux.Add(new Posicion(auxx, posicion.y));
                else
                {
                    if (tablero[auxx, posicion.y].color != this.color) aux.Add(new Posicion(auxx, posicion.y));
                    break;
                }
                auxx--;
            }
            return aux;
        }

        List<Posicion> MovAlfil(Pieza[,] tablero)
        {
            List<Posicion> aux = new List<Posicion>();

            int auxx = posicion.x;
            int auxy = posicion.y;

            //movimiento hacia arriba a la derecha (x+, y+)
            auxx++; auxy++;
            while (auxx < 8 && auxy < 8)
            {

                if (tablero[auxx, auxy] == null) aux.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) aux.Add(new Posicion(auxx, auxy));
                    break;
                }

                auxx++;
                auxy++;

            }

            auxx = posicion.x;
            auxy = posicion.y;

            //movimiento hacia abajo a la derecha (x+, y-)
            auxx++; auxy--;
            while (auxx < 8 && auxy >= 0)
            {

                if (tablero[auxx, auxy] == null) aux.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) aux.Add(new Posicion(auxx, auxy));
                    break;
                }

                auxx++;
                auxy--;

            }

            auxx = posicion.x;
            auxy = posicion.y;

            //movimiento hacia abajo a la izquierda (x-, y-)
            auxx--; auxy--;
            while (auxx >= 0 && auxy >= 0)
            {

                if (tablero[auxx, auxy] == null) aux.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) aux.Add(new Posicion(auxx, auxy));
                    break;
                }

                auxx--;
                auxy--;

            }

            auxx = posicion.x;
            auxy = posicion.y;

            //movimiento hacia arriba a la izquierda (x-, y+)
            auxx--; auxy++;
            while (auxx >= 0 && auxy < 8)
            {

                if (tablero[auxx, auxy] == null) aux.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) aux.Add(new Posicion(auxx, auxy));
                    break;
                }

                auxx--;
                auxy++;

            }
            return aux;
        }

    }
}
