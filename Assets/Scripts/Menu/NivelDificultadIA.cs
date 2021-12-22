using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NivelDificultadIA : MonoBehaviour
{

    public Dropdown dificultades;

    private void Start()
    {
        int dificultad;

        switch (StaticVariables.getDificultadIA())
        {
            case 0:
                dificultad = 0;
                break;
            case 5:
                dificultad = 1;
                break;
            case 10:
                dificultad = 2;
                break;
            case 15:
                dificultad = 3;
                break;
            case 20:
                dificultad = 4;
                break;
            default:
                dificultad = 2;
                break;
        }

        dificultades.value = dificultad;
    }

    public void establecerDificultad(int indice)
    {
        int dificultad;

        switch (indice)
        {
            case 0:
                dificultad = 0;
                break;
            case 1:
                dificultad = 5;
                break;
            case 2:
                dificultad = 10;
                break;
            case 3:
                dificultad = 15;
                break;
            case 4:
                dificultad = 20;
                break;
            default:
                dificultad = 10;
                break;
        }

        StaticVariables.setDificultadIA(dificultad);
    }

    public void EmpezarPartida()
    {
        if (StaticVariables.getMultijugador())
            PhotonNetwork.LoadLevel("Juego");
        else
            SceneManager.LoadScene("Juego");
    }

    public void Atras()
    {
        SceneManager.LoadScene("JugarOpcionesMultijugador");
    }

}
