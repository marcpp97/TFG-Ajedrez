using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class GestorPhoton : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField]
    private byte maxPlayers = 2;

    const byte movPos = 1;
    const byte mensajeText = 2;
    const byte mensajeTeam = 3;
    const byte mensajeAcabar = 4;
    const byte mensajePromocion = 5;

    public bool conectado;
    bool jugadoresConectados;

    Juego juego;

    Text textoMovimientos;

    bool finPartida;

    private void Update()
    {
        if(conectado && jugadoresConectados && PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            if(StaticVariables.getColorJugador() == 0)
                StaticVariables.setColorPierde(1);
            else
                StaticVariables.setColorPierde(0);
            StaticVariables.setFinPartida(true);
        }

    }

    public void iniciar(Juego j, Text t)
    {
        juego = j;
        textoMovimientos = t;

        conectado = false;
        finPartida = false;
        jugadoresConectados = false;

        PhotonNetwork.ConnectUsingSettings();
        textoMovimientos.text += "Conectando...\n";
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    void CreateRoom()
    {
        int randoomNumber = UnityEngine.Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom("Sala " + randoomNumber, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        StaticVariables.setColorJugador(teamAleatorio());

        PhotonNetwork.RaiseEvent(mensajeTeam, StaticVariables.getColorJugador(), raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnJoinedRoom()
    {
        textoMovimientos.text += "Se ha conectado\n";

        if(PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            textoMovimientos.text += "Esperando jugadores\n";
            Thread trd = new Thread(esperandoJugadorThread);
            trd.IsBackground = true;
            trd.Start();
        }
        else
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            jugadoresConectados = true;
            textoMovimientos.text += "Jugadores conectados\n";
        }

        if (!StaticVariables.getJuegoRaton())
            textoMovimientos.text += "Presiona espacio\n";

        GameObject.Find("NombreSala").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;

        conectado = true;

        juego.recargarTablero();

        juego.cambiarEstadoUI(true);
    }

    void esperandoJugadorThread()
    {
        bool esperando = true;

        while(esperando)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                esperando = false;
                textoMovimientos.text += "Jugadores conectados\n";
            }
        }

    }

    int teamAleatorio()
    {
        int r = UnityEngine.Random.Range(0, 99);

        if (r < 50)
            return 0;
        return 1;
    }

    public bool enviarPosicionMovimiento(Posicion posPieza, Posicion posCasilla)
    {
        int[] pos = { posPieza.x, posPieza.y, posCasilla.x, posCasilla.y };
        return PhotonNetwork.RaiseEvent(movPos, pos, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    public bool enviarMensaje(string m)
    {
        return PhotonNetwork.RaiseEvent(mensajeText, m, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    public bool enviarAcabarPartida()
    {
        if (!finPartida)
        {
            finPartida = true;
            if(StaticVariables.getColorPierde() != -1)
                return PhotonNetwork.RaiseEvent(mensajeAcabar, StaticVariables.getColorPierde(), RaiseEventOptions.Default, SendOptions.SendReliable);
        }
        return false;
    }

    public bool enviarPromocion(string s)
    {
        return PhotonNetwork.RaiseEvent(mensajePromocion, s, RaiseEventOptions.Default, SendOptions.SendReliable);
    }
    
    public void OnEvent(EventData photonEvent)
    {
        byte code = photonEvent.Code;
        
        switch (code)
        {
            case movPos:
                int[] posiciones = (int[])photonEvent.CustomData;
                Posicion posPieza = new Posicion(posiciones[0], posiciones[1]);
                Posicion posCasilla = new Posicion(posiciones[2], posiciones[3]);
                juego.MoverMultijugador(posPieza, posCasilla);
                break;

            case mensajeText:
                string mens = (string)photonEvent.CustomData;
                juego.setMensajeChat(mens);
                break;

            case mensajeTeam:
                int t = (int)photonEvent.CustomData;
                if (t == 0)
                    StaticVariables.setColorJugador(1);
                else
                    StaticVariables.setColorJugador(0);

                juego.recargarTablero();

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                raiseEventOptions.Receivers = ReceiverGroup.All;
                raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
                PhotonNetwork.RaiseEvent(mensajeTeam, StaticVariables.getColorJugador(), raiseEventOptions, SendOptions.SendReliable);
                break;

            case mensajeAcabar:
                int n = (int)photonEvent.CustomData;

                if (n != StaticVariables.getColorPierde())
                {
                    StaticVariables.setFinPartida(true);

                    StaticVariables.setColorPierde(n);

                    if (StaticVariables.getColorPierde() == 0)
                        textoMovimientos.text += "BLANCAS se rinden";
                    else
                        textoMovimientos.text += "NEGRAS se rinden";
                }
                break;

            case mensajePromocion:
                string s = (string)photonEvent.CustomData;
                juego.promocionarPiezaMultijugador(s);
                break;
            default:
                break;
        }

    }

    public bool checkIfTwoPlayers()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == 2;
    }

    public void Desconectar(string l)
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        StartCoroutine(DesconectaryAcabar(l));
    }

    IEnumerator DesconectaryAcabar(string l)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(l);
    }

}
