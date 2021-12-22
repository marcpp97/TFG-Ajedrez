using PiezasClases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VerPartida : MonoBehaviour
{
    public Color colorBlanco;
    public Color colorNegro;

    public Sprite spriteMovCas;
    public Sprite spriteComCas;

    //Raton
    Vector3 posicionOriginal;
    Posicion posicionPiezaSeleccionada;

    public GameObject prefabCasilla;
    public Sprite[] piezasSprites;
    public Text textoTurno;

    public Pieza piezaSeleccionada;

    public Ajedrez ajedrez;

    public Text textoGanador;

    List<(string, int, int)> listaFENoriginal;
    Tuple<string, int, int> ultimoOriginal;

    bool movimientosDiferentes;

    JugadorIA jugadorIA;

    bool IAPensando;

    // Start is called before the first frame update
    void Start()
    {
        ajedrez = new Ajedrez(piezasSprites, prefabCasilla, spriteMovCas, spriteComCas, colorBlanco, colorNegro);

        jugadorIA = new JugadorIA();
        jugadorIA.setFENStockfish(ajedrez.getFEN());
        IAPensando = false;

        piezaSeleccionada = null;
        posicionPiezaSeleccionada = null;

        ajedrez.iniciar();
        textoTurno.text = ajedrez.textoTurno();

        textoGanadorEscribir();

        listaFENoriginal = new List<(string, int, int)>(StaticVariables.getPartida().movimientosPartida);

        ultimoOriginal = null;

        movimientosDiferentes = false;

    }

    // Update is called once per frame
    void Update()
    {
        PartidaMovimientos();
    }

    public void PartidaMovimientos()
    {
        if (StaticVariables.getColorJugador() == ajedrez.turno)
        {
            ClickJugador();
            MoverPieza();
            DejarClickJugador();
        }
        else
        {
            if(movimientosDiferentes && !StaticVariables.getAnimacionEnCurso() && !jugadorIA.realizandoSet)
                MoverIA();
        }
        if (!StaticVariables.getAnimacionEnCurso() && jugadorIA.promocion != -1)
        {
            promocionarPieza(jugadorIA.promocion);
            jugadorIA.promocion = -1;
            ajedrez.checkFinalPartida();
        }
    }

    void Mover(Pieza posPieza, Posicion posCasilla)
    {

        if (ultimoOriginal == null)
        {
            ultimoOriginal = StaticVariables.getPartida().movimientosPartida.Find(x => x.Item1.Equals(ajedrez.getFEN())).ToTuple();
            int n = StaticVariables.getPartida().movimientosPartida.IndexOf(ultimoOriginal.ToValueTuple());
            StaticVariables.getPartida().movimientosPartida.RemoveRange(n + 1, StaticVariables.getPartida().movimientosPartida.Count - n - 1);
            
            movimientosDiferentes = true;
        }

        Posicion piezaAnterior = posPieza.posicion;

        ajedrez.MoverPieza(posPieza, posCasilla);

        textoTurno.text = ajedrez.textoTurno();

    }

    public void MoverIA()
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

                Mover(p, pos[1]);

                IAPensando = false;
            }
        }
    }

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
                            if (ajedrez.MovPos(piezaSeleccionada, posicionCasilla) && piezaSeleccionada.color == ajedrez.turno)
                            {
                                if (!StaticVariables.getAnimacionEnCurso())
                                {
                                    Mover(piezaSeleccionada, posicionCasilla);

                                    jugadorIA.setFENStockfish(ajedrez.getFEN());
                                }
                            }
                            else
                            {
                                piezaSeleccionada.pieza.transform.position = posicionOriginal;
                            }
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

    public Ajedrez getAjedrez()
    {
        return ajedrez;
    }

    public void promocionarPieza(int i)
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
        textoTurno.text = ajedrez.textoTurno();
    }

    void textoGanadorEscribir()
    {
        ajedrez.setFEN(StaticVariables.getPartida().movimientosPartida[StaticVariables.getPartida().movimientosPartida.Count - 1].Item1);
        if(!ajedrez.quedanCasillas())
        {
            if (StaticVariables.getColorPierde() == 0)
                textoGanador.text = "NEGRAS ganan por jaque mate";
            else
                textoGanador.text = "BLANCAS ganan por jaque mate";
        }
        else
        {
            if (StaticVariables.getPartida().relojBlanco <= 0 || StaticVariables.getPartida().relojNegro <= 0)
            {
                if(StaticVariables.getPartida().relojBlanco <= 0)
                    textoGanador.text = "NEGRAS ganan por tiempo agotado";
                if (StaticVariables.getPartida().relojNegro <= 0)
                    textoGanador.text = "BLANCAS ganan por tiempo agotado";
            }
            else
            {
                if (StaticVariables.getColorPierde() == 0)
                    textoGanador.text = "NEGRAS ganan por abandono";
                if (StaticVariables.getColorPierde() == 1)
                    textoGanador.text = "BLANCAS ganan por abandono";
                if (StaticVariables.getColorPierde() == 2)
                    textoGanador.text = "EMPATE";
            }
        }
        ajedrez.setFEN(StaticVariables.getPartida().movimientosPartida[0].Item1);
    }

    public void Atras()
    {
        if(ultimoOriginal != null && ultimoOriginal.Equals(StaticVariables.getPartida().movimientosPartida.Find(x => x.Item1.Equals(ajedrez.getFEN())).ToTuple()))
        {
            ultimoOriginal = null;
            StaticVariables.getPartida().movimientosPartida = new List<(string, int, int)>(listaFENoriginal);
        }
        movimientosDiferentes = false;

        ajedrez.casillasAColorOriginal();

        ajedrez.anteriorMovimiento();
        textoTurno.text = ajedrez.textoTurno();
    }

    public void Adelante()
    {
        ajedrez.casillasAColorOriginal();
        ajedrez.siguienteMovimiento();
        textoTurno.text = ajedrez.textoTurno();
    }

    public void Salir()
    {
        SceneManager.LoadScene("Menu");
    }
}
