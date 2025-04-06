using UnityEngine;
using System.Threading.Tasks;

public class PowerUpDamage : MonoBehaviour
{
    public string powerUpName;  // Nombre del power-up (para buscarlo en la base de datos)
    private float extraDamage;  // Daño adicional
    private float duration;     // Duración del power-up

    private async void Start()
    {
        // Obtener el extraDamage y duration desde la base de datos
        await GetDamagePowerUpFromDatabase();
    }

    // Método para obtener el extraDamage y duration desde la base de datos
    private async Task GetDamagePowerUpFromDatabase()
    {
        string endpoint = $"/api/objects/name/{powerUpName}";
        string response = await FindObjectOfType<ConnectionManager>().SendGetRequest(endpoint);

        if (response != null)
        {
            try
            {
                // Asumimos que la respuesta es un JSON con el power-up único
                PowerUpResponse powerUp = JsonUtility.FromJson<PowerUpResponse>(response);
                if (powerUp != null)
                {
                    extraDamage = powerUp.value;  // 'value' es el daño
                    duration = powerUp.duration;  // 'duration' es la duración
                    Debug.Log($"Daño del power-up: {extraDamage}, Duración: {duration}");
                }
                else
                {
                    Debug.LogError("No se encontró el power-up en la base de datos.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al parsear la respuesta: " + ex.Message);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BulletController bulletController)) // Detectar si colisiona con el jugador
        {
            bulletController.ActivatePowerUp(GetExtraDamage(), GetDuration()); // Pasar el daño y duración al controlador de balas
            Destroy(gameObject); // Destruir el power-up después de activarse
        }
    }

    // Métodos públicos para acceder a extraDamage y duration
    public float GetExtraDamage() => extraDamage;
    public float GetDuration() => duration;

    // Estructura para mapear la respuesta del servidor
    [System.Serializable]
    public class PowerUpResponse
    {
        public string name;
        public float value;  // Asumimos que 'value' es el daño
        public float duration; // Duración del power-up
    }
}
