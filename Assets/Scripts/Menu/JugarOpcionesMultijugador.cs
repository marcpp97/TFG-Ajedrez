using UnityEngine;
using UnityEngine.SceneManagement;

public class JugarOpcionesMultijugador : MonoBehaviour
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
        StaticVariables.setMultijugador(n);

        if(n)
            SceneManager.LoadScene("Juego");
        else
            SceneManager.LoadScene("NivelDificultadIA");
    }

    public void atras()
    {
        SceneManager.LoadScene("JugarOpciones");
    }

}
