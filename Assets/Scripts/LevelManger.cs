using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Panel de Victoire")]
    public GameObject victoryPanel; // Assigner dans l'Inspector

    [Header("Temps avant changement")]
    public float delayBeforeNextLevel = 3f;

    [Header("Nom de la scène suivante (laisser vide pour passer par index)")]
    public string nextLevelName = "Level2"; // ou vide si tu préfères index

    // Appeler quand le joueur gagne
    public void OnLevelComplete()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[LevelManager] victoryPanel n'est pas assigné !");
        }

        StartCoroutine(LoadNextLevelAfterDelay());
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayBeforeNextLevel));

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            // Vérifier si la scène existe dans Build Profiles
            if (IsSceneInBuild(nextLevelName))
            {
                Debug.Log("[LevelManager] Chargement scène par nom : " + nextLevelName);
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogError($"[LevelManager] La scène '{nextLevelName}' n'existe pas dans Build Profiles (ou nom incorrect).");
            }
        }
        else
        {
            // Chargement par index (sécurisé)
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            if (nextIndex < sceneCount)
            {
                Debug.Log("[LevelManager] Chargement scène par index : " + nextIndex);
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                Debug.LogWarning("[LevelManager] Aucun niveau suivant (index hors bornes). Vérifie Build Profiles.");
            }
        }
    }

    // Vérifie si un nom de scène est présent dans Build Profiles
    private bool IsSceneInBuild(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i); // ex: "Assets/Scenes/Level2.unity"
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }

    // Méthodes publiques utiles
    public void LoadNextLevel() => StartCoroutine(LoadNextLevelAfterDelay()); // réutilise la logique
    public void LoadNextLevelByIndex()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int next = currentSceneIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else Debug.LogWarning("[LevelManager] LoadNextLevelByIndex: pas de niveau suivant.");
    }
    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
