using System.Collections.Generic;

namespace PiezasClases
{
    public class Caballo : Pieza
    {
        public Caballo(int c, Posicion p) : base(c, p)
        {
            nombre = "c";
        }

        public override List<Posicion> getPosDis(Pieza[,] tablero)
        {
            listaPosDis = new List<Posicion>();

            //movimientos en dirección eje y

            if (posicion.x - 1 >= 0 && posicion.y + 2 < 8 && tablero[posicion.x - 1, posicion.y + 2] == null) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 2));
            else if (posicion.x - 1 >= 0 && posicion.y + 2 < 8 && tablero[posicion.x - 1, posicion.y + 2].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y + 2));

            if (posicion.x + 1 < 8 && posicion.y + 2 < 8 && tablero[posicion.x + 1, posicion.y + 2] == null) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 2));
            else if (posicion.x + 1 < 8 && posicion.y + 2 < 8 && tablero[posicion.x + 1, posicion.y + 2].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y + 2));

            if (posicion.x - 1 >= 0 && posicion.y - 2 >= 0 && tablero[posicion.x - 1, posicion.y - 2] == null) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 2));
            else if (posicion.x - 1 >= 0 && posicion.y - 2 >= 0 && tablero[posicion.x - 1, posicion.y - 2].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 1, posicion.y - 2));

            if (posicion.x + 1 < 8 && posicion.y - 2 >= 0 && tablero[posicion.x + 1, posicion.y - 2] == null) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 2));
            else if (posicion.x + 1 < 8 && posicion.y - 2 >= 0 && tablero[posicion.x + 1, posicion.y - 2].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 1, posicion.y - 2));

            //movimientos en dirección eje x

            if (posicion.x + 2 < 8 && posicion.y + 1 < 8 && tablero[posicion.x + 2, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x + 2, posicion.y + 1));
            else if (posicion.x + 2 < 8 && posicion.y + 1 < 8 && tablero[posicion.x + 2, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 2, posicion.y + 1));

            if (posicion.x + 2 < 8 && posicion.y - 1 >= 0 && tablero[posicion.x + 2, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x + 2, posicion.y - 1));
            else if (posicion.x + 2 < 8 && posicion.y - 1 >= 0 && tablero[posicion.x + 2, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x + 2, posicion.y - 1));

            if (posicion.x - 2 >= 0 && posicion.y + 1 < 8 && tablero[posicion.x - 2, posicion.y + 1] == null) listaPosDis.Add(new Posicion(posicion.x - 2, posicion.y + 1));
            else if (posicion.x - 2 >= 0 && posicion.y + 1 < 8 && tablero[posicion.x - 2, posicion.y + 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 2, posicion.y + 1));

            if (posicion.x - 2 >= 0 && posicion.y - 1 >= 0 && tablero[posicion.x - 2, posicion.y - 1] == null) listaPosDis.Add(new Posicion(posicion.x - 2, posicion.y - 1));
            else if (posicion.x - 2 >= 0 && posicion.y - 1 >= 0 && tablero[posicion.x - 2, posicion.y - 1].color != this.color) listaPosDis.Add(new Posicion(posicion.x - 2, posicion.y - 1));

            return listaPosDis;
        }
    }
}
