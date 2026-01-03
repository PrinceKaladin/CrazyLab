using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorFlaskGame : MonoBehaviour
{
    // Нормальные спрайты колб (8 штук)
    public Sprite[] normalSprites = new Sprite[8];          // 0-red, 1-orange, 2-yellow, 3-green, 4-blue, 5-purple, 6-pink, 7-reserve

    // Спрайты ПРАВИЛЬНОГО выбора для каждого цвета (например, светящиеся/с ободком)
    public Sprite[] correctSprites = new Sprite[8];

    // Спрайты НЕПРАВИЛЬНОГО выбора для каждого цвета (например, тёмные/с крестиком)
    public Sprite[] incorrectSprites = new Sprite[8];

    public AudioClip correctSound;

    public Text questionText;        // UI текст вопроса
    public Text scoreText;           // UI текст счёта

    private GameObject[] flasks = new GameObject[3];
    private int correctIndex;        // 0..6 — индекс правильного цвета
    private string[] colors = { "red", "orange", "yellow", "green", "blue", "purple", "pink" };
    private int correctGuesses = 0;
    private AudioSource audioSource;
    private bool canClick = true;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        SetupUI();

        // Создаём 3 колбы
        // Внутри цикла создания колб в методе Start():
        for (int i = 0; i < 3; i++)
        {
            flasks[i] = new GameObject("Flask" + i);
            flasks[i].transform.position = new Vector3(-1.2f + i * 1.2f, 0, 0);

            // ← Вот это главное изменение
            flasks[i].transform.localScale = new Vector3(0.3f, 0.3f, 1f);  // в 2 раза меньше

            SpriteRenderer sr = flasks[i].AddComponent<SpriteRenderer>();
            BoxCollider2D col = flasks[i].AddComponent<BoxCollider2D>();

            // Коллайдер тоже уменьшаем пропорционально
            col.size = new Vector2(3f, 5f);           // было примерно 1.5×2.5 → теперь в 2 раза больше, т.к. масштаб 0.5
                                                      // или ещё точнее: col.size = new Vector2(3f, 5f) / 0.5f;  // = 6×10, если масштаб станет другим

            sr.sortingOrder = 1;
        }

        NewRound();
        UpdateScoreUI();
    }

    void SetupUI()
    {
        if (questionText != null) return;

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Вопрос
        GameObject qObj = new GameObject("Question");
        qObj.transform.SetParent(canvasObj.transform);
        questionText = qObj.AddComponent<Text>();
        questionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        questionText.fontSize = 40;
        questionText.color = Color.white;
        questionText.alignment = TextAnchor.MiddleCenter;
        questionText.rectTransform.sizeDelta = new Vector2(900, 120);
        questionText.rectTransform.anchoredPosition = new Vector2(0, 180);

        // Счёт
        GameObject sObj = new GameObject("Score");
        sObj.transform.SetParent(canvasObj.transform);
        scoreText = sObj.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        scoreText.fontSize = 28;
        scoreText.color = new Color(1f, 0.9f, 0.4f);
        scoreText.rectTransform.sizeDelta = new Vector2(400, 60);
        scoreText.rectTransform.anchoredPosition = new Vector2(-380, 240);
    }

    void Update()
    {
        if (!canClick || !Input.GetMouseButtonDown(0)) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (hit.collider.gameObject == flasks[i])
                {
                    HandleSelection(i);
                    break;
                }
            }
        }
    }

    void NewRound()
    {
        canClick = false;

        // Случайно выбираем три разных цвета
        int[] selectedColors = PickThreeDifferentColors();

        // Назначаем нормальные спрайты
        for (int i = 0; i < 3; i++)
        {
            flasks[i].GetComponent<SpriteRenderer>().sprite = normalSprites[selectedColors[i]];
        }

        // Выбираем, какая из трёх будет правильной
        correctIndex = Random.Range(0, 3);
        int correctColorId = selectedColors[correctIndex];

        questionText.text = $"Select <b>{colors[correctColorId]}</b> bulb!";

        Invoke(nameof(EnableClicking), 0.6f);
    }

    private int[] PickThreeDifferentColors()
    {
        int[] indices = new int[3];
        indices[0] = Random.Range(0, 7);
        do { indices[1] = Random.Range(0, 7); } while (indices[1] == indices[0]);
        do { indices[2] = Random.Range(0, 7); } while (indices[2] == indices[0] || indices[2] == indices[1]);
        return indices;
    }

    void EnableClicking() => canClick = true;

    void HandleSelection(int selected)
    {
        if (!canClick) return;
        canClick = false;

        SpriteRenderer sr = flasks[selected].GetComponent<SpriteRenderer>();
        int colorId = GetCurrentColorId(selected); // какой сейчас цвет у выбранной колбы

        if (selected == correctIndex)
        {
            sr.sprite = correctSprites[colorId];
            audioSource.PlayOneShot(correctSound);
            correctGuesses++;
            UpdateScoreUI();

            if (correctGuesses >= 5)
                Invoke(nameof(NextLevel), 1.8f);
            else
                Invoke(nameof(NewRound), 1.4f);
        }
        else
        {
            sr.sprite = incorrectSprites[colorId];
            Invoke(nameof(NewRound), 1.4f);
        }
    }

    private int GetCurrentColorId(int flaskIndex)
    {
        Sprite current = flasks[flaskIndex].GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < normalSprites.Length; i++)
        {
            if (normalSprites[i] == current)
                return i;
        }
        return 0; // fallback
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Correct: {correctGuesses} / 5";
    }

    void NextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.Log("Level");
    }
}