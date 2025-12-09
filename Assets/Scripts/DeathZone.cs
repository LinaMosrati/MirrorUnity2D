using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject gameOverPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0f; // Pause le jeu
            gameOverPanel.SetActive(true); // Affiche Game Over
        }
    }
}