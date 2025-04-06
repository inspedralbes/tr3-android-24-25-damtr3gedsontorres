using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class ConnectionManager : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private string baseUrl = "http://187.33.149.132:3000";  // Cambia esto a la URL de tu servidor real

    // Método para realizar una solicitud POST
    public async Task<string> SendPostRequest<T>(string endpoint, T data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            Debug.Log("JSON enviado: " + json);

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            string fullUrl = baseUrl + endpoint;
            Debug.Log("URL completa: " + fullUrl);

            HttpResponseMessage response = await client.PostAsync(fullUrl, content);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return responseText;
            }
            else
            {
                Debug.LogError($"Error en la solicitud: {response.StatusCode} - Respuesta del servidor: {responseText}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en la conexión: " + ex.Message);
            return null;
        }
    }

    // Método para realizar una solicitud GET
    public async Task<string> SendGetRequest(string endpoint)
    {
        try
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            string fullUrl = baseUrl + endpoint;
            Debug.Log("URL completa: " + fullUrl);

            HttpResponseMessage response = await client.GetAsync(fullUrl);
            string responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return responseText;
            }
            else
            {
                Debug.LogError($"Error en la solicitud: {response.StatusCode} - Respuesta del servidor: {responseText}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en la conexión: " + ex.Message);
            return null;
        }
    }
}
