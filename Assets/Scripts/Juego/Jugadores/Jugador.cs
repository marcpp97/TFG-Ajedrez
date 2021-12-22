using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Jugador
{

    public static GameObject DetectarClick()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;

    }

}
