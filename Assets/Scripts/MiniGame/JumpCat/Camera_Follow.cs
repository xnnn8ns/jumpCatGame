using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {

    public Transform Target;
    
    private GameObject Game_Controller;
    private bool Game_Over = false;

    private float Time_ToDown = 0;

    void Start()
    {
        Game_Controller = GameObject.Find("Game_Controller");
    }

    void Update()
    {
        // Проверяем не закончилась ли игра
        Game_Over = Game_Controller.GetComponent<Game_Controller>().Get_GameOver();
    }

    void FixedUpdate()
    {
        // Если нет, то двигаем камеру
        if (Game_Over)
        {
            if(Time.time < Time_ToDown + 4f)
                transform.position -= new Vector3(0, 1f, 0);
            else
            {
                // Если да, то удаляем все объекты и игрока в том числе (с тэгами определенными)
                // GameObject Player = GameObject.FindGameObjectWithTag("Player");
                // GameObject[] Objects = GameObject.FindGameObjectsWithTag("Object");

                // Destroy(Player);
                // foreach (GameObject Obj in Objects)
                //     Destroy(Obj);
            }
        }
    }

	void LateUpdate () 
    {
        if(!Game_Over)
        {
            // Если игрок выше камеры на 2 единицы, то двигаем камеру
            if (Target.position.y > transform.position.y + 2)
            {
                Vector3 New_Pos = new Vector3(transform.position.x, Target.position.y - 2, transform.position.z);
                transform.position = New_Pos;
            }

            Time_ToDown = Time.time;
        }
	}
}
