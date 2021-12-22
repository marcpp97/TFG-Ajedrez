using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static string pathDirectorio = Application.persistentDataPath + "/Partidas";

    static string tipoArchivo = ".apg"; // apg = ajedrez pa guapos

    public static string NombrePartida()
    {
        string n;
        if (StaticVariables.getMultijugador())
            n = "Multijugador ";
        else
            n = "IA ";

        n += DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
        return n;
    }

    public static void GuardarPartida()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(pathDirectorio))
        {
            Directory.CreateDirectory(pathDirectorio);
        }
        
        string path = pathDirectorio + "/" + StaticVariables.getPartida().nombre + tipoArchivo;

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, StaticVariables.getPartida());
        stream.Close();

    }

    public static string[] CargarPosiblesPartidas()
    {
        if (Directory.Exists(pathDirectorio))
        {
            return Directory.GetFiles(pathDirectorio);
        }
        else
            return null;
    }

    public static Partida CargarPartida(string s)
    {
        if (File.Exists(s))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(s, FileMode.Open);

            Partida p =  formatter.Deserialize(stream) as Partida;
            stream.Close();

            return p;
        }
        else
            return null;

    }

    public static void EliminarPartida(string s)
    {
        if (File.Exists(s))
        {
            File.Delete(s);
        }
    }

}
