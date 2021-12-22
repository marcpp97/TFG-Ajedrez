using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void Start()
    {
        
    }

    public void Update()
    {

    }

    public void cargarEscena(string n)
    {
        SceneManager.LoadScene(n);
    }

    public void salirJuego()
    {
        Application.Quit();
    }
}
