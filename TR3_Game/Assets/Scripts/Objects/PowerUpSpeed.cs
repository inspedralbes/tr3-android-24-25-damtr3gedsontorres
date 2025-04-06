using UnityEngine;
using System.Threading.Tasks;

public class PowerUpSpeed : MonoBehaviour
{
    public string powerUpName;  // Nombre del power-up (para buscarlo en la base de datos)
    private float speedIncrease; // Cuánto aumenta la velocidad
    private float duration;      // Duración del power-up

    private async void Start()
    {
        // Obtener el speedIncrease y duration desde la base de datos
        await GetSpeedPowerUpFromDatabase();
    }

    // Método para obtener el speedIncrease y duration desde la base de datos
    private async Task GetSpeedPowerUpFromDatabase()
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
                    speedIncrease = powerUp.value;  // 'value' es el aumento de velocidad
                    duration = powerUp.duration;    // 'duration' es la duración
                    Debug.Log($"Incremento de velocidad: {speedIncrease}, Duración: {duration}");
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

    // Métodos públicos para obtener el speedIncrease y duration
    public float GetSpeedIncrease() => speedIncrease;
    public float GetDuration() => duration;

    // Estructura para mapear la respuesta del servidor
    [System.Serializable]
    public class PowerUpResponse
    {
        public string name;
        public float value;  // Asumimos que 'value' es el aumento de velocidad
        public float duration; // Duración del power-up
    }
}
