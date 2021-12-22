using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opciones : MonoBehaviour
{

    public Toggle pantallaCompleta;
    public Dropdown idiomas;

    Resolution resolution;

    private void Start()
    {
        iniciarIdiomas(StaticVariables.getIdioma());

        pantallaCompleta.isOn = StaticVariables.getPantalla();

    }

    private void Update()
    {
        
    }

    public void setPantallaCompleta(bool pantalla)
    {
        StaticVariables.setPantalla(pantalla);

        if (pantalla)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        else
            Screen.fullScreen = false;
    }

    void iniciarIdiomas(string n)
    {
        string aux = "";

        switch (n)
        {
            case "es-ES":
                aux = "Castellano";
                break;
            case "ca-ES":
                aux = "Valenciano";
                break;
            case "en-US":
                aux = "Inglés";
                break;
            case "it-IT":
                aux = "Italiano";
                break;
            case "zh-CN":
                aux = "Chino";
                break;
            default:
                aux = "Castellano";
                break;
        }

        idiomas.value = idiomas.options.FindIndex(opcion => opcion.text.Equals(aux));
    }

    public void setIdioma(int indice)
    {
        switch (indice)
        {
            case 0:
                StaticVariables.setIdioma("es-ES");
                break;
            case 1:
                StaticVariables.setIdioma("ca-ES");
                break;
            case 2:
                StaticVariables.setIdioma("en-US");
                break;
            case 3:
                StaticVariables.setIdioma("it-IT");
                break;
            case 4:
                StaticVariables.setIdioma("zh-CN");
                break;
            default:
                StaticVariables.setIdioma("es-ES");
                break;
        }
    }

    public void atras()
    {
        SceneManager.LoadScene("Menu");
    }

}
