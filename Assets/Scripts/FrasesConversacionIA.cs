using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrasesConversacionIA
{

    static string[] frasesComerPieza = new string[] 
    { "Ja, esa pieza es mía ahora", "Lo siento pero me quedo con esta pieza", "A la proxima mira donde pones tus piezas", "Te comí",
      "Tienes una pieza menos ahora" };

    static string[] frasesTardaMucho = new string[] 
    { "Estoy esperando a que muevas", "Me voy a hacer un café", "Veo que te está costando decidirte", "Te veo sufrir", "Venga no tengo todo el dia" };

    static string[] frasesJugadorMueve = new string[]
    { "¡Por fin has movido!", "¿Ese es tu movimiento?", "No esta mal" };

    static string[] frasesIAMueve = new string[]
    { "¿Te ha gustado mi movimiento?", "Muevo esta pieza", "¡Que jugada he hecho!" };

    public static string[] FrasesAleatorias(int i)
    {

        switch (i)
        {
            case 0:
                return frasesComerPieza;
            case 1:
                return frasesTardaMucho;
            case 2:
                return frasesJugadorMueve;
            case 3:
                return frasesIAMueve;
            default:
                return frasesTardaMucho;
        }

    }

}
