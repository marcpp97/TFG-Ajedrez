using PiezasClases;
using System;
using System.IO;
using UnityEngine;
using Stockfish.NET;
using Stockfish.NET.Models;
using System.Threading;
using UnityEngine.UI;

public class JugadorIA
{

    #region privados
    JugadorIA jIA;
    IStockfish stockfish;
    #endregion

    #region publicos
    public bool pensandoMov;
    public bool realizandoSet;
    public bool creando;

    public Posicion[] movRealizado;

    public int promocion;
    #endregion

    #region Constructor
    public JugadorIA()
    {
        if (jIA == null)
        {
            jIA = this;
            creando = true;
            CrearStockfish();
            pensandoMov = false;
            realizandoSet = false;
            movRealizado = null;
            promocion = -1;
        }
    }
    #endregion

    #region moverAleatorio
    public Posicion[] MoverAleatoriamente(Pieza[] listaPiezas, Pieza[,] tablero)
    {
        int i = UnityEngine.Random.Range(0, listaPiezas.Length - 1);
        var l = listaPiezas[i].getPosDis(tablero).ToArray();
        if (l.Length != 0)
        {
            int j = UnityEngine.Random.Range(0, l.Length - 1);
            return new Posicion[] { listaPiezas[i].posicion, l[j] };
        }
        return null;
    }

    public string EleccionPromocion()
    {
        return "d";
    }
    #endregion

    #region Stockfish
    void CrearStockfish()
    {
        stockfish = null;
        Settings settings = new Settings(0, 0, false, 1, StaticVariables.getDificultadIA(), 30, 100, false);
        Thread trd = new Thread(() => ThreadCrearStockfish(settings));
        trd.IsBackground = true;
        trd.Start();
    }

    void ThreadCrearStockfish(Settings settings)
    {
        try
        {
            stockfish = new Stockfish.NET.Core.Stockfish(GetStockfishDir(), 4, settings);
            if (stockfish == null)
                throw new Exception("Error al crear el stockfish");
            creando = false;
        }catch(Exception)
        {
            Juego juego = GameObject.Find("Juego").GetComponent<Juego>();

            juego.textoChat.text += "No se encuentra la aplicación de Stockfish\n";

            StaticVariables.setFinPartida(true);
            StaticVariables.setColorPierde(-2);

        }
    }

    public void makeBestMoveStockfish()
    {
        movRealizado = null;
        promocion = -1;
        Thread trd = new Thread(ThreadMovimiento);
        trd.IsBackground = true;
        trd.Start();
    }

    void ThreadMovimiento()
    {
        while(creando || realizandoSet) { }

        pensandoMov = true;
        //var bestMove = stockfish.GetBestMove();
        var bestMove = stockfish.GetBestMoveTime();

        Posicion posOrigen = getPosFromString(bestMove[0], bestMove[1]);
        Posicion posDestino = getPosFromString(bestMove[2], bestMove[3]);

        if(bestMove.Length == 5)
        {
            char c = char.ToLower(bestMove[4]);
            switch (c)
            {
                case 'q':
                    promocion = 0;
                    break;
                case 'n':
                    promocion = 1;
                    break;
                case 'r':
                    promocion = 2;
                    break;
                case 'b':
                    promocion = 3;
                    break;
                default:
                    break;
            }
        }

        movRealizado = new Posicion[] { posOrigen, posDestino };
        pensandoMov = false;
    }

    public void setFENStockfish(string FEN)
    {
        Thread trd = new Thread(() => ThreadSetFEN(ConverFENMaloEnFENBueno(FEN)));
        trd.IsBackground = true;
        trd.Start();

    }

    void ThreadSetFEN(string s)
    {
        while(creando || realizandoSet) { }

        realizandoSet = true;
        stockfish.SetFenPosition(s);
        realizandoSet = false;
    }

    static string GetStockfishDir()
    {

        var dir = Application.streamingAssetsPath;
        string so;

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                so = "win\\stockfish_13_win_x64.exe";
                break;
            case RuntimePlatform.LinuxPlayer:
                so = "lin\\stockfish_13_linux_x64";
                break;
            case RuntimePlatform.OSXPlayer:
                so = "OS\\stockfish";
                break;
            case RuntimePlatform.Android:
                so = "and\\stockfish.android.armv7";
                break;
            case RuntimePlatform.IPhonePlayer:
                so = "OS\\stockfish";
                break;
            default:
                so = "win\\stockfish_13_win_x64.exe";
                break;
        }

        var path = $@"{dir}\Stockfish.Program\{so}";
        
        return path;
    }

    #endregion

    #region Complementos
    Posicion getPosFromString(char cx, char cy)
    {
        int x;
        int y = Convert.ToInt32(new string(cy, 1)) - 1;
        switch (cx)
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
                throw new Exception();
        }
        return new Posicion(x,y);
    }

    public string ConverFENMaloEnFENBueno(string FENMalo)
    {
        // tcadract / pppppppp / 8 / 8 / 8 / 8 / PPPPPPPP / TCADRACT b RDrd -0 1
        string FENBueno = "";

        var aux = FENMalo.Split(' ');

        foreach (var c in aux[0])
        {
            if (char.IsLetter(c))
            {
                char clower = char.ToLower(c);
                char auxc;
                switch (clower)
                {
                    case 't':
                        auxc = 'r';
                        break;
                    case 'c':
                        auxc = 'n';
                        break;
                    case 'a':
                        auxc = 'b';
                        break;
                    case 'd':
                        auxc = 'q';
                        break;
                    case 'r':
                        auxc = 'k';
                        break;
                    case 'p':
                        auxc = 'p';
                        break;
                    default:
                        throw new Exception();
                }
                if (char.IsUpper(c))
                    FENBueno += char.ToUpper(auxc);
                else
                    FENBueno += auxc;
            }
            else
                FENBueno += c;
        }

        FENBueno += " ";

        if (aux[1].Equals("b"))
            FENBueno += "w";
        else
            FENBueno += "b";

        FENBueno += " ";

        foreach (var c in aux[2])
        {
            var auxlower = char.ToLower(c);
            char auxc;
            switch (auxlower)
            {
                case 'r':
                    auxc = 'k';
                    break;
                case 'd':
                    auxc = 'q';
                    break;
                case '-':
                    auxc = '-';
                    break;
                default:
                    throw new Exception();
            }
            if (char.IsLetter(c) && char.IsUpper(c))
                FENBueno += char.ToUpper(auxc);
            else
                FENBueno += auxc;
        }

        FENBueno += " ";

        FENBueno += aux[3] + " " + aux[4] + " " + aux[5];

        return FENBueno;

    }

    #endregion

}
