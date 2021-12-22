using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class CargarPartidaGuardada : MonoBehaviour
{
    public GameObject content;
    public GameObject prefabBoton;

    string[] listaPartidas;
    string seleccionado;

    GameObject botonSeleccionado;
    List<GameObject> listaBotones;

    // Start is called before the first frame update
    void Start()
    {
        seleccionado = "";
        botonSeleccionado = null;

        listaPartidas = SaveSystem.CargarPosiblesPartidas();
        listaBotones = new List<GameObject>();

        if(listaPartidas != null)
            cargarEnPantallaLista();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void cargarEnPantallaLista()
    {
        
        int n = 0;
        foreach (var p in listaPartidas) 
        {

            GameObject go = Instantiate(prefabBoton, content.transform);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - n, go.transform.position.z);
            Text textChildren = go.GetComponentInChildren<Text>();
            textChildren.text = cargarNombre(p);
            go.name = cargarNombre(p);
            go.GetComponent<Button>().onClick.AddListener(() => Seleccionado(textChildren));
            listaBotones.Add(go);

            n += 100;
        }
    }

    string cargarNombre(string p)
    {
        string[] s;

        p = p.Replace('\\', '/');

        s = p.Split('/');

        s = s[s.Length - 1].Split('.');
        return s[0];
    }

    public void Cargar()
    {
        if(!seleccionado.Equals(""))
        {
            Partida p = SaveSystem.CargarPartida(seleccionado);
            if (p != null)
            {
                StaticVariables.setPartida(p);
                StaticVariables.setVerPartida(true);
                StaticVariables.setMultijugador(false);
                StaticVariables.setJuegoRaton(true);
                SceneManager.LoadScene("VerPartida");
            }
            else
                Debug.Log("ERROR AL CARGAR LA PARTIDA");
        }
    }

    public void Eliminar()
    {
        if (!seleccionado.Equals(""))
        {
            foreach (var b in listaBotones)
            {
                Destroy(b);
            }
            SaveSystem.EliminarPartida(seleccionado);
            seleccionado = "";
            SceneManager.LoadScene("PartidasGuardadas");
            //cargarEnPantallaLista();
        }
    }

    public void Seleccionado(Text texto)
    {
        if(botonSeleccionado != null)
        {
            botonSeleccionado.GetComponent<Image>().color = Color.white;
        }

        foreach (var p in listaPartidas)
        {
            if (texto.text.Equals(cargarNombre(p)))
            {
                seleccionado = p;
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    var b = content.transform.GetChild(i).gameObject;
                    if(b.name.Equals(texto.text))
                    {
                        botonSeleccionado = b;
                        botonSeleccionado.GetComponent<Image>().color = Color.gray;
                        break;
                    }
                }
                break;
            }
        }
    }

    public void Atras()
    {
        SceneManager.LoadScene("Menu");
    }

}
