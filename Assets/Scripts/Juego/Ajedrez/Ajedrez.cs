using System;
using PiezasClases;
using UnityEngine;
using System.Collections.Generic;

public class Ajedrez
{
    public Color colorBlanco;
    public Color colorNegro;
    public int numeroColorTablero = 0;

    public GameObject prefabCasilla;
    public Sprite spriteMovCas;
    public Sprite spriteComCas;
    public Sprite[] piezasSprites;

    Tablero tableroCasillas;
    Piezas tableroPiezas;

    List<GameObject> listaResaltados;

    string FEN;

    //0 es blancas y 1 es negras
    public int turno { get; set; }

    public Posicion enPassant;

    public bool[,] enroques;

    public int halfmoveClock;
    int fullmoveClock;

    //Relojes
    public Reloj rb;
    public Reloj rn;

    //Coordenadas
    GameObject cN;
    GameObject cB;

    //Piezas Comidas
    public GameObject piezasComidasBlancas;
    public GameObject piezasComidasNegras;

    public Ajedrez(Sprite[] spr, GameObject pref, Sprite prefabMov, Sprite prefabCom, Color c1, Color c2)
    {

        piezasSprites = spr;
        prefabCasilla = pref;
        spriteMovCas = prefabMov;
        spriteComCas = prefabCom;
        colorBlanco = c1;
        colorNegro = c2;

        rb = GameObject.Find("RelojBlanco").GetComponent<Reloj>();
        rn = GameObject.Find("RelojNegro").GetComponent<Reloj>();

        cN = GameObject.Find("CoordNegras");
        cB = GameObject.Find("CoordBlancas");

        piezasComidasBlancas = GameObject.Find("PiezasComidasBlancas");
        piezasComidasNegras = GameObject.Find("PiezasComidasNegras");

        listaResaltados = new List<GameObject>();

        enPassant = null;
        enroques = new bool[2,2];

        //StaticVariables.setFinPartida(0);

        if (StaticVariables.getPartida().movimientosPartida.Count == 0)
        {
            //estado tablero, quien mueve, enroques, en passant, movimientos totales despues de negro, movimientos desde mov peon o comer
            FEN = "tcadract/pppppppp/8/8/8/8/PPPPPPPP/TCADRACT b RDrd - 0 1";
            //FEN = "r6t/pTp2p1d/2Ap1p2/5P2/P7/4A3/5P2/R5d1 b - - 0 1";

            (string, int, int) tupla;
            tupla.Item1 = FEN;
            tupla.Item2 = StaticVariables.getPartida().relojBlanco;
            tupla.Item3 = StaticVariables.getPartida().relojNegro;
            
            StaticVariables.getPartida().movimientosPartida.Add(tupla);
        }
        else
        {
            var aux = StaticVariables.getPartida().movimientosPartida[0];
            FEN = aux.Item1;
            rb.setTiempo(aux.Item2);
            rn.setTiempo(aux.Item3);
        }
    }

    public void iniciar()
    {
        string[] secuencias = FEN.Split(' ');

        CrearTablero();
        CrearPiezas();
        CargarUI();
        cargaDesdeFEN(secuencias);
        
    }

    void CargarUI()
    {
        GameObject goJ;
        GameObject goO;

        GameObject rb = GameObject.Find("RelojBlanco");
        GameObject rn = GameObject.Find("RelojNegro");

        var pJR = Camera.main.WorldToScreenPoint(new Vector3(-7.1f, -3.6f, 0));
        var pOR = Camera.main.WorldToScreenPoint(new Vector3(-7.1f, 3.6f, 0));

        var pJP = Camera.main.WorldToScreenPoint(new Vector3(-7.1f, -4.5f, 0));
        var pOP = Camera.main.WorldToScreenPoint(new Vector3(-7.1f, 4.5f, 0));

        if (StaticVariables.getColorJugador() == 0)
        {
            goJ = cB;
            goO = cN;
            rb.transform.position = pJR;
            rn.transform.position = pOR;
            if (piezasComidasBlancas != null && piezasComidasNegras != null)
            {
                piezasComidasBlancas.transform.position = pOP;
                piezasComidasNegras.transform.position = pJP;
            }
        }
        else
        {
            goJ = cN;
            goO = cB;
            rb.transform.position = pOR;
            rn.transform.position = pJR;
            if (piezasComidasBlancas != null && piezasComidasNegras != null)
            {
                piezasComidasBlancas.transform.position = pJP;
                piezasComidasNegras.transform.position = pOP;
            }
        }
        if (piezasComidasBlancas != null && piezasComidasNegras != null)
        {
            piezasComidasBlancas.GetComponent<PiezasComidas>().iniciar(0, piezasSprites);
            piezasComidasNegras.GetComponent<PiezasComidas>().iniciar(1, piezasSprites);
        }

        goJ.SetActive(true);
        goO.SetActive(false);
    }

    void CrearTablero()
    {

        GameObject go = new GameObject();
        go.name = "Tablero";
        tableroCasillas = go.AddComponent<Tablero>();
        tableroCasillas.iniciar(colorBlanco, colorNegro, prefabCasilla);

    }

    void CrearPiezas()
    {

        GameObject go = new GameObject();
        go.name = "Piezas";
        tableroPiezas = go.AddComponent<Piezas>();
        tableroPiezas.iniciar(piezasSprites, tableroCasillas);

    }

    void cargaDesdeFEN(string[] secuencias)
    {

        //Carga del tablero

        cargaTablero(secuencias[0]);

        //Turno

        if (secuencias[1].Equals("b")) turno = 0;
        else turno = 1;

        //Enroques

        setEnroques(secuencias[2]);

        //En Passant

        if (secuencias[3].Equals("-")) enPassant = null;
        else enPassant = getPosicionFromString(secuencias[3]);
        
        //Tiempos

        //Halfmove
        halfmoveClock = Convert.ToInt32(secuencias[4]);
        //Fullmove
        fullmoveClock = Convert.ToInt32(secuencias[5]);

        /*
        if(!secuencias[6].Equals("-") && !secuencias[7].Equals("-"))
        {
            rb.setTiempo(Convert.ToInt32(secuencias[6]));
            rn.setTiempo(Convert.ToInt32(secuencias[7]));
        }*/

        rb.setTiempo(StaticVariables.getPartida().relojBlanco);
        rn.setTiempo(StaticVariables.getPartida().relojNegro);

    }

    void setEnroques(string enr)
    {

        /*if (enr[0].Equals('R'))
            enroques[0, 0] = true;
        else
            enroques[0, 0] = false;

        if (enr[1].Equals('D'))
            enroques[0, 1] = true;
        else
            enroques[0, 1] = false;

        if (enr[2].Equals('r'))
            enroques[1, 0] = true;
        else
            enroques[1, 0] = false;

        if (enr[3].Equals('d'))
            enroques[1, 1] = true;
        else
            enroques[1, 1] = false;*/

        enroques[0, 0] = false;
        enroques[0, 1] = false;
        enroques[1, 0] = false;
        enroques[1, 1] = false;

        foreach (var c in enr)
        {
            switch (c)
            {
                case 'R':
                    enroques[0, 0] = true;
                    break;
                case 'D':
                    enroques[0, 1] = true;
                    break;
                case 'r':
                    enroques[1, 0] = true;
                    break;
                case 'd':
                    enroques[1, 1] = true;
                    break;
                default:
                    break;
            }
        }



    }

    string getEnroques()
    {
        string aux = "";

        if (enroques[0, 0])
            aux += "R";

        if (enroques[0, 1])
            aux += "D";

        if (enroques[1, 0])
            aux += "r";

        if (enroques[1, 1])
            aux += "d";

        if (!enroques[0, 0] && !enroques[0, 1] && !enroques[1, 0] && !enroques[1, 1])
            aux += "-";

        return aux;
    }

    public Posicion getPosicionFromString(string n)
    {
        char coordx = n[0];
        char coordy = n[1];

        int x;
        int y = Convert.ToInt32(new string(coordy, 1)) - 1;

        switch (coordx)
        {
            case 'a':
                x = 0;
                break;
            case 'b':
                x = 1;
                break;
            case 'c':
                x = 2;
                break;
            case 'd':
                x = 3;
                break;
            case 'e':
                x = 4;
                break;
            case 'f':
                x = 5;
                break;
            case 'g':
                x = 6;
                break;
            case 'h':
                x = 7;
                break;
            default:
                x = -1;
                break;
        }

        if (x != -1) return new Posicion(x, y);
        return null;

    }

    string getStringFromPosicion(Posicion pos)
    {

        string x;
        string y = "" + (pos.y + 1);

        switch (pos.x)
        {
            case 0:
                x = "a";
                break;
            case 1:
                x = "b";
                break;
            case 2:
                x = "c";
                break;
            case 3:
                x = "d";
                break;
            case 4:
                x = "e";
                break;
            case 5:
                x = "f";
                break;
            case 6:
                x = "g";
                break;
            case 7:
                x = "h";
                break;
            default:
                x = "";
                break;
        }

        if (x.Equals("")) return "-";
        return x + y;

    }

    void cargaTablero(string ord)
    {
        int x = 0;
        int y = 7;
        int color;
        for (int i = 0; i < ord.Length; i++)
        {
            char c = ord[i];
            if (char.IsDigit(c)) x += Convert.ToInt32(new string(c, 1));
            else
            {
                if (c == '/') { y--; x = 0; }
                else
                {
                    if (char.IsLower(c)) color = 1;
                    else color = 0;
                    tableroPiezas.CrearPieza(new string(char.ToLower(c), 1), color, new Posicion(x,y));
                    x++;
                }
            }
        }
    }

    public void CambiarColores()
    {

        Color[,] coloresDisponibles = new Color[,] { { colorBlanco, colorNegro }, { Color.white, Color.black } };

        numeroColorTablero++;

        Debug.Log(numeroColorTablero + " " + coloresDisponibles.GetLength(0) + " ");

        if (numeroColorTablero >= coloresDisponibles.GetLength(0))
            numeroColorTablero = 0;

        tableroCasillas.CambiarColor(coloresDisponibles[numeroColorTablero, 0], coloresDisponibles[numeroColorTablero, 1]);
    }

    public Posicion GetPosCasilla(GameObject cas)
    {
        return tableroCasillas.getPosicion(cas);
    }

    public Pieza GetPiezaTablero(Posicion p)
    {
        return tableroPiezas.GetPieza(p);
    }

    public void anteriorMovimiento()
    {
        var tupla = StaticVariables.getPartida().movimientosPartida.Find(x => x.Item1.Equals(FEN));
        var momento = StaticVariables.getPartida().movimientosPartida.IndexOf(tupla);
        if (momento - 1 >= 0)
        {
            eliminarPiezasAnterior(momento - 1);
        }
        
    }

    public void siguienteMovimiento()
    {
        var tupla = StaticVariables.getPartida().movimientosPartida.Find(x => x.Item1.Equals(FEN));
        var momento = StaticVariables.getPartida().movimientosPartida.IndexOf(tupla);

        if (momento + 1 < StaticVariables.getPartida().movimientosPartida.Count)
        {
            eliminarPiezasAnterior(momento + 1);
        }
            
    }

    public void RecargarAjedrez()
    {
        string[] secuencias = FEN.Split(' ');

        tableroCasillas.RecargarTablero();
        tableroPiezas.RecargarPiezas();
        CargarUI();
        cargaDesdeFEN(secuencias);
    }

    void eliminarPiezasAnterior(int momento)
    {
        tableroPiezas.RecargarPiezas();
        var tupla = StaticVariables.getPartida().movimientosPartida[momento];
        FEN = tupla.Item1;
        StaticVariables.getPartida().relojBlanco = tupla.Item2;
        StaticVariables.getPartida().relojNegro = tupla.Item3;
        var aux = FEN.Split(' ');
        cargaDesdeFEN(aux);
    }

    public string MoverPieza(Pieza pie, Posicion posCas)
    {
        Vector3 cas = tableroCasillas.tableroCasillas[posCas.x, posCas.y].GetCasilla().transform.position;
        cas = new Vector3(cas.x, cas.y, cas.z - 1);

        var aux = new Posicion(pie.posicion.x, pie.posicion.y);

        string comer = tableroPiezas.MoverPieza(pie, posCas, cas, enPassant, turno);

        casillasAColorOriginal();
        tableroCasillas.marcarCasillasMovimiento(aux, posCas);

        if (pie.nombre.Equals("p"))
        {
            checkEnPassant(aux, posCas);
        }
        else
            enPassant = null;

        if (!comer.Equals("") && piezasComidasBlancas != null && piezasComidasNegras != null)
            añadirPiezaComida(comer);
        
        siguienteJugador();
        guardarFEN();

        checkFinalPartida();
        if (tableroPiezas.piezaEnCambio == null) {
            actualizarRelojes();
        }

        return comer;

    }

    public void checkFinalPartida()
    {
        if (!quedanCasillas())
        {
            if (tableroPiezas.isReyAtacado(turno, tableroPiezas.tableroPiezas))
            {
                StaticVariables.setColorPierde(turno);
            }
            else
            {
                StaticVariables.setColorPierde(2);
            }
            StaticVariables.setFinPartida(true);
        }
        if (halfmoveClock == 50)
        {
            StaticVariables.setFinPartida(true);
            StaticVariables.setColorPierde(2);
        }
    }

    void añadirPiezaComida(string s)
    {
        GameObject go;
        
        if (turno == 1)
            go = piezasComidasBlancas;
        else
            go = piezasComidasNegras;

        go.GetComponent<PiezasComidas>().añadirPiezaComida(s);
    }

    void checkEnPassant(Posicion pie, Posicion posCas)
    {
        int dif = pie.y - posCas.y;

        if (Math.Abs(dif) == 2)
        {
            if(dif > 0)
                enPassant = new Posicion(posCas.x, pie.y - 1);
            else
                enPassant = new Posicion(posCas.x, pie.y + 1);
        }
        else
            enPassant = null;
    }

    public bool MovPos(Pieza pieza, Posicion casilla)
    {
        
        return tableroPiezas.isPosMov(pieza, casilla);
    }

    void guardarFEN()
    {
        string aux = "";
        int espacios = 0;
        
        for (int i = 7; i >= 0; i--)
        {
            for (int j = 0; j < 8; j++)
            {
                var p = tableroPiezas.GetPieza(new Posicion(j,i));
                if (p != null) {
                    char c = Convert.ToChar(p.nombre);
                    if(espacios != 0)
                    {
                        aux += espacios;
                        espacios = 0;
                    }
                    if (p.color == 0)
                        c = char.ToUpper(c);
                    aux += new string(c, 1);
                }
                else
                {
                    espacios++;
                }

            }
            if (espacios != 0) aux += espacios;
            espacios = 0;
            if(i != 0) aux += "/";

        }

        aux += " ";

        if (turno == 0) aux += "b";
        else aux += "n";

        //enroques

        aux += " ";

        aux += getEnroques();

        //En passant

        aux += " ";

        if (enPassant != null) aux += getStringFromPosicion(enPassant);
        else aux += "-";

        //Tiempos
        //Halfmove
        aux += " ";

        aux += halfmoveClock;

        //Fullmove
        aux += " ";

        aux += fullmoveClock;
        
        //Debug.Log(aux);

        if (StaticVariables.getPartida().movimientosPartida.FindIndex(x => x.Item1.Equals(FEN)) + 1 < StaticVariables.getPartida().movimientosPartida.Count)
        {
            StaticVariables.getPartida().movimientosPartida.RemoveRange(StaticVariables.getPartida().movimientosPartida.FindIndex(x => x.Item1.Equals(FEN)) + 1, StaticVariables.getPartida().movimientosPartida.Count - (StaticVariables.getPartida().movimientosPartida.FindIndex(x => x.Item1.Equals(FEN)) + 1));
        }

        FEN = aux;

        (string, int, int) tupla;
        tupla.Item1 = FEN;
        tupla.Item2 = rb.getTiempo();
        tupla.Item3 = rn.getTiempo();

        StaticVariables.getPartida().movimientosPartida.Add(tupla);
    }

    public void ResaltarCasilla(Posicion pos)
    {
        tableroCasillas.cambiarColorCasillaMov(pos);
    }

    public void QuitarResaltoCasillas()
    {
        tableroCasillas.RecargarTablero();
    }

    public void ResaltarCasDisp(Pieza p)
    {
        listaResaltados = new List<GameObject>();

        List<Posicion> casillas = tableroPiezas.PosReyNoAtacado(p);
        
        foreach (var cas in casillas)
        {
            
            GameObject go = new GameObject();
            go.AddComponent<SpriteRenderer>().color = new Color(1,1,1,.5f);
            Vector3 pos = tableroCasillas.getPosEsp(cas);
            pos.z = -1.25f;
            go.transform.position = pos;
            
            Pieza aux = tableroPiezas.GetPieza(cas);

            if (aux == null)
            {
                go.GetComponent<SpriteRenderer>().sprite = spriteMovCas;
                go.transform.localScale = new Vector3(0.4f,0.4f,1f);
            }
            else if(aux.color != p.color)
            {
                go.GetComponent<SpriteRenderer>().sprite = spriteComCas;
                go.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
            }
            else
            {
                Debug.Log("ERROR");
            }

            listaResaltados.Add(go);
            
        }
    }

    public List<GameObject> GetResaltados()
    {
        return listaResaltados;
    }

    public Vector3 GetVector3Casilla(Posicion pos)
    {
        return tableroCasillas.tableroCasillas[pos.x, pos.y].GetCasilla().transform.position;
    }

    public bool quedanCasillas()
    {
        List<Pieza> piezas = tableroPiezas.getPiezasColor(turno);
        List<Posicion> posicionesPos = new List<Posicion>();
        foreach (var p in piezas)
        {
            //Sacar todas las posiciones posibles de cada pieza con posreymov o algo asi y juntarlas y si es 0 devolver false
            posicionesPos.AddRange(tableroPiezas.PosReyNoAtacado(p));
        }

        if (posicionesPos.Count == 0)
            return false;
        return true;
    }

    public void siguienteJugador()
    {
        if (turno == 0) turno = 1;
        else
        {
            turno = 0;
            fullmoveClock++;
        }
    }

    public string textoTurno()
    {
        string t;
        if (turno == 0)
            t = "BLANCAS";
        else
            t = "NEGRAS";

        return "Turno de " + t;
    }

    public bool isPiezaEnCambio()
    {
        return tableroPiezas.piezaEnCambio != null;
    }

    public void promocionarPieza(string p)
    {
        bool result = tableroPiezas.promocionarPieza(p, turno);
        if (!result)
            anteriorMovimiento();
        else
        {
            StaticVariables.getPartida().movimientosPartida.RemoveAt(StaticVariables.getPartida().movimientosPartida.Count - 1);
            var m = StaticVariables.getPartida().movimientosPartida[StaticVariables.getPartida().movimientosPartida.Count - 1];
            FEN = m.Item1;
            StaticVariables.getPartida().relojBlanco = m.Item2;
            StaticVariables.getPartida().relojNegro = m.Item3;
            guardarFEN();
            //actualizarRelojes();
        }
    }

    public bool isEnCambio()
    {
        if (tableroPiezas.piezaEnCambio == null)
            return false;
        return true;
    }

    void actualizarRelojes()
    {
        if (turno == 0)
        {
            rb.continuarReloj();
            rn.pararReloj();
        }
        else
        {
            rn.continuarReloj();
            rb.pararReloj();
        }
    }

    public void casillasAColorOriginal()
    {
        tableroCasillas.cambiarCasillasMarcadasAColorOriginal();
    }

    public void setFEN(string f)
    {
        FEN = f;
        RecargarAjedrez();
    }

    public string getFEN()
    {
        return FEN;
    }

    public Pieza[] getPiezasIA()
    {
        return tableroPiezas.getPiezasColor(turno).ToArray();
    }

    public Pieza[,] getTablero()
    {
        return tableroPiezas.tableroPiezas;
    }

    public int getTiempoSegundosJugador()
    {
        if (StaticVariables.getColorJugador() == 0)
            return rb.getTiempo();
        else
            return rn.getTiempo();
    }

}

