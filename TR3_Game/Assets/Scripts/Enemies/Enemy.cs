using UnityEngine;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    [Header("Estadísticas")]
    public string enemyName;
    public int health = 100;
    public int attack = 10;
    public float speed = 3f;
    public int reward = 20;
    public float detectionRadius = 7f;

    [Header("Referencias")]
    public GameObject explosionEffect;
    public AudioClip attackSound;
    public AudioClip deathSound;

    private Transform player;
    private bool isFollowing = false;
    private Rigidbody2D enemyRb;
    private Animator enemyAnim;
    private ConnectionManager connectionManager;
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Patrullaje")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private float patrolWaitTime = 2f;
    private float patrolTimer = 0f;

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        connectionManager = FindObjectOfType<ConnectionManager>();
        Debug.Log(enemyName);
        if (connectionManager != null)
            GetEnemyData(enemyName);
        else
            Debug.LogError("No se ha encontrado el ConnectionManager.");
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            isFollowing = distanceToPlayer <= detectionRadius;
        }
    }

    private void FixedUpdate()
    {
        if (isFollowing && player != null)
            FollowPlayer();
        else
            Patrol();
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        enemyRb.MovePosition(enemyRb.position + direction * speed * Time.fixedDeltaTime);
        enemyAnim.SetFloat("Horizontal", direction.x);
        enemyAnim.SetFloat("Vertical", direction.y);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        enemyRb.MovePosition(enemyRb.position + direction * speed * Time.fixedDeltaTime);
        enemyAnim.SetFloat("Horizontal", direction.x);
        enemyAnim.SetFloat("Vertical", direction.y);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            DonPolloController playerController = collision.gameObject.GetComponent<DonPolloController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attack);
                lastAttackTime = Time.time;
                PlaySound(attackSound);
            }
        }
    }

    private async void GetEnemyData(string name)
    {
        Debug.Log($"Cargando datos del enemigo: {name}");
        string endpoint = "/api/enemies/name/" + name;
        string response = await connectionManager.SendGetRequest(endpoint);
        Debug.Log($"Llamando al endpoint: {endpoint}");

        if (!string.IsNullOrEmpty(response))
        {
            EnemyData enemyData = JsonUtility.FromJson<EnemyData>(response);
            health = enemyData.health;
            attack = enemyData.attack;
            speed = enemyData.speed;
            reward = enemyData.reward;
            Debug.Log($"Datos cargados: Salud={health}, Ataque={attack}, Velocidad={speed}, Recompensa={reward}");
        }
        else
        {
            Debug.LogError("No se pudo obtener los datos del enemigo.");
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        PlaySound(deathSound);
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        foreach (Transform point in patrolPoints)
            Gizmos.DrawSphere(point.position, 0.2f);
    }

    [System.Serializable]
    public class EnemyData
    {
        public int health;
        public int attack;
        public float speed;
        public int reward;
    }
}
