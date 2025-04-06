using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f; // Tiempo antes de destruirse
    public GameObject explosionEffect; // Efecto de explosión
    public Collider2D playerCollider; // Collider del jugador para ignorar colisión
    public int damage = 10; // Daño de la bala

    private float extraDamage = 0f; // Daño adicional por power-ups
    private bool isPowerUpActive = false; // Indica si el power-up está activo

    private void Start()
    {
        // Ignorar colisión entre la bala y el jugador
        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }

        Destroy(gameObject, lifetime); // La bala se destruye después de un tiempo
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala impacta un enemigo (con cualquier script de Enemy)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Impacto con enemigo: " + collision.gameObject.name); // Verifica si está entrando en la colisión

            // Intentamos obtener el script 'Enemy' en el objeto colisionado
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage + (int)extraDamage); // Aplica daño al enemigo, incluyendo daño extra del power-up
            }

            Explode(); // Genera explosión y destruye la bala
        }
        else if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Bullet"))
        {
            Explode(); // La bala explota si toca cualquier otro objeto que no sea jugador o otra bala
        }
    }

    void Explode()
    {
        // Crear efecto de explosión si está configurado
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Debug.Log("Explosion creada en: " + transform.position); // Verifica la posición
        }
        Destroy(gameObject); // Destruir la bala
    }

    // Método para activar el power-up de daño
    public void ActivatePowerUp(float extraDamageAmount, float duration)
    {
        extraDamage += extraDamageAmount; // Aumentar el daño extra
        isPowerUpActive = true;

        // Desactivar el power-up después de un tiempo determinado
        CancelInvoke(nameof(DeactivatePowerUp));
        Invoke(nameof(DeactivatePowerUp), duration);

        Debug.Log($"PowerUp activado! Daño extra: {extraDamage} por {duration} segundos");
    }

    // Método para desactivar el power-up de daño
    private void DeactivatePowerUp()
    {
        extraDamage = 0f; // Resetear el daño extra
        isPowerUpActive = false;
        Debug.Log("PowerUp terminado, daño extra reseteado.");
    }
}
