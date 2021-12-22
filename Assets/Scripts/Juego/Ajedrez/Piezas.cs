using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PiezasClases;
using Photon.Pun;

public class Piezas : MonoBehaviour
{

    Sprite[] piezasSprites { set; get; }

    public Pieza[,] tableroPiezas { get; set; }
    Tablero tableroCas;

    Posicion reyBlanco;
    Posicion reyNegro;

    Posicion posReyPosible;

    public Pieza piezaEnCambio;

    public float velocidadAnimacion = 20;

    public void iniciar(Sprite[] piezas, Tablero tableroCas)
    {

        piezasSprites = piezas;
        this.tableroCas = tableroCas;
        tableroPiezas = new Pieza[8, 8];

        posReyPosible = null;
        piezaEnCambio = null;
    }

    public void RecargarPiezas()
    {
        foreach (var p in tableroPiezas)
        {
            if(p != null)
                Destroy(p.pieza);
        }
        tableroPiezas = new Pieza[8,8];
        posReyPosible = null;
        piezaEnCambio = null;
    }

    public Pieza GetPieza(Posicion p)
    {
        Pieza pie = tableroPiezas[p.x, p.y];
        if (pie != null)
            return pie;
        return null;
    }

    public void CrearPieza(string n, int c, Posicion pos)
    {
        tableroPiezas[pos.x, pos.y] = seleccionarPieza(n, c, pos);

        if(n.Equals("r"))
            if (c == 0)
                reyBlanco = pos;
            else
                reyNegro = pos;

        if (n.Equals("p"))
        {
            if ((pos.y == 6 && c == 1) || (pos.y == 1 && c == 0))
                tableroPiezas[pos.x, pos.y].hasMove = false;
            else
                tableroPiezas[pos.x, pos.y].hasMove = true;
        }

        Vector3 posEs = tableroCas.tableroCasillas[pos.x, pos.y].GetCasilla().transform.position;

        tableroPiezas[pos.x, pos.y].CrearPiezaGameObject(posEs, SeleccionarSprite(n, c), this.transform);

        if (StaticVariables.getColorJugador() == c && StaticVariables.getMultijugador())
        {
            tableroPiezas[pos.x, pos.y].pieza.AddComponent<PhotonView>();
            var aux = tableroPiezas[pos.x, pos.y].pieza.AddComponent<PhotonTransformView>();
            aux.m_SynchronizePosition = true;
            aux.m_SynchronizeScale = true;
            aux.m_UseLocal = true;
        }

    }

    public Pieza seleccionarPieza(string n, int c, Posicion pos)
    {
        Pieza p;

        switch (n)
        {
            case "p":
                p = new Peon(c, pos);
                break;
            case "r":
                p = new Rey(c, pos);
                break;
            case "c":
                p = new Caballo(c, pos);
                break;
            case "a":
                p = new Alfil(c, pos);
                break;
            case "t":
                p = new Torre(c, pos);
                break;
            case "d":
                p = new Dama(c, pos);
                break;
            default:
                p = null;
                break;
        }

        return p;

    }

    Sprite SeleccionarSprite(string n, int c)
    {
        int index;

        switch (n)
        {
            case "p":
                if (c == 0)
                    index = 0;
                else
                    index = 1;
                break;
            case "r":
                if (c == 0)
                    index = 2;
                else
                    index = 3;
                break;
            case "c":
                if (c == 0)
                    index = 4;
                else
                    index = 5;
                break;
            case "a":
                if (c == 0)
                    index = 6;
                else
                    index = 7;
                break;
            case "t":
                if (c == 0)
                    index = 8;
                else
                    index = 9;
                break;
            case "d":
                if (c == 0)
                    index = 10;
                else
                    index = 11;
                break;
            default:
                index = -1;
                break;
        }

        if (index != -1)
            return piezasSprites[index];
        return null;

    }

    public List<Posicion> GetPosDispPieza(Pieza p, Pieza[,] tablero)
    {
        return p.getPosDis(tablero);
    }

    public bool isPosMov(Pieza pieza, Posicion casilla)
    {
        
        //var lista = GetPosDispPieza(pieza, tableroPiezas);
        var lista = PosReyNoAtacado(pieza); // el peon se mueve antes no se porque
        
        //Mirar si el rey es atacado, poner solo las casillas que no haran el jaque mate
        return lista.Exists(aux => aux.x == casilla.x && aux.y == casilla.y);
        

    }

    public string MoverPieza(Pieza pie, Posicion posCas, Vector3 posEs, Posicion enPassant, int turno)
    {
        Pieza goCas;
        Ajedrez ar;
        if (!StaticVariables.getVerPartida())
            ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
        else
            ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();
        //si hay una pieza la destruye
        string comer = "";

        ar.halfmoveClock++;

        if (pie.nombre.Equals("p"))
            ar.halfmoveClock = 0;

        if (enPassant != null && posCas.isEqual(enPassant) && pie.nombre.Equals("p"))
        {
            
            Posicion aux;
            if (turno != 0)
            {
                goCas = tableroPiezas[posCas.x, posCas.y + 1];
                aux = new Posicion(posCas.x, posCas.y + 1);
            }
            else
            {
                goCas = tableroPiezas[posCas.x, posCas.y - 1];
                aux = new Posicion(posCas.x, posCas.y - 1);
            }
            tableroPiezas[aux.x, aux.y] = null;

            comer = goCas.nombre;
            Destroy(goCas.pieza);
        }
        else
        {
            goCas = tableroPiezas[posCas.x, posCas.y];
        }
        
        if (goCas != null)
        {
            ar.halfmoveClock = 0;
            if (goCas.nombre.Equals("t") && !goCas.hasMove)
            {
                if (goCas.posicion.x == 0)
                    ar.enroques[goCas.color, 1] = false;
                if (goCas.posicion.x == 7)
                    ar.enroques[goCas.color, 0] = false;
            }
            comer = goCas.nombre;
            Destroy(goCas.pieza);
        }
            
        movimientosDistintasPiezas(pie, posCas);

        //quita la pieza de la posicion original
        tableroPiezas[pie.posicion.x, pie.posicion.y] = null;

        //Pone la pieza en el lugar nuevo
        pie.posicion = posCas;

        //Mover pieza al lugar correspondiente (si esta jugando el jugador con raton se mueve instantaneamente, si no se realiza una animación)
        if(StaticVariables.getJuegoRaton() && StaticVariables.getColorJugador() == turno)
            pie.pieza.transform.position = posEs;
        else
            StartCoroutine(animacionMoverPieza(pie.pieza.transform, posEs, velocidadAnimacion));

        tableroPiezas[posCas.x, posCas.y] = pie;

        pie.hasMove = true;

        if(pie.nombre.Equals("p"))
        {
            if(pie.posicion.y == 0 || pie.posicion.y == 7) 
            {
                if (StaticVariables.getColorJugador() == turno) {
                    GameObject go;
                    if (turno == 0)
                        go = GameObject.Find("EleccionPromocionBlancas");
                    else
                        go = GameObject.Find("EleccionPromocionNegras");

                    var aux = pie.pieza.transform.position;
                    go.transform.position = Camera.main.WorldToScreenPoint(new Vector3(aux.x, (aux.y / 2) + 0.1f, aux.z));
                }
                piezaEnCambio = pie;
            }
        }
        return comer;
    }

    public bool promocionarPieza(string p, int turno)
    {
        GameObject go;
        if (turno == 0)
            go = GameObject.Find("EleccionPromocionNegras");
        else
            go = GameObject.Find("EleccionPromocionBlancas");

        go.transform.position = new Vector3(1285, -1000, 0);

        bool result = false;

        if (!p.Equals(""))
        {
            Destroy(piezaEnCambio.pieza);
            CrearPieza(p, piezaEnCambio.color, piezaEnCambio.posicion);
            result = true;
        }
        piezaEnCambio = null;
        return result;
    }

    void movimientosDistintasPiezas(Pieza p, Posicion pos)
    {
        if (p.nombre.Equals("r"))
            reyMovimientosYEnroques(p, pos);
        if (p.nombre.Equals("t"))
            torreEnroques(p);
    }

    void torreEnroques(Pieza pie)
    {
        if (!pie.hasMove)
        {
            Ajedrez ar;
            if (!StaticVariables.getVerPartida())
                ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
            else
                ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

            if (pie.posicion.x == 0)
                ar.enroques[pie.color, 1] = false;
            if (pie.posicion.x == 7)
                ar.enroques[pie.color, 0] = false;

        }
    }

    void reyMovimientosYEnroques(Pieza p, Posicion pos)
    {
        
        if (p.color == 0)
            reyBlanco = pos;
        else
            reyNegro = pos;

        if (!p.hasMove)
        {
            Ajedrez ar;
            if (!StaticVariables.getVerPartida())
                ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
            else
                ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

            Pieza torre = null;
            //Enroque corto
            if (pos.isEqual(new Posicion(6, p.posicion.y)) && tableroPiezas[7, p.posicion.y].pieza != null && !tableroPiezas[7, p.posicion.y].hasMove)
            {
                torre = tableroPiezas[7, p.posicion.y];
                tableroPiezas[torre.posicion.x, torre.posicion.y] = null;
                torre.posicion = new Posicion(pos.x - 1, p.posicion.y);
                
            }

            //Enroque largo
            if (pos.isEqual(new Posicion(2, p.posicion.y)) && tableroPiezas[0, p.posicion.y].pieza != null && !tableroPiezas[0, p.posicion.y].hasMove)
            {
                torre = tableroPiezas[0, p.posicion.y];
                tableroPiezas[torre.posicion.x, torre.posicion.y] = null;
                torre.posicion = new Posicion(pos.x + 1, p.posicion.y);
                
            }

            if (torre != null)
            {
                Vector3 esp = ar.GetVector3Casilla(torre.posicion);
                esp = new Vector3(esp.x, esp.y, -1f);
                //torre.pieza.transform.position = esp;
                StartCoroutine(animacionMoverPieza(torre.pieza.transform, esp, velocidadAnimacion));
                tableroPiezas[torre.posicion.x, torre.posicion.y] = torre;
            }

            ar.enroques[p.color, 0] = false;
            ar.enroques[p.color, 1] = false;

        }

    }

    public List<Posicion> getPosDispPiezasColor(int color, Pieza[,] tablero)
    {
        List<Posicion> listaPosDisPiezasColor = new List<Posicion>();

        foreach (var p in tablero)
        {
            if (p != null && p.color == color)
            {
                listaPosDisPiezasColor.AddRange(GetPosDispPieza(p, tablero));
            }
                
        }
        
        return listaPosDisPiezasColor;

    }
    
    public bool isReyAtacado(int color, Pieza[,] tablero)
    {
        Posicion posRey;
        List<Posicion> lista;

        if(color == 0)
        {
            posRey = reyBlanco;
            lista = getPosDispPiezasColor(1, tablero);
        } else
        {
            posRey = reyNegro;
            lista = getPosDispPiezasColor(0, tablero);
        }

        if(posReyPosible != null)
        {
            posRey = posReyPosible;
            posReyPosible = null;
        }
        
        return lista.Exists(aux => aux.x == posRey.x && aux.y == posRey.y);
    }

    public List<Posicion> PosReyNoAtacado(Pieza pieza)
    {

        var lista = GetPosDispPieza(pieza, tableroPiezas);

        List<Posicion> listaAux = new List<Posicion>(lista);

        Ajedrez ar;
        if(!StaticVariables.getVerPartida())
            ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
        else
            ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

        foreach (var pos in lista)
        {
            
            Pieza[,] tableroAux = new Pieza[8,8];
            var piezaAux = seleccionarPieza(pieza.nombre, pieza.color, pieza.posicion);

            System.Array.Copy(tableroPiezas, tableroAux, tableroPiezas.Length);

            tableroAux[piezaAux.posicion.x, piezaAux.posicion.y] = null;

            piezaAux.posicion = pos;
            tableroAux[pos.x, pos.y] = piezaAux;

            if(ar.enPassant != null && pos.isEqual(ar.enPassant))
            {
                if (piezaAux.color == 0)
                    tableroAux[piezaAux.posicion.x, piezaAux.posicion.y - 1] = null;
                else
                    tableroAux[piezaAux.posicion.x, piezaAux.posicion.y + 1] = null;
            }

            if (piezaAux.nombre.Equals("r"))
            {
                posReyPosible = piezaAux.posicion;

            }
            
            if(isReyAtacado(piezaAux.color, tableroAux))
            {
                listaAux.Remove(pos);
            }
            
            if (checkPosibleEnroque(piezaAux, tableroPiezas))
            {
                listaAux.Remove(pos);
            }

        }
        return listaAux;
    }

    bool checkPosibleEnroque(Pieza p, Pieza[,] tableroAux)
    {
        if (p.nombre.Equals("r"))
        {
            Ajedrez ar;
            if (!StaticVariables.getVerPartida())
                ar = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
            else
                ar = GameObject.Find("Juego").GetComponent<VerPartida>().getAjedrez();

            //Enroque corto
            if (ar.enroques[p.color, 0] && p.posicion.isEqual(new Posicion(6, p.posicion.y)))
            {
                int colorOpon;
                if (p.color == 0)
                    colorOpon = 1;
                else
                    colorOpon = 0;
                List<Posicion> lista = getPosDispPiezasColor(colorOpon, tableroAux);
                return lista.Exists(aux => aux.x == p.posicion.x && aux.y == p.posicion.y) || lista.Exists(aux => aux.x == p.posicion.x - 1 && aux.y == p.posicion.y) || lista.Exists(aux => aux.x == p.posicion.x - 2 && aux.y == p.posicion.y);
            }

            //Enroque largo
            if (ar.enroques[p.color, 1] && p.posicion.isEqual(new Posicion(2, p.posicion.y)))
            {
                int colorOpon;
                if (p.color == 0)
                    colorOpon = 1;
                else
                    colorOpon = 0;
                List<Posicion> lista = getPosDispPiezasColor(colorOpon, tableroAux);
                return lista.Exists(aux => aux.x == p.posicion.x && aux.y == p.posicion.y) || lista.Exists(aux => aux.x == p.posicion.x + 1 && aux.y == p.posicion.y) || lista.Exists(aux => aux.x == p.posicion.x + 2 && aux.y == p.posicion.y);
            }
        }
        return false;
    }

    public List<Pieza> getPiezasColor(int color)
    {
        List<Pieza> lista = new List<Pieza>();
        foreach (var p in tableroPiezas)
        {
            if (p != null && p.color == color)
                lista.Add(p);
        }
        return lista;
    }

    IEnumerator animacionMoverPieza(Transform pieza, Vector3 posDestino, float speed)
    {
        StaticVariables.setAnimacionEnCurso(true);
        while (pieza.position != posDestino)
        {
            pieza.position = Vector3.Lerp(pieza.position, posDestino, Time.deltaTime * speed);
            yield return null;
        }
        StaticVariables.setAnimacionEnCurso(false);
    }

}
