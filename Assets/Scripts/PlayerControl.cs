using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MirrorMovement : MonoBehaviour
{
    [Header("Mirror Setup")]
    public Transform bottomPlayer;
    public Transform topPlayer;

    private Rigidbody2D bottomRb;
    private Rigidbody2D topRb;
    private GroundCheck bottomGround;
    private GroundCheck topGround;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private bool bottomBlocked = false;
    private bool topBlocked = false;

    [Header("Jump Animation Settings")]
    public Sprite[] jumpSprites; // assign sp1_7, sp1_8, sp1_10, sp1_11
    [Header("Walk Animation Settings")]
    public Sprite[] walkSprites; // assign sp1_1, sp1_2, sp1_3 (for example)
    public float walkFrameDuration = 0.15f;

    [Header("Sprite Renderers")]
    public SpriteRenderer bottomRenderer;
    public SpriteRenderer topRenderer;

    [Header("Crouch Settings")]
    public Sprite crouchSprite;  
    public Sprite crouchMoveSprite;
    public float crouchSpeed = 2f;
    public float crouchScaleY = 0.7f; // Réduction de taille (70% de la hauteur normale)

    private bool isCrouching = false;
    private bool topPlayerCrouching = false;  // Top player s'accroupit quand Space/↑
    private bool bottomPlayerCrouching = false;  // Bottom player s'accroupit quand ↓
    
    private Vector3 normalScale;
    private Vector3 crouchScale;

    [Header("Timing")]
    public float frameDuration = 0.1f;

    private bool isAnimatingJump = false;
    private bool isAnimatingWalk = false;

    void Start()
    {
        bottomRb = bottomPlayer.GetComponent<Rigidbody2D>();
        topRb = topPlayer.GetComponent<Rigidbody2D>();

        bottomGround = bottomPlayer.GetComponent<GroundCheck>();
        topGround = topPlayer.GetComponent<GroundCheck>();

        // Sauvegarder la taille normale
        normalScale = bottomPlayer.localScale;
        crouchScale = new Vector3(normalScale.x, normalScale.y * crouchScaleY, normalScale.z);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float horizontal = 0f;

        // Check crouch (seulement avec S maintenant)
        isCrouching = keyboard.sKey.isPressed;

        // Horizontal input
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            horizontal = -1;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            horizontal = 1;

        // Apply crouch speed if crouching
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // Prevent movement if blocked
        if (bottomBlocked && Mathf.Sign(horizontal) == Mathf.Sign(bottomRb.linearVelocity.x))
            horizontal = 0;

        // Movement
        bottomRb.linearVelocity = new Vector2(horizontal * currentSpeed, bottomRb.linearVelocity.y);

        // Mirror movement
        if (topBlocked && Mathf.Sign(-horizontal) == Mathf.Sign(topRb.linearVelocity.x))
            topRb.linearVelocity = new Vector2(0, topRb.linearVelocity.y);
        else
            topRb.linearVelocity = new Vector2(-horizontal * currentSpeed, topRb.linearVelocity.y);

        // Jump vers le haut (Space ou ↑) - Bottom saute, Top descend
        if ((keyboard.spaceKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) && !isCrouching)
        {
            bool jumped = false;

            if (bottomGround.isGrounded)
            {
                bottomRb.linearVelocity = new Vector2(bottomRb.linearVelocity.x, jumpForce);
                jumped = true;
            }

            if (topGround.isGrounded)
            {
                topRb.linearVelocity = new Vector2(topRb.linearVelocity.x, -jumpForce);
                jumped = true;
            }

            if (jumped && !isAnimatingJump)
            {
                StopCoroutineIfRunning(ref isAnimatingWalk);
                StartCoroutine(PlayJumpAnimation());
            }
        }

        // Jump inversé (↓) - Top saute vers le haut, Bottom descend
        if (keyboard.downArrowKey.wasPressedThisFrame && !isCrouching)
        {
            bool jumped = false;

            // Bottom player descend (force négative)
            if (bottomGround.isGrounded)
            {
                bottomRb.linearVelocity = new Vector2(bottomRb.linearVelocity.x, -jumpForce);
                jumped = true;
            }

            // Top player saute vers le haut (force positive)
            if (topGround.isGrounded)
            {
                topRb.linearVelocity = new Vector2(topRb.linearVelocity.x, jumpForce);
                jumped = true;
            }

            if (jumped && !isAnimatingJump)
            {
                StopCoroutineIfRunning(ref isAnimatingWalk);
                StartCoroutine(PlayJumpAnimation());
            }
        }

        // Handle animations
        if (isCrouching && bottomGround.isGrounded)
        {
            StopCoroutineIfRunning(ref isAnimatingWalk);
            StopCoroutineIfRunning(ref isAnimatingJump);

            // Réduire la taille des joueurs
            bottomPlayer.localScale = crouchScale;
            topPlayer.localScale = crouchScale;

            // If crouching + moving
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                bottomRenderer.sprite = crouchMoveSprite;
                topRenderer.sprite = crouchMoveSprite;
            }
            else
            {
                bottomRenderer.sprite = crouchSprite;
                topRenderer.sprite = crouchSprite;
            }
        }
        else if (Mathf.Abs(horizontal) > 0.1f && bottomGround.isGrounded && !isAnimatingJump)
        {
            // Remettre la taille normale
            bottomPlayer.localScale = normalScale;
            topPlayer.localScale = normalScale;

            if (!isAnimatingWalk)
                StartCoroutine(PlayWalkAnimation());
        }
        else
        {
            // Remettre la taille normale
            bottomPlayer.localScale = normalScale;
            topPlayer.localScale = normalScale;

            StopCoroutineIfRunning(ref isAnimatingWalk);
        }
    }

    private IEnumerator PlayJumpAnimation()
    {
        isAnimatingJump = true;
        for (int i = 0; i < jumpSprites.Length; i++)
        {
            bottomRenderer.sprite = jumpSprites[i];
            topRenderer.sprite = jumpSprites[i];
            yield return new WaitForSeconds(frameDuration);
        }
        isAnimatingJump = false;
    }

    private IEnumerator PlayWalkAnimation()
    {
        isAnimatingWalk = true;
        int index = 0;
        while (isAnimatingWalk)
        {
            bottomRenderer.sprite = walkSprites[index];
            topRenderer.sprite = walkSprites[index];
            index = (index + 1) % walkSprites.Length;
            yield return new WaitForSeconds(walkFrameDuration);
        }
    }

    private void StopCoroutineIfRunning(ref bool isAnimatingFlag)
    {
        if (isAnimatingFlag)
        {
            StopAllCoroutines();
            isAnimatingFlag = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.attachedRigidbody == bottomRb)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                // Vérifier si collision frontale
                if (collision.contacts[0].normal.x < -0.5f)  
                    bottomBlocked = true;
            }
        }

        if (collision.otherCollider.attachedRigidbody == topRb)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
                topBlocked = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.attachedRigidbody == bottomRb)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
                bottomBlocked = false;
        }

        if (collision.otherCollider.attachedRigidbody == topRb)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
                topBlocked = false;
        }
    }
}