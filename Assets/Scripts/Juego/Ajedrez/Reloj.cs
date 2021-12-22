using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reloj : MonoBehaviour
{

    public Text texto;

    int escalaTiempo = 0;

    float tiempoInicial = 60 * 10;
    float tiempoRelojSegundos = 0f;

    // Start is called before the first frame update
    void Start()
    {

        tiempoRelojSegundos = tiempoInicial;

        actualizarReloj();

    }

    // Update is called once per frame
    void Update()
    {
        if (escalaTiempo != 0 && !StaticVariables.getVerPartida())
        {
            tiempoRelojSegundos -= Time.deltaTime * escalaTiempo;
            actualizarReloj();
            if (tiempoRelojSegundos <= 0f)
            {
                Ajedrez ajedrez = GameObject.Find("Juego").GetComponent<Juego>().getAjedrez();
                StaticVariables.setColorPierde(ajedrez.turno);
                StaticVariables.setFinPartida(true);
            }
        }

    }

    void actualizarReloj()
    {

        if (tiempoRelojSegundos < 0) tiempoRelojSegundos = 0;

        var m = (int)tiempoRelojSegundos / 60;
        var s = (int)tiempoRelojSegundos % 60;

        texto.text = m.ToString("00") + ":" + s.ToString("00");

    }

    public void pararReloj()
    {
        escalaTiempo = 0;
    }

    public void continuarReloj()
    {
        escalaTiempo = 1;
    }

    public void setTiempo(int segundos)
    {
        tiempoRelojSegundos = segundos;
        actualizarReloj();
    }

    public int getTiempo()
    {
        return (int) tiempoRelojSegundos;
    }

    public void darTiempo(int minutos)
    {

        int segundos = minutos * 60;

        tiempoRelojSegundos += segundos;

    }

}
