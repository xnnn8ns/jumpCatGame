using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Platform_Generator : MonoBehaviour {

    public GameObject Platform_Green;
    public GameObject TopPlatformPrefab;
    public GameObject Platform_Blue;
    public GameObject Platform_White;
    public GameObject Platform_Brown;

    public GameObject Spring;
    public GameObject Trampoline;
    public GameObject Propeller;

    private GameObject Platform;
    private GameObject Random_Object;
    float Offset;
    Vector3 Top_Left;
    private Game_Controller gameController;
    private GameObject highestPlatform;
    private GameObject topPlatform;

    public float Current_Y = 0;
    public bool ShouldSpawnPlatforms = true;

	void Start () 
    {
        // Границы
        Top_Left = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Offset = 1.2f;

        gameController = GetComponent<Game_Controller>();
	}

    public void Generate_Platform(int Num)
    {
        if (!ShouldSpawnPlatforms)
        {
            return;
        }

        for (int i = 0; i < Num; i++)
        {
            // Высчитываем x, y платформ
            float Dist_X = Random.Range(Top_Left.x + Offset, -Top_Left.x - Offset);
            float Dist_Y = Random.Range(2f, 5f);

            // коричневая платформа с шансом 1/8
            int Rand_BrownPlatform = Random.Range(1, 8);

            if (Rand_BrownPlatform == 1)
            {
                float Brown_DistX = Random.Range(Top_Left.x + Offset, -Top_Left.x - Offset);
                float Brown_DistY = Random.Range(Current_Y + 1, Current_Y + Dist_Y - 1);
                Vector3 BrownPlatform_Pos = new Vector3(Brown_DistX, Brown_DistY, 0);

                Instantiate(Platform_Brown, BrownPlatform_Pos, Quaternion.identity);
            }

            // Остальные платформы
            Current_Y += Dist_Y;
            Vector3 Platform_Pos = new Vector3(Dist_X, Current_Y, 0);
            int Rand_Platform = Random.Range(1, 10);

            if (Rand_Platform == 1) // Голубая платформа
                Platform = Instantiate(Platform_Blue, Platform_Pos, Quaternion.identity);
            else if (Rand_Platform == 2) // Белая
                Platform = Instantiate(Platform_White, Platform_Pos, Quaternion.identity);
            else // Зеленая
                Platform = Instantiate(Platform_Green, Platform_Pos, Quaternion.identity);

            if (Rand_Platform != 2)
            {
                // Создаем доп. объекты (пружинка, пропеллер, батут)
                int Rand_Object = Random.Range(1, 40);

                // Проверяем, находится ли счет игрока в диапазоне от MaxScore - 4000 до MaxScore
                if (!(gameController.GetScore() >= gameController.GetMaxScore() - 4000 && gameController.GetScore() <= gameController.GetMaxScore()))
                {
                    if (Rand_Object == 4) // пружинка
                    {
                        Vector3 Spring_Pos = new Vector3(Platform_Pos.x + 0.5f, Platform_Pos.y + 0.27f, 0);
                        Random_Object = Instantiate(Spring, Spring_Pos, Quaternion.identity);
                        
                        // Устанавливаем объект в родителя
                        Random_Object.transform.parent = Platform.transform;
                    }
                    else if (Rand_Object == 7) // Батут
                    {
                        Vector3 Trampoline_Pos = new Vector3(Platform_Pos.x + 0.13f, Platform_Pos.y + 0.25f, 0);
                        Random_Object = Instantiate(Trampoline, Trampoline_Pos, Quaternion.identity);

                        Random_Object.transform.parent = Platform.transform;
                    }
                    else if (Rand_Object == 15) // Пропеллер
                    {
                        Vector3 Propeller_Pos = new Vector3(Platform_Pos.x + 0.13f, Platform_Pos.y + 0.15f, 0);
                        Random_Object = Instantiate(Propeller, Propeller_Pos, Quaternion.identity);

                        Random_Object.transform.parent = Platform.transform;
                    }
                }
            }

            // Обновляем highestPlatform, если текущая платформа выше
            if (highestPlatform == null || Platform.transform.position.y > highestPlatform.transform.position.y)
            {
                highestPlatform = Platform;
            }
        }

        // Если игрок достиг MaxScore, создаем самую верхнюю платформу из префаба
        if (gameController.GetScore() >= gameController.GetMaxScore())
        {
            float Dist_Y = Random.Range(2f, 5f);
        
            // Создаем новую платформу из префаба
            Vector3 topPlatformPosition = highestPlatform.transform.position + new Vector3(0, Dist_Y, 0);
            topPlatform = Instantiate(TopPlatformPrefab, topPlatformPosition, Quaternion.identity);
        }
    }
}
