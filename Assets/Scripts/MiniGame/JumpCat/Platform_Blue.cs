using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Blue : MonoBehaviour
{

    // Движущаяся платформа
    private bool To_Right = true;
    private float Offset = 1.2f;

    void FixedUpdate()
    {
        // Двигаем платформу
        Vector3 Top_Left = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

        if (To_Right) // Направо
        {
            if (transform.position.x < -Top_Left.x - Offset)
                transform.position += new Vector3(0.1f, 0, 0);
            else
                To_Right = false;
        }
        else // Налево
        {
            if (transform.position.x > Top_Left.x + Offset)
                transform.position -= new Vector3(0.1f, 0, 0);
            else
                To_Right = true;
        }
    }
}
