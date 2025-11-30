using UnityEngine;

public class PlayerMenuMove : MonoBehaviour
{
    public float speed = 100f;    // Vitesse très rapide
    public float resetX = 10f;   // Position X où le joueur disparaît (à droite)
    public float startX = -10f;  // Position X où il réapparaît (à gauche)

    void Update()
    {
        // Déplacer le joueur vers la droite
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Réinitialiser sa position quand il sort de l'écran
        if (transform.position.x > resetX)
        {
            Vector3 pos = transform.position;
            pos.x = startX;
            transform.position = pos;
        }
    }
}
