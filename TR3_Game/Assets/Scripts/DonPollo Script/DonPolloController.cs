using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DonPolloController : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float speed = 5f;

    public GameObject finalScreenCanvas;

    public float powerUpTime;
    public float powerUpSpeed;
    private bool isSpeedPowerUpActive = false;

    [Header("Efectos de Partículas")]
    public HealthBar healthBar;
    public GameObject hitEffect;

    private Rigidbody2D donPolloRb;
    private Animator donPolloAnim;
    private Vector2 direction;
    private float axisX;
    private float axisY;

    // Variables para estadísticas de la partida
    private float startTime;
    private int enemiesDefeated = 0;
    private int bulletsFired = 0;
    private string userEmail;

    private void Start()
    {
        donPolloRb = GetComponent<Rigidbody2D>();
        donPolloAnim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Capturar el tiempo de inicio de la partida
        startTime = Time.time;

        // Obtener el email del jugador desde PlayerPrefs
        userEmail = PlayerPrefs.GetString("user_email", "desconocido@example.com");
    }

    private void Update()
    {
        axisX = Input.GetAxisRaw("Horizontal");
        axisY = Input.GetAxisRaw("Vertical");
        direction = new Vector2(axisX, axisY).normalized;

        donPolloAnim.SetFloat("Horizontal", axisX);
        donPolloAnim.SetFloat("Vertical", axisY);

        if (axisX != 0 || axisY != 0)
        {
            donPolloAnim.SetFloat("LastX", axisX);
            donPolloAnim.SetFloat("LastY", axisY);
        }
    }

    private void FixedUpdate()
    {
        donPolloRb.MovePosition(donPolloRb.position + direction * speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (donPolloAnim != null)
        {
            donPolloAnim.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (donPolloAnim != null)
        {
            donPolloAnim.SetBool("IsDead", true);
        }

        this.enabled = false;

        if (donPolloRb != null)
        {
            donPolloRb.linearVelocity = Vector2.zero;
            donPolloRb.isKinematic = true;
        }

        // Registrar los datos de la partida en el servidor
        FindAnyObjectByType<GameOver>().mostrarGameOver();
        SaveGameData();
        // SceneManager.LoadScene("FinalScreen"); // Cambia "GameOverScene" por el nombre de tu escena de Game Over
    }

    // Método para contar enemigos eliminados (llámalo en el script de los enemigos cuando mueran)
    public void AddEnemyDefeated()
    {
        enemiesDefeated++;
    }

    // Método para contar balas disparadas (llámalo en el script de disparo)
    public void AddBulletFired()
    {
        bulletsFired++;
    }

    public void ActivateSpeedPowerUp(float extraSpeed, float duration)
    {
        if (!isSpeedPowerUpActive)
        {
            powerUpSpeed = extraSpeed;
            speed += extraSpeed;
            isSpeedPowerUpActive = true;
            Debug.Log($"PowerUp de velocidad activado! Nueva velocidad: {speed} por {duration} segundos");
            CancelInvoke(nameof(DeactivateSpeedPowerUp));
            Invoke(nameof(DeactivateSpeedPowerUp), duration);
        }
    }

    private void DeactivateSpeedPowerUp()
    {
        speed -= powerUpSpeed;
        isSpeedPowerUpActive = false;
        Debug.Log("PowerUp de velocidad terminado, velocidad restaurada.");
    }

    // Enviar datos al backend
    private async void SaveGameData()
    {
        float playTime = Time.time - startTime;
        DateTime lastLogin = DateTime.Now; // Última vez que jugó

        var gameData = new
        {
            play_time = Mathf.RoundToInt(playTime),
            enemies_defeated = enemiesDefeated,
            user_email = userEmail,
            last_login = lastLogin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            bullets_used = bulletsFired,
            // scoreGame = score
        };

        // Guardar los datos en PlayerPrefs
        PlayerPrefs.SetInt("playTime", Mathf.RoundToInt(playTime));
        PlayerPrefs.SetInt("enemiesDefeated", enemiesDefeated);
        PlayerPrefs.SetInt("bulletsFired", bulletsFired);
        PlayerPrefs.SetString("lastLogin", lastLogin.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        // PlayerPrefs.SetInt("score", score);

        // También puedes guardar el correo electrónico si es necesario
        PlayerPrefs.SetString("userEmail", userEmail);

        PlayerPrefs.Save();  // Asegúrate de guardar los datos

        ConnectionManager connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager != null)
        {
            string response = await connectionManager.SendPostRequest("/api/mongo/games", gameData);
            Debug.Log("Datos de la partida enviados: " + response);
        }
        else
        {
            Debug.LogError("No se encontró ConnectionManager en la escena.");
        }
    }
    
private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("FinalItem")) 
    {
        Debug.Log("Objeto final recogido. Mostrando pantalla final.");
        finalScreenCanvas.SetActive(true); 
        this.enabled = false; 
    }
    else if (collision.gameObject.CompareTag("Enemy")) // Asegúrate de que el enemigo tenga este tag
    {
        Debug.Log("¡Colisión con el enemigo!");
        TakeDamage(10); // Resta 10 puntos de vida al personaje (ajustar el daño)
    }
    else if (collision.gameObject.TryGetComponent(out PowerUpSpeed powerUp))
    {
        ActivateSpeedPowerUp(powerUp.GetSpeedIncrease(), powerUp.GetDuration());
        Destroy(collision.gameObject);
    }
}


}
