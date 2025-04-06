using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private int health = 100;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float detectionRadius = 7f;
    private Rigidbody2D slimeRb;
    private Animator slimeAnim;
    private Transform player;
    private bool isFollowing = false;

    [Header("Patrullaje")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private float patrolWaitTime = 2f;
    private float patrolTimer = 0f;

    private void Start()
    {
        slimeRb = GetComponent<Rigidbody2D>();
        slimeAnim = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        // Detecta si el jugador está dentro del radio de detección
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            isFollowing = distanceToPlayer <= detectionRadius;
        }
    }

    private void FixedUpdate()
    {
        if (isFollowing && player != null)
        {
            FollowPlayer();
        }
        else
        {
            Patrol(); // Realiza el patrullaje si no está siguiendo al jugador
        }
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        slimeRb.MovePosition(slimeRb.position + direction * speed * Time.fixedDeltaTime);
        slimeAnim.SetFloat("Horizontal", direction.x);
        slimeAnim.SetFloat("Vertical", direction.y);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        slimeRb.MovePosition(slimeRb.position + direction * speed * Time.fixedDeltaTime);
        slimeAnim.SetFloat("Horizontal", direction.x);
        slimeAnim.SetFloat("Vertical", direction.y);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            patrolTimer += Time.fixedDeltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Radio de detección

        Gizmos.color = Color.blue;
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawSphere(point.position, 0.2f); // Muestra los puntos de patrullaje
        }
    }
}
