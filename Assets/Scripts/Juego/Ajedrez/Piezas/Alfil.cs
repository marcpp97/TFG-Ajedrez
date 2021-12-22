using System.Collections.Generic;

namespace PiezasClases
{
    public class Alfil : Pieza
    {
        public Alfil(int c, Posicion p) : base(c, p)
        {
            nombre = "a";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {
            listaPosDis = new List<Posicion>();

            int auxx = posicion.x;
            int auxy = posicion.y;

            //movimiento hacia arriba a la derecha (x+, y+)
            auxx++; auxy++;
            while (auxx < 8 && auxy < 8)
            {

                if (tablero[auxx, auxy] == null) listaPosDis.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) listaPosDis.Add(new Posicion(auxx, auxy));
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

                if (tablero[auxx, auxy] == null) listaPosDis.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) listaPosDis.Add(new Posicion(auxx, auxy));
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

                if (tablero[auxx, auxy] == null) listaPosDis.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) listaPosDis.Add(new Posicion(auxx, auxy));
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

                if (tablero[auxx, auxy] == null) listaPosDis.Add(new Posicion(auxx, auxy));
                else
                {
                    if (tablero[auxx, auxy].color != this.color) listaPosDis.Add(new Posicion(auxx, auxy));
                    break;
                }

                auxx--;
                auxy++;

            }
            return listaPosDis;
        }
    }
}
