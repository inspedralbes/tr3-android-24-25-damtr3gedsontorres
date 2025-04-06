using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public int bulletDamage = 10;
    
    private Collider2D playerCollider;
    private Vector2 lastDirection = Vector2.right;
    
    public float extraDamage = 0f;
    public float powerUpTime = 0f;
    private bool isPowerUpActive = false;

    private DonPolloController donPolloController;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
            donPolloController = player.GetComponent<DonPolloController>(); // Obtener referencia al script de control del jugador
        }
    }

    void Update()
    {
        float axisX = Input.GetAxisRaw("Horizontal");
        float axisY = Input.GetAxisRaw("Vertical");

        if (axisX != 0 || axisY != 0)
        {
            lastDirection = new Vector2(axisX, axisY).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = lastDirection * bulletSpeed;

        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        if (bulletCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, playerCollider);
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = GetCurrentBulletDamage();
            Debug.Log($"Disparando bala con daño: {bulletScript.damage}");
        }

        // Sumar una bala disparada en DonPolloController
        if (donPolloController != null)
        {
            donPolloController.AddBulletFired();
        }
        else
        {
            Debug.LogWarning("DonPolloController no encontrado en el jugador.");
        }
    }

    public void ActivatePowerUp(float extraDamageAmount, float duration)
    {
        extraDamage += extraDamageAmount;
        powerUpTime = duration;
        isPowerUpActive = true;
        CancelInvoke(nameof(DeactivatePowerUp));
        Invoke(nameof(DeactivatePowerUp), powerUpTime);
        Debug.Log($"PowerUp activado! Daño extra: {extraDamage} por {powerUpTime} segundos");
    }

    private void DeactivatePowerUp()
    {
        Debug.Log("PowerUp terminado, daño extra reseteado.");
        extraDamage = 0f;
        isPowerUpActive = false;
    }

    private int GetCurrentBulletDamage()
    {
        return bulletDamage + (int)extraDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PowerUpDamage powerUp))
        {
            float powerUpDamage = powerUp.GetExtraDamage();
            float powerUpDuration = powerUp.GetDuration();

            ActivatePowerUp(powerUpDamage, powerUpDuration);
            Destroy(other.gameObject);
        }
    }
}
