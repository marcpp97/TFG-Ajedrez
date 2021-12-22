using PiezasClases;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Juego : MonoBehaviour
{

    #region variables
    public Color colorBlanco;
    public Color colorNegro;

    public Sprite spriteMovCas;
    public Sprite spriteComCas;

    //Raton
    Vector3 posicionOriginal;
    Posicion posicionPiezaSeleccionada;

    //Voz
    Speech2Text speech;
    Text2Speech text2Speech;
    int vecesFalladas;
    int faseFalladas;
    Posicion posCasillaFallados;
    Posicion posPiezaFallados;

    public Text textoMovimientos;
    public GameObject prefabCasilla;
    public Sprite[] piezasSprites;
    public Text textoTurno;

    public Pieza piezaSeleccionada;

    public Ajedrez ajedrez;

    public GameObject ventana;

    [HideInInspector]
    public GameObject gestorPhotonObject;
    [HideInInspector]
    public GestorPhoton gestorPhoton;

    public Text textoChat;
    public InputField textoInsertadoChat;

    public GameObject iconoEscuchando;
    public GameObject iconoHablando;

    JugadorIA jugadorIA;
    bool IAPensando;

    int tiempoDesdeMov;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        StaticVariables.setPartida(new Partida(SaveSystem.NombrePartida()));

        ajedrez = new Ajedrez(piezasSprites, prefabCasilla, spriteMovCas, spriteComCas, colorBlanco, colorNegro);

        StaticVariables.Reiniciar();

        crearMultijugador();

        iconoHablando.SetActive(false);
        iconoEscuchando.SetActive(false);

        if(!StaticVariables.getJuegoRaton())
        {
            speech = new Speech2Text();
            text2Speech = new Text2Speech();
            vecesFalladas = 0;
            faseFalladas = 0;
            //textoMovimientos.text = "Presiona espacio\n";
            string com = "Al jugador le ha tocado ser las piezas ";
            if (StaticVariables.getColorJugador() == 0)
                com += "blancas";
            else
                com += "negras";

            text2Speech.empezarSpeechSimple(com);
            text2Speech.empezarSpeechSimple("Presiona espacio para realizar su siguiente movimiento");
        }

        if (!StaticVariables.getMultijugador())
        {
            jugadorIA = new JugadorIA();
            jugadorIA.setFENStockfish(ajedrez.getFEN());
        }
        IAPensando = false;

        piezaSeleccionada = null;
        posicionPiezaSeleccionada = null;

        ajedrez.iniciar();
        textoTurno.text = ajedrez.textoTurno();

        ventana.SetActive(false);

        tiempoDesdeMov = ajedrez.getTiempoSegundosJugador();
    }

    // Update is called once per frame
    void Update()
    {
        ComprobarEstadoPartida();
        //ajedrez.CambiarColores(colorBlanco, colorNegro);
    }

    public void ComprobarEstadoPartida()
    {
        if (!StaticVariables.getFinPartida())
            PartidaMovimientos();
        else
        {
            AcabaPartida();
        }
    }

    public void PartidaMovimientos()
    {
        if (StaticVariables.getMultijugador())
        {
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                StaticVariables.setColorPierde(-1);
                StaticVariables.setFinPartida(true);
            }

            if (gestorPhoton.GetComponent<GestorPhoton>().conectado && gestorPhoton.GetComponent<GestorPhoton>().checkIfTwoPlayers())
            {
                if (StaticVariables.getJuegoRaton())
                {
                    ClickJugador();
                    MoverPieza();
                    DejarClickJugador();
                }
                else
                    Speech();
                    //EmpezarSpeech();
            }
        }
        else
        {
            if (StaticVariables.getColorJugador() == ajedrez.turno)
            {
                if (StaticVariables.getJuegoRaton())
                {
                    ClickJugador();
                    MoverPieza();
                    DejarClickJugador();
                }
                else
                {
                    Speech();

                    if (tiempoDesdeMov - ajedrez.getTiempoSegundosJugador() >= 10)
                    {
                        decirFraseAleatoria(1);
                        tiempoDesdeMov = ajedrez.getTiempoSegundosJugador();
                    }
                }
                //EmpezarSpeech();
            }
            else
            {
                if (!StaticVariables.getAnimacionEnCurso() && jugadorIA != null && !jugadorIA.realizandoSet && !jugadorIA.creando)
                    MoverIAStockfish();//MoverIAAleatoriamente();
            }

            if(!StaticVariables.getAnimacionEnCurso() && jugadorIA != null && !jugadorIA.creando && jugadorIA.promocion != -1)
            {
                promocionarPieza(jugadorIA.promocion);
                jugadorIA.promocion = -1;
                ajedrez.checkFinalPartida();
            }
        }

    }

    #region IA
    public void MoverIAAleatoriamente()
    {
        if (!ajedrez.isEnCambio())
        {
            var pos = jugadorIA.MoverAleatoriamente(ajedrez.getPiezasIA(), ajedrez.getTablero());

            if (pos != null)
            {
                Pieza p = ajedrez.GetPiezaTablero(pos[0]);

                if (ajedrez.MovPos(p, pos[1]))
                {
                    Mover(p, pos[1]);
                }

                if (ajedrez.isEnCambio())
                {
                    string s = jugadorIA.EleccionPromocion();
                    ajedrez.promocionarPieza(s);
                    textoTurno.text = ajedrez.textoTurno();
                }
            }
        }
    }

    void MoverIAStockfish()
    {
        if (!jugadorIA.realizandoSet && !jugadorIA.creando)
        {
            if (!IAPensando)
            {
                jugadorIA.makeBestMoveStockfish();
                IAPensando = true;
            }

            if (!jugadorIA.pensandoMov)
            {
                var pos = jugadorIA.movRealizado;

                if (pos != null)
                {
                    Pieza p = ajedrez.GetPiezaTablero(pos[0]);

                    string comer = Mover(p, pos[1]);

                    IAPensando = false;

                    if (!StaticVariables.getJuegoRaton())
                    {
                        int i;

                        if (!comer.Equals(""))
                            i = 0;
                        else
                            i = 3;

                        decirFraseAleatoria(i);

                    }
                }
            }
        }
    }

    void decirFraseAleatoria(int i)
    {

        var res = text2Speech.getFraseAleatoria(i);

        if (res != null)
        {
            //text2Speech.pararSpeechs();
            text2Speech.empezarSpeechSimple(res);
        }

    }

    #endregion

    #region mover
    string Mover(Pieza posPieza, Posicion posCasilla)
    {
        Posicion piezaAnterior = posPieza.posicion;

        string movJug;
        if (ajedrez.turno == 0)
            movJug = "BLANCAS: ";
        else
            movJug = "NEGRAS: ";

        string comido = ajedrez.MoverPieza(posPieza, posCasilla);

        textoTurno.text = ajedrez.textoTurno();

        textoMovimientos.text += movJug + piezaAnterior.GetToString() + " " + posCasilla.GetToString() + "\n";

        if(!StaticVariables.getJuegoRaton())
            text2Speech.empezarSpeechComplejo(posPieza.color, posPieza.nombre, piezaAnterior.GetToString(), posCasilla.GetToString());

        if (gestorPhoton != null)
            enviarMovimiento(piezaAnterior, posCasilla);

        return comido;

    }

    public void MoverMultijugador(Posicion posPieza, Posicion posCasilla)
    {
        Pieza pie = ajedrez.GetPiezaTablero(posPieza);

        if(pie != null)
            Mover(pie, posCasilla);

    }
    #endregion

    #region Raton
    void ClickJugador()
    {
        if (Input.GetMouseButtonDown(0) && piezaSeleccionada == null && !ajedrez.isPiezaEnCambio())
        {

            GameObject go = Jugador.DetectarClick();

            if (go != null)
            {

                posicionPiezaSeleccionada = ajedrez.GetPosCasilla(go);

                if (posicionPiezaSeleccionada != null)
                {
                    Pieza p = ajedrez.GetPiezaTablero(posicionPiezaSeleccionada);
                    if (p != null)
                    {
                        piezaSeleccionada = p;
                        posicionOriginal = piezaSeleccionada.pieza.transform.position;

                        //cambiar color casillas para ver donde puede moverse la pieza
                        ajedrez.ResaltarCasDisp(piezaSeleccionada);
                    }
                }
            }
        }
    }

    void MoverPieza()
    {
        if (piezaSeleccionada != null && Input.GetMouseButton(0))
        {
            GameObject go = piezaSeleccionada.pieza;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            go.transform.position = Vector2.Lerp(go.transform.position, mousePos, 1f);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z - 1.5f);

        }
    }

    void DejarClickJugador()
    {
        if (piezaSeleccionada != null && !Input.GetMouseButton(0) && !ajedrez.isPiezaEnCambio())
        {
            GameObject go = Jugador.DetectarClick();

            if (StaticVariables.getColorJugador() == ajedrez.turno)
            {

                if (go != null)
                {
                    var posicionCasilla = ajedrez.GetPosCasilla(go);

                    if (posicionCasilla != null)
                    {
                        if (!posicionCasilla.isEqual(piezaSeleccionada.posicion))
                        {
                            if (ajedrez.MovPos(piezaSeleccionada, posicionCasilla))
                            {
                                if (piezaSeleccionada.color == ajedrez.turno)
                                {
                                    if (!StaticVariables.getAnimacionEnCurso())
                                    {
                                        Mover(piezaSeleccionada, posicionCasilla);

                                        if (!StaticVariables.getMultijugador())
                                            jugadorIA.setFENStockfish(ajedrez.getFEN());//jugadorIA.setMoveOponente(ajedrez.getFEN());
                                    }
                                }
                                else
                                {
                                    piezaSeleccionada.pieza.transform.position = posicionOriginal;
                                }
                            }
                            else
                            {
                                piezaSeleccionada.pieza.transform.position = posicionOriginal;
                                //Debug.Log("No es posible el movimiento 1");
                                textoMovimientos.text += "No es posible el movimiento\n";
                            }
                        }
                        else
                        {
                            piezaSeleccionada.pieza.transform.position = posicionOriginal;
                        }
                    }
                    else
                    {
                        piezaSeleccionada.pieza.transform.position = posicionOriginal;
                        //Debug.Log("No es posible el movimiento 2");
                        textoMovimientos.text += "No es posible el movimiento\n";
                    }
                }
                else
                {
                    piezaSeleccionada.pieza.transform.position = posicionOriginal;
                    //Debug.Log("No es posible el movimiento 3");
                    textoMovimientos.text += "No es posible el movimiento\n";
                }
            }
            else
            {
                piezaSeleccionada.pieza.transform.position = posicionOriginal;
            }

            var lista = ajedrez.GetResaltados();
            foreach (var r in lista)
            {
                Destroy(r);
            }

            piezaSeleccionada = null;
            posicionPiezaSeleccionada = null;
            posicionOriginal = Vector3.zero;
        }

    }
    #endregion

    #region Speech

    void Speech()
    {
        getSpeech();
        EmpezarSpeech();
    }

    void getSpeech()
    {
        if (Input.GetKeyDown(KeyCode.Space) && piezaSeleccionada == null && !StaticVariables.getSpeech2TextEnCurso())
        {
            iconoHablando.SetActive(true);
            StaticVariables.setSpeech2TextEnCurso(true);
            text2Speech.pararSpeechs();

            StartCoroutine(FadeImage(1f, iconoHablando.GetComponent<Image>()));

            speech.Record(); //Hacerlo Threat

        }
    }

    void EmpezarSpeech()
    {
        if (StaticVariables.getSpeech2TextEnCurso() && !speech.estaEscuchando)
        {

            text2Speech.pararSpeechs();

            string[] secuencia = speech.resultado;
            
            if (secuencia == null || secuencia[0].Equals("error"))
            {
                text2Speech.empezarSpeechSimple("No te he entendido");
                StaticVariables.setSpeech2TextEnCurso(false);
                iconoHablando.SetActive(false);
                if (!ajedrez.isPiezaEnCambio())
                {
                    vecesFalladas++;
                    if (vecesFalladas >= 2)
                    {
                        if (faseFalladas == 0)
                        {
                            text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");
                            ajedrez.QuitarResaltoCasillas();
                        }
                        if (faseFalladas == 1)
                            text2Speech.empezarSpeechSimple("Por favor, diga la casilla a la que quiere mover la pieza");
                    }
                }
                else
                {
                    text2Speech.empezarSpeechSimple("Por favor, diga la pieza a la que desea promocionar");
                }
                return;
            }

            if (secuencia[0].Equals("Comando"))
            {
                
                switch (secuencia[1])
                {
                    case "Abandonar":

                        Abandonar();
                        break;

                    case "Tablero":

                        text2Speech.empezarSpeechSimple("Se ha cambiado el color del tablero");

                        ajedrez.CambiarColores();

                        break;

                    case "Tiempo":

                        int tiempo;
                        if (StaticVariables.getColorJugador() == 0)
                            tiempo = ajedrez.rb.getTiempo();
                        else
                            tiempo = ajedrez.rn.getTiempo();

                        int minutos = tiempo / 60;
                        int segundos = tiempo % 60;

                        if(segundos == 0)
                            text2Speech.empezarSpeechSimple("Le quedan " + minutos + " minutos"); 
                        else
                            text2Speech.empezarSpeechSimple("Le quedan " + minutos + " minutos y " + segundos + " segundos");

                        break;

                    case "Fallados":
                        
                        if(faseFalladas == 2)
                        {
                            
                            ajedrez.QuitarResaltoCasillas();

                            if (secuencia[2].Equals("si"))
                            {
                                Pieza p = ajedrez.GetPiezaTablero(posPiezaFallados);
                                piezaSeleccionada = p;

                                if (ajedrez.MovPos(p, posCasillaFallados) && p.color == ajedrez.turno)
                                {

                                    if (!StaticVariables.getAnimacionEnCurso())
                                        Mover(p, posCasillaFallados);
                                    if (!StaticVariables.getMultijugador())
                                        jugadorIA.setFENStockfish(ajedrez.getFEN());

                                    decirFraseAleatoria(2);

                                    tiempoDesdeMov = ajedrez.getTiempoSegundosJugador();

                                    vecesFalladas = 0;
                                }
                                else
                                {
                                    text2Speech.empezarSpeechSimple("No es posible el movimiento");
                                    text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");

                                    vecesFalladas++;
                                }

                                faseFalladas = 0;

                            }
                            else
                            {

                                vecesFalladas++;
                                faseFalladas = 0;

                                text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");

                            }

                            posCasillaFallados = null;
                            posPiezaFallados = null;

                        }
                        else
                        {
                            text2Speech.empezarSpeechSimple("Formato incorrecto");
                        }

                        break;

                    case "Tiempo_extra":

                        if(!StaticVariables.getMultijugador())
                        {

                            Reloj rj;

                            if (StaticVariables.getColorJugador() == 0)
                                rj = ajedrez.rb;
                            else
                                rj = ajedrez.rn;

                            rj.darTiempo(int.Parse(secuencia[2]));

                            text2Speech.empezarSpeechSimple("Se han añadido " + secuencia[2] + " minutos al reloj");

                        }
                        else
                        {
                            text2Speech.empezarSpeechSimple("No se puede realizar en una partida multijugador");
                        }

                        break;

                    default:
                        break;
                }

                StaticVariables.setSpeech2TextEnCurso(false);
                iconoHablando.SetActive(false);
                piezaSeleccionada = null;

            }

            if (secuencia[0].Equals("Orden"))
            {
                if (!ajedrez.isPiezaEnCambio())
                {
                    if (vecesFalladas < 2)
                    {
                        var ordenesx = secuencia[2].Split(' ');
                        var ordenesy = secuencia[3].Split(' ');

                        if ((!secuencia[2].Equals("") || !secuencia[3].Equals("")) && ordenesx.Length == 2 && ordenesy.Length == 2)
                        {
                            //Transformar orden a Posicion , Posicion

                            Posicion posPieza = PosicionFromString(ordenesx[0], ordenesy[0]);
                            Posicion posCasilla = PosicionFromString(ordenesx[1], ordenesy[1]);

                            if (posCasilla != null && posPieza != null)
                            {
                                Pieza p = ajedrez.GetPiezaTablero(posPieza);
                                if (p != null)
                                {
                                    piezaSeleccionada = p;

                                    if (ajedrez.MovPos(p, posCasilla) && p.color == ajedrez.turno)
                                    {
                                        if (!StaticVariables.getAnimacionEnCurso())
                                            Mover(p, posCasilla);
                                        if (!StaticVariables.getMultijugador())
                                            jugadorIA.setFENStockfish(ajedrez.getFEN());

                                        decirFraseAleatoria(2);

                                        tiempoDesdeMov = ajedrez.getTiempoSegundosJugador();

                                        vecesFalladas = 0;
                                        
                                    }
                                    else
                                    {
                                        text2Speech.empezarSpeechSimple("No es posible el movimiento");
                                    }
                                }
                                else
                                {
                                    text2Speech.empezarSpeechSimple("No hay pieza donde ha seleccionado");
                                }

                            }
                            else
                            {
                                text2Speech.empezarSpeechSimple("Casilla seleccionada incorrecta");
                            }

                        }
                        else
                        {
                            text2Speech.empezarSpeechSimple("Formato incorrecto");
                        }
                    }
                    else
                    {
                        if (faseFalladas != 2)
                        {
                            var x = secuencia[2];
                            var y = secuencia[3];

                            if (!secuencia[2].Equals("") || !secuencia[3].Equals(""))
                            {
                                Posicion posPieza = PosicionFromString(x, y);

                                if (posPieza != null)
                                {
                                    if (faseFalladas == 0)
                                    {
                                        Pieza p = ajedrez.GetPiezaTablero(posPieza);
                                        if (p != null)
                                        {
                                            posPiezaFallados = posPieza;
                                            faseFalladas++;
                                            ajedrez.ResaltarCasilla(posPieza);
                                            text2Speech.empezarSpeechSimple("Por favor, diga la casilla a la que quiere mover la pieza");
                                        }
                                        else
                                        {
                                            text2Speech.empezarSpeechSimple("No hay pieza donde ha seleccionado");

                                            if (faseFalladas == 0)
                                                text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");
                                            if (faseFalladas == 1)
                                                text2Speech.empezarSpeechSimple("Por favor, diga la casilla a la que quiere mover la pieza");

                                        }
                                    }
                                    else
                                    {
                                        if (faseFalladas == 1)
                                        {
                                            posCasillaFallados = posPieza;
                                            faseFalladas++;
                                            ajedrez.ResaltarCasilla(posPieza);
                                            text2Speech.empezarSpeechSimple("¿Quiere realizar este movimiento?");
                                        }

                                    }
                                }
                                else
                                {
                                    text2Speech.empezarSpeechSimple("Casilla seleccionada incorrecta");

                                    if (faseFalladas == 0)
                                        text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");
                                    if (faseFalladas == 1)
                                        text2Speech.empezarSpeechSimple("Por favor, diga la casilla a la que quiere mover la pieza");

                                }
                            }
                            else
                            {
                                text2Speech.empezarSpeechSimple("Formato incorrecto");

                                if (faseFalladas == 0)
                                    text2Speech.empezarSpeechSimple("Por favor, diga la casilla de la pieza que desea mover");
                                if (faseFalladas == 1)
                                    text2Speech.empezarSpeechSimple("Por favor, diga la casilla a la que quiere mover la pieza");

                            }

                        }
                        else
                        {
                            text2Speech.empezarSpeechSimple("Formato incorrecto");
                            text2Speech.empezarSpeechSimple("¿Quiere realizar este movimiento?");
                        }

                    }
                }
                else
                {
                    text2Speech.empezarSpeechSimple("Formato incorrecto");
                    text2Speech.empezarSpeechSimple("Por favor, diga la pieza a la que desea promocionar");
                }

                StaticVariables.setSpeech2TextEnCurso(false);
                iconoHablando.SetActive(false);
                piezaSeleccionada = null;

            }

            if (secuencia[0].Equals("PiezasPromocion"))
            {
                if(ajedrez.isPiezaEnCambio())
                {
                    string p;

                    switch (secuencia[1])
                    {
                        case "Dama":
                            p = "d";
                            break;
                        case "Torre":
                            p = "t";
                            break;
                        case "Caballo":
                            p = "c";
                            break;
                        case "Alfil":
                            p = "a";
                            break;
                        case "Atras":
                            p = "";
                            break;
                        default:
                            p = "";
                            break;
                    }

                    StaticVariables.setSpeech2TextEnCurso(false);
                    iconoHablando.SetActive(false);
                    piezaSeleccionada = null;

                    ajedrez.promocionarPieza(p);
                    if (StaticVariables.getMultijugador())
                        gestorPhoton.GetComponent<GestorPhoton>().enviarPromocion(p);
                    textoTurno.text = ajedrez.textoTurno();

                }
            }
            
        }
    }
    #endregion

    Posicion PosicionFromString(string x, string y)
    {
        int resx;
        int resy;

        if (int.TryParse(y, out int aux)) resy = aux - 1;
        else
            return null;

        if (!char.IsLetter(x[0])) return null;

        switch (x.ToLower())
        {
            case "a":
                resx = 0;
                break;
            case "b":
                resx = 1;
                break;
            case "c":
                resx = 2;
                break;
            case "d":
                resx = 3;
                break;
            case "e":
                resx = 4;
                break;
            case "f":
                resx = 5;
                break;
            case "g":
                resx = 6;
                break;
            case "h":
                resx = 7;
                break;
            default:
                return null;
        }
        return new Posicion(resx, resy);
    }

    public void cambiarEstadoUI(bool b)
    {
        GameObject.Find("Abandonar").GetComponent<Button>().interactable = b;

    }

    public Ajedrez getAjedrez()
    {
        return ajedrez;
    }

    public void sceneLoader(string scene)
    {
        if(!StaticVariables.getJuegoRaton())
            text2Speech.pararSpeechs();

        if(gestorPhoton != null)
        {
            gestorPhoton.GetComponent<GestorPhoton>().Desconectar(scene);
        }
        SceneManager.LoadScene(scene);
    }

    public void guardarPartida(Button boton)
    {
        Text textoGuardar = GameObject.Find("TextoPartidaGuardada").GetComponent<Text>();
        try
        {
            SaveSystem.GuardarPartida();
        }
        catch(Exception)
        {
            textoGuardar.text = "Ha ocurrido un error";
            StartCoroutine(FadeTextToZeroAlpha(1f, textoGuardar));
            return;
        }
        textoGuardar.text = "Partida guardada con éxito";
        StartCoroutine(FadeTextToZeroAlpha(1f, textoGuardar));
        boton.interactable = false;
    }

    public void promocionarPieza(int i)
    {
        if (StaticVariables.getJuegoRaton())
        {
            string p;
            switch (i)
            {
                case 0:
                    p = "d";
                    break;
                case 1:
                    p = "c";
                    break;
                case 2:
                    p = "t";
                    break;
                case 3:
                    p = "a";
                    break;
                default:
                    p = "";
                    break;
            }
            ajedrez.promocionarPieza(p);
            if (StaticVariables.getMultijugador())
                gestorPhoton.GetComponent<GestorPhoton>().enviarPromocion(p);
            textoTurno.text = ajedrez.textoTurno();
        }
    }

    void enviarMovimiento(Posicion pieza, Posicion casilla)
    {
        gestorPhoton.GetComponent<GestorPhoton>().enviarPosicionMovimiento(pieza, casilla);
    }

    public void promocionarPiezaMultijugador(string s)
    {
        ajedrez.promocionarPieza(s);
    }

    void crearMultijugador()
    {
        if (StaticVariables.getMultijugador())
        {
            gestorPhotonObject = new GameObject();
            gestorPhotonObject.name = "GestorPhoton";
            gestorPhoton = gestorPhotonObject.AddComponent<GestorPhoton>();
            gestorPhoton.iniciar(this, textoMovimientos);

            cambiarEstadoUI(false);
        }
        else
        {
            gestorPhoton = null;
            GameObject.Find("Chat").SetActive(false);
        }
    }

    public void recargarTablero()
    {
        ajedrez.RecargarAjedrez();
    }

    public void enviarMensajeChat()
    {
        if(!textoInsertadoChat.text.Equals("") && gestorPhoton.GetComponent<GestorPhoton>().checkIfTwoPlayers())
        {
            string m;
            if (StaticVariables.getColorJugador() == 0)
                m = "BLANCO: ";
            else
                m = "NEGRO: ";

            m += textoInsertadoChat.text + "\n";

            gestorPhoton.GetComponent<GestorPhoton>().enviarMensaje(m);

            textoChat.text += m;
            textoInsertadoChat.Select();
        }
        textoInsertadoChat.text = "";
        GameObject.Find("ChatInput").GetComponent<InputField>().ActivateInputField();
    }

    public void setMensajeChat(string s)
    {
        textoChat.text += s;
    }

    public void AcabaPartida()
    {
        if (!ventana.activeSelf)
        {
            ventana.SetActive(true);
            Text texto = GameObject.Find("TextoGanador").GetComponent<Text>();

            int color = StaticVariables.getColorPierde();

            if (color == 0)
                texto.text = "GANAN NEGRAS";
            if (color == 1)
                texto.text = "GANAN BLANCAS";
            if (color == 2)
                texto.text = "EMPATE";
            if (color == -1)
            {
                texto.text = "SIN CONEXIÓN";
                GameObject.Find("GuardarPartida").GetComponent<Button>().interactable = false;
            }
            if(color == -2)
            {
                texto.text = "ERROR";
                GameObject.Find("GuardarPartida").GetComponent<Button>().interactable = false;
            }

            GameObject.Find("RelojBlanco").GetComponent<Reloj>().pararReloj();
            GameObject.Find("RelojNegro").GetComponent<Reloj>().pararReloj();

            AcabarPartidaMultijugador();

            if (!StaticVariables.getJuegoRaton())
            {
                if (color == StaticVariables.getColorJugador())
                    text2Speech.empezarSpeechSimple("Has perdido");
                else
                {
                    if (color == 2)
                        text2Speech.empezarSpeechSimple("Has empatado");
                    else
                    {
                        if(color == -1)
                            text2Speech.empezarSpeechSimple("No hay conexión");
                        else
                        {
                            if (color == -2)
                                text2Speech.empezarSpeechSimple("Error con la aplicación de Stockfish");
                            else
                                text2Speech.empezarSpeechSimple("Has ganado");
                        }
                    }
                }
            }
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeImage(float t, Image i)
    {
        while (StaticVariables.getSpeech2TextEnCurso())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                yield return null;
            }

            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
    }

    public void Atras()
    {
        ajedrez.anteriorMovimiento();
        textoTurno.text = ajedrez.textoTurno();
    }

    public void Adelante()
    {
        ajedrez.siguienteMovimiento();
        textoTurno.text = ajedrez.textoTurno();
    }

    public void Abandonar()
    {
        if (!StaticVariables.getFinPartida())
        {
            if (!StaticVariables.getJuegoRaton())
                text2Speech.pararSpeechs();

            StaticVariables.setColorPierde(StaticVariables.getColorJugador());
            StaticVariables.setFinPartida(true);

            if (StaticVariables.getColorPierde() == 0)
            {
                textoMovimientos.text += "BLANCAS se rinden\n";
                if (!StaticVariables.getJuegoRaton())
                    text2Speech.empezarSpeechSimple("Blancas se rinden");
            }
            else
            {
                textoMovimientos.text += "NEGRAS se rinden\n";
                if (!StaticVariables.getJuegoRaton())
                    text2Speech.empezarSpeechSimple("Negras se rinden");
            }
        }
    }

    public void AcabarPartidaMultijugador()
    {
        if (StaticVariables.getMultijugador())
        {
            gestorPhoton.GetComponent<GestorPhoton>().enviarAcabarPartida();
        }
    }

    private void OnApplicationQuit()
    {
        if (!StaticVariables.getJuegoRaton())
            text2Speech.pararSpeechs();
    }

}
