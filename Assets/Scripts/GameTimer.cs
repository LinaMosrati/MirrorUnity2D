using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 40f;
    private float currentTime;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;

    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningTime = 10f;
    public float pulseSpeed = 4f;
    public float pulseScale = 1.15f;

    private bool gameStopped = false;

    void Start()
    {
        currentTime = startTime;
        gameOverPanel.SetActive(false);
        timerText.color = normalColor;
    }

    void Update()
    {
        if (gameStopped) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        // Format time as MM:SS
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // Change color when time is almost up
        if (currentTime <= warningTime)
        {
            timerText.color = warningColor;

            // Small pulse scaling animation
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1);
            timerText.transform.localScale = new Vector3(scale, scale, 1);
        }
        else
        {
            timerText.color = normalColor;
            timerText.transform.localScale = Vector3.one;
        }
    }

    void GameOver()
    {
        gameStopped = true;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        timerText.text = "TIME OVER";
        timerText.transform.localScale = Vector3.one;
    }
}
