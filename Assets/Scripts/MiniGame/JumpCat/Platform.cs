using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public float Jump_Force = 10f;
    private float Destroy_Distance;
    private bool Create_NewPlatform = false;
    private GameObject Game_Controller;

    void Start()
    {
        Game_Controller = GameObject.Find("Game_Controller");

        // Установить расстояние для уничтожения платформ за пределами экрана
        Destroy_Distance = Game_Controller.GetComponent<Game_Controller>().Get_DestroyDistance();
    }

    void FixedUpdate()
    {
        // Если платформа за экраном
        if (transform.position.y - Camera.main.transform.position.y < Destroy_Distance)
        {
            // Создаем новую
            if (name != "Platform_Brown(Clone)" && name != "Spring(Clone)" && name != "Trampoline(Clone)" && !Create_NewPlatform)
            {
                Game_Controller.GetComponent<Platform_Generator>().Generate_Platform(1);
                Create_NewPlatform = true;
            }
            
            // Деактивируем всякие эффекты, картинку
            GetComponent<EdgeCollider2D>().enabled = false;
            GetComponent<PlatformEffector2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;

            // Окончательно отключаем и удаляем
            if (transform.childCount > 0)
            {
                if(transform.GetChild(0).GetComponent<Platform>()) // Если есть дочерний объект
                {
                    transform.GetChild(0).GetComponent<EdgeCollider2D>().enabled = false;
                    transform.GetChild(0).GetComponent<PlatformEffector2D>().enabled = false;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                }

                // удаляем платформу, когда звук завершится
                if (!GetComponent<AudioSource>().isPlaying && !transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
                    Destroy(gameObject);
            }
            else
            {
                // удаляем платформу, когда звук завершится
                if (!GetComponent<AudioSource>().isPlaying)
                    Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D Other)
    {
        // добавляем силы, когда игрок падает сверху
        if (Other.relativeVelocity.y < 0f)
        {
            Rigidbody2D Rigid = Other.collider.GetComponent<Rigidbody2D>();
    
            if (Rigid != null)
            {
                Vector2 Force = Rigid.velocity;
                Force.y += Jump_Force; // Добавляем Jump_Force к текущей вертикальной скорости
                Rigid.velocity = Force;
    
                // Воспроизводим звук прыжка
                GetComponent<AudioSource>().Play();
    
                // Если у игрового объекта есть анимация; Например, пружина, батут и пропеллер
                if (GetComponent<Animator>())
                    GetComponent<Animator>().SetBool("Active", true);
    
                // Проверяем тип платформы
                Platform_Type();
            }
        }
    }

    void Platform_Type()
    {
        if (GetComponent<Platform_White>())
            GetComponent<Platform_White>().Deactive();
        else if (GetComponent<Platform_Brown>())
            GetComponent<Platform_Brown>().Deactive();
    }
}
