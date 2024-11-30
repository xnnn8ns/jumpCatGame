using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPlatform : MonoBehaviour
{
    private bool playerBounced = false;
    public static bool gameNeedEnd = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !gameNeedEnd)
        {
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);

                // Если игрок уже отскочил, устанавливаем флаг, что игра завершена
                if (playerBounced)
                {
                    gameNeedEnd = true;
                    playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                // Если игрок отскочил вверх от платформы, устанавливаем флаг
                if (playerRigidbody.velocity.y > 0)
                {
                    playerBounced = true;
                }
            }
        }
    }
}