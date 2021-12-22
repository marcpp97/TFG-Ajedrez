using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JugarOpciones : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cargarEscena(bool n)
    {
        StaticVariables.setJuegoRaton(n);

        SceneManager.LoadScene("JugarOpcionesMultijugador");
    }

    public void atras()
    {
        SceneManager.LoadScene("Menu");
    }

}
