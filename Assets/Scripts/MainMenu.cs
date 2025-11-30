using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{  
      public void StartLevel1() { SceneManager.LoadScene("Level1"); } 
      public void StartLevel2() { SceneManager.LoadScene("Level2"); }
    [Header("Scene Names")]
    public string level1Name = "Level1";
    public string level2Name = "Level2";

    [Header("Bounce Settings")]
    public float bounceScale = 1.1f;
    public float bounceSpeed = 8f;

    private Vector3 originalScale;
    private bool isHovered = false;

    [Header("Button Type")]
    public bool isForestButton = false; // assign in Inspector
    public bool isSpaceButton = false;  // assign in Inspector

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Smooth bounce effect
        if (isHovered)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale * bounceScale,
                Time.deltaTime * bounceSpeed
            );
        }
        else
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale,
                Time.deltaTime * bounceSpeed
            );
        }
    }

    // Called when mouse enters
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    // Called when mouse exits
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

 
   
}
