using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody2D body;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [SerializeField]
    float impulseMagnitude;

    private void OnEnable()
    {
        GameLogic.OnLevelCleared += OnLevelCleared;
    }

    private void OnDisable()
    {
        GameLogic.OnLevelCleared -= OnLevelCleared;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Let's keep balls on the same velocity no matter what.
        // Sometimes their speed may drop because they are hitting other balls
        // at different angles. Even though they share the same speed and mass
        // in the beginning, they may end up having different speeds,
        // that was quite suprising to me!

        // You can experiment with collisions here:
        // https://phet.colorado.edu/sims/html/collision-lab/latest/collision-lab_en.html
        float tolerance = 0.5f;
        float impulseSquare = impulseMagnitude * impulseMagnitude;
        if (Mathf.Abs(body.velocity.sqrMagnitude - impulseSquare) > tolerance)
        {
            body.velocity = body.velocity.normalized * impulseMagnitude;
        }
    }

    public void Fire(Vector2 direction)
    {
        body.isKinematic = false;
        if (transform.parent != null)
            transform.parent = transform.parent.parent; // Free it from the pad-parent.
        Vector2 impulse = impulseMagnitude * direction;
        body.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void GlueToPad(GameObject pad)
    {
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0f;
        transform.parent = pad.transform; // Bind to pad, so that it moves with it.
    }

    public Vector3 CenterOnPad(GameObject pad)
    {
        Vector3 ballSize = spriteRenderer.bounds.size;
        Vector3 padSize = pad.GetComponent<SpriteRenderer>().bounds.size;

        float xRelativeToPad = 0f;
        float yRelativeToPad = (padSize.y + ballSize.y) / 2f;

        return new Vector3(xRelativeToPad, yRelativeToPad, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AdjustVelocity(body.velocity);
        }
    }

    private void AdjustVelocity(Vector2 velocity)
    {
        const float ADJUST_THRESHOLD = 4f;
        const float ADJUST_AMOUNT = 10f;

        Vector2[] axes = { Vector2.down, Vector2.right, Vector3.left };

        foreach (Vector2 ax in axes)
        {
            float angle = Vector2.SignedAngle(ax, velocity);

            if (Mathf.Abs(angle) < ADJUST_THRESHOLD)
            {
                var adjustedAngle = ADJUST_AMOUNT * Mathf.Sign(angle);

                // rotate velocity `angle` degrees counterclockwise along Z axis:
                Vector2 newVelDirection = Quaternion.Euler(0, 0, adjustedAngle) * body.velocity.normalized;
                Vector2 newVelocity = body.velocity.magnitude * newVelDirection;
                body.velocity = newVelocity;
                return;
            }
        }
    }

    public void OnLevelCleared(int _level)
    {
        body.velocity /= 3f;
        animator.enabled = true;
        animator.Play("BallWinsAnim", -1, 0f);
    }
}
