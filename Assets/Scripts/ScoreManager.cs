using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;
    public TMP_Text levelPassedText;
    private int score = 0;
    private Vector3 originalScale;  
    public GameObject winPanel;

    [Header("Level Settings")]
    public int scoreToPass = 3;

    [Header("Level Passed Animation Settings")]
    public float fadeDuration = 0.5f;
    public float popScale = 1.5f;
    public float displayTime = 1.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        originalScale = scoreText.transform.localScale;
        UpdateScoreText();

        if (levelPassedText != null)
        {
            levelPassedText.gameObject.SetActive(false);
            levelPassedText.alpha = 0;
            levelPassedText.transform.localScale = Vector3.one;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        StartCoroutine(PopAnimation());

        if (score >= scoreToPass)
            DisplayWinPanel();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"<color=#FFD700><b>★ {score}</b></color>";
    }

    private IEnumerator PopAnimation()
    {
        scoreText.transform.localScale = originalScale * 1.3f;
        yield return new WaitForSeconds(0.15f);
        scoreText.transform.localScale = originalScale;
    }

    // ✅ MÉTHODE CORRIGÉE - Utilise uniquement le LevelManager
    private void DisplayWinPanel()
    {
        Debug.Log("=== 🎯 LEVEL COMPLETE ===");
        Debug.Log("Score atteint : " + score + " / " + scoreToPass);
        
        // Afficher le panel de victoire
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Debug.Log("✅ Win Panel activé");
        }
        else
        {
            Debug.LogWarning("⚠️ Win Panel non assigné dans l'Inspector");
        }

        // Trouver et appeler le LevelManager
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        if (levelManager != null)
        {
            Debug.Log("✅ LevelManager trouvé ! Appel de OnLevelComplete()");
            levelManager.OnLevelComplete();
        }
        else
        {
            Debug.LogError("❌ ERREUR : LevelManager non trouvé dans la scène !");
            Debug.LogError("Assure-toi qu'il y a un GameObject avec le script LevelManager dans la scène.");
        }
    }

    // ✅ ANIMATION OPTIONNELLE (si tu veux l'utiliser avant de charger la scène)
    private IEnumerator LevelPassedAnimation()
    {
        if (levelPassedText == null) yield break;

        levelPassedText.gameObject.SetActive(true);
        levelPassedText.text = "<gradient=#FFD700,#FFA500><b> LEVEL PASSED! </b></gradient>";

        // Animation d'apparition
        float t = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * popScale;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            levelPassedText.alpha = alpha;
            levelPassedText.transform.localScale = Vector3.Lerp(startScale, targetScale, t / fadeDuration);
            yield return null;
        }

        // Animation de stabilisation
        t = 0f;
        Vector3 normalScale = Vector3.one;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            levelPassedText.transform.localScale = Vector3.Lerp(targetScale, normalScale, t / 0.2f);
            yield return null;
        }

        // Animation de rotation
        float rotationTime = 0.5f;
        t = 0f;
        Quaternion startRot = Quaternion.Euler(0, 0, -5);
        Quaternion endRot = Quaternion.Euler(0, 0, 5);
        while (t < rotationTime)
        {
            t += Time.deltaTime;
            levelPassedText.transform.rotation = Quaternion.Lerp(startRot, endRot, Mathf.PingPong(t * 4, 1));
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        // Fade out avant de charger
        if (FadeManager.instance != null)
            yield return StartCoroutine(FadeManager.instance.FadeOut(1f));
    }

    // ✅ MÉTHODE UTILITAIRE pour tester le score (optionnel)
    [ContextMenu("Test Add Score")]
    private void TestAddScore()
    {
        AddScore(1);
    }

    [ContextMenu("Test Win Condition")]
    private void TestWinCondition()
    {
        score = scoreToPass;
        DisplayWinPanel();
    }
}