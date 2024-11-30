using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {

    [SerializeField] private AudioClip winGameSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private Sprite YouWinSprite;
    public AudioSource audioSourceWin;
    private GameObject Player;

    private float Max_Height = 0;
    private const int MaxScore = 25000;
    public TextMeshProUGUI Txt_Score;

    private int Score;

    private Vector3 Top_Left;
    private Vector3 Camera_Pos;

    private bool Game_Over = false;
    private bool platformsSpawned = true;
    private bool gameEnding = false;
    private bool controlEnabled = false;

    public TextMeshProUGUI Txt_GameOverScore;
    private Platform_Generator platformGenerator;

    void Awake () 
    {
        Player = GameObject.Find("Cat");
        if (Player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        platformGenerator = GetComponent<Platform_Generator>();
        if (platformGenerator == null)
        {
            Debug.LogError("Platform_Generator not found");
            return;
        }

        Camera_Pos = Camera.main.transform.position;
        Top_Left = Camera.main.ScreenToWorldPoint(Vector3.zero);

        StartCoroutine(EnableControlAfterDelay(1.0f));
    }
    
    void FixedUpdate () 
    {
        if(!Game_Over && controlEnabled)
        {
            // Высчитываем максимальную высоту
            if (Player.transform.position.y > Max_Height)
            {
                Max_Height = Player.transform.position.y;
            }

            // Обновляем счет
            Score = (int)(Max_Height * 50);
            Txt_Score.text = Score.ToString();

            // проверяем проигрыш (путем высоты)
            if (Player.transform.position.y - Camera.main.transform.position.y < Get_DestroyDistance())
            {
                // проигрываем звук падения
                GetComponent<AudioSource>().Play();
                
                // Запускаем game_over
                StartCoroutine(Set_GameOver());
                Game_Over = true;
            }

            // Проверяем, достиг ли игрок максимального количества очков
            if (Score >= MaxScore && platformsSpawned)
            {
                // Останавливаем спавн платформ
                platformGenerator.Generate_Platform(1);
                platformGenerator.ShouldSpawnPlatforms = false;
                platformsSpawned = false;
            }

            // Убедитесь, что TopPlatform объявлен и инициализирован
            if (TopPlatform.gameNeedEnd && !gameEnding)
            {
                gameEnding = true;
                StartCoroutine(DelayedGameOver());
            }
        }
    }

    public bool Get_GameOver()
    {
        return Game_Over;
    }

    public float Get_DestroyDistance()
    {
        return Camera_Pos.y + Top_Left.y;
    }

    IEnumerator DelayedGameOver()
    {
        Rigidbody2D playerRigidbody = Player.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.gravityScale = 0;
        playerRigidbody.isKinematic = true;
    
        // Проигрываем звук победы
        audioSourceWin.PlayOneShot(winGameSound);
    
        yield return new WaitForSeconds(1.5f);
        
        Game_Over = true;
        StartCoroutine(Set_GameOver());
    }

    IEnumerator Set_GameOver()
    {
        GameObject Background_Canvas = GameObject.Find("Background_Canvas");
        yield return new WaitForSeconds(1.5f);

        if (Score < MaxScore * 0.3f) // Если игрок набрал меньше 30% от максимального счета
        {
            PlayerPrefs.SetInt("ErrorCount", Convert.ToInt32(Score / 1000));

            // Воспроизводим звук проигрыша
            audioSourceWin.PlayOneShot(loseSound);
        }

        // Включаем все объекты в Background_Canvas
        foreach (Transform child in Background_Canvas.transform)
        {
            child.gameObject.SetActive(true);
        }

        // Если игрок достиг самого высокого счета, меняем спрайт объекта Game_Over
        if (gameEnding)
        {
            GameObject Game_Over = GameObject.Find("Game_Over");
            Game_Over.GetComponent<Image>().sprite = YouWinSprite;
        }

        Txt_GameOverScore.text = Score.ToString();
        Background_Canvas.GetComponent<Animator>().enabled = true;

        // Убедитесь, что WhatDoPanel не активируется
        GameObject WhatDoPanel = Background_Canvas.transform.Find("WhatDoPanel").gameObject;
        if (WhatDoPanel != null)
        {
            WhatDoPanel.SetActive(false);
        }
    }

    IEnumerator EnableControlAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        controlEnabled = true;
    }

    public int GetScore()
    {
        return Score;
    }

    public int GetMaxScore()
    {
        return MaxScore;
    }

    public void RestartGame()
    {
        // Сброс всех необходимых переменных и состояний
        Max_Height = 0;
        Score = 0;
        Game_Over = false;
        platformsSpawned = true;
        gameEnding = false;
        controlEnabled = false;

        TopPlatform.gameNeedEnd = false;

        // Перезапуск текущей сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}