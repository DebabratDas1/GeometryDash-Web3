using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int maxScore = 5000;
    public int currentScore = 0;
    public Transform player; 
    public float levelLength = 100f; 

    public TextMeshProUGUI scoreText;

    private Vector3 startPosition;

    private int lastScore = -1; 

    
    public bool useSmoothing = false;
    private float displayedProgress = 0f;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
       
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        StartCoroutine(DelayedFindReferencesAndReset());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFindReferencesAndReset());
    }

    IEnumerator DelayedFindReferencesAndReset()
    {
     
        yield return null;

        FindReferences();
        ResetScore();
    }

    void FindReferences()
    {
       
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            startPosition = player.position;
         
        }
        else
        {
            player = null;
        
            Debug.LogWarning("Player not found in scene!");
        }

        
        GameObject scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
        {
            scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
           
        }
        else
        {
            scoreText = null;
            
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceTravelled = player.position.x - startPosition.x;
        float targetProgress = Mathf.Clamp01(distanceTravelled / levelLength);

        if (useSmoothing)
        {
            displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 5f);
            currentScore = Mathf.RoundToInt(maxScore * displayedProgress);
        }
        else
        {
            currentScore = Mathf.RoundToInt(maxScore * targetProgress);
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null && currentScore != lastScore)
        {
            
            scoreText.text = "Score: " + currentScore;
            lastScore = currentScore;
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        lastScore = -1;
        displayedProgress = 0f;

        if (player != null)
            startPosition = player.position;

        UpdateScoreUI();
    }
}
