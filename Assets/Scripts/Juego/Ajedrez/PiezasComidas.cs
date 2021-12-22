using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiezasComidas : MonoBehaviour
{
    int color;
    
    Dictionary<string, int> listaPiezas;

    Sprite[] piezasSprites;

    GameObject go;

    List<GameObject> listaGameObjects;

    public void iniciar(int c, Sprite[] l)
    {
        color = c;
        piezasSprites = l;

        reiniciarListaPiezas();

        go = this.gameObject;
        listaGameObjects = new List<GameObject>();
    }

    void reiniciarListaPiezas()
    {
        listaPiezas = new Dictionary<string, int>();

        listaPiezas.Add("p", 0);
        listaPiezas.Add("c", 0);
        listaPiezas.Add("a", 0);
        listaPiezas.Add("t", 0);
        listaPiezas.Add("d", 0);
    }

    public void añadirPiezaComida(string s)
    {
        listaPiezas[s] = listaPiezas[s] + 1;

        dibujarPiezasComidas();
    }

    void dibujarPiezasComidas()
    {
        if (listaGameObjects.Count != 0)
        {
            foreach (var p in listaGameObjects)
            {
                Destroy(p);
            }
        }

        if (listaPiezas.Count != 0)
        {
            float x = -180;
            float z = 0;
            foreach (var p in listaPiezas)
            {
                for (int i = 0; i < p.Value; i++)
                {
                    x += 15;
                    z += 0.1f;

                    GameObject child = new GameObject();
                    child.AddComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                    child.transform.position = go.transform.position + new Vector3(x, 0, z);
                    child.transform.SetParent(go.transform);

                    if(color == 0)
                        switch (p.Key)
                        {
                            case "p":
                                child.AddComponent<Image>().sprite = piezasSprites[0];
                                break;
                            case "c":
                                child.AddComponent<Image>().sprite = piezasSprites[4];
                                break;
                            case "a":
                                child.AddComponent<Image>().sprite = piezasSprites[6];
                                break;
                            case "t":
                                child.AddComponent<Image>().sprite = piezasSprites[8];
                                break;
                            case "d":
                                child.AddComponent<Image>().sprite = piezasSprites[10];
                                break;
                            default:
                                break;
                        }
                    else
                        switch (p.Key)
                        {
                            case "p":
                                child.AddComponent<Image>().sprite = piezasSprites[1];
                                break;
                            case "c":
                                child.AddComponent<Image>().sprite = piezasSprites[5];
                                break;
                            case "a":
                                child.AddComponent<Image>().sprite = piezasSprites[7];
                                break;
                            case "t":
                                child.AddComponent<Image>().sprite = piezasSprites[9];
                                break;
                            case "d":
                                child.AddComponent<Image>().sprite = piezasSprites[11];
                                break;
                            default:
                                break;
                        }
                    listaGameObjects.Add(child);
                }
                if(p.Value != 0)
                    x += 30;
            }
        }
    }

}
