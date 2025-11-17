using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int totalKeysRequired = 3;
    public string winMessage = "You Escaped!";
    public string loseMessage = "Caught by Jimmy!";
    
    [Header("UI References")]
    public GameObject winUI;
    public GameObject loseUI;
    
    private PlayerController player;
    private bool gameEnded = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
        }
        
        if (winUI != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
    }
    
    public void CheckWinCondition()
    {
        if (gameEnded) return;
        
        if (player != null && player.GetKeyCount() >= totalKeysRequired)
        {
            WinGame();
        }
    }
    
    public void WinGame()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log(winMessage);
        
        if (winUI != null)
        {
            winUI.SetActive(true);
        }
        
        Time.timeScale = 0f; // Pause game
    }
    
    public void GameOver()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log(loseMessage);
        
        if (loseUI != null)
        {
            loseUI.SetActive(true);
        }
        
        Time.timeScale = 0f; // Pause game
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
