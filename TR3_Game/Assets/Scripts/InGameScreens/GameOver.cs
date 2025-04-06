using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    
    public GameObject gameOverPanel;

    public void mostrarGameOver()
    {
        gameOverPanel.SetActive(true);
        // textPuntos.text = "Puntos: " + PlayerPrefs.GetInt("score", 0).ToString();
    }

    public void reiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameOverPanel.SetActive(false);
    }

    public void irAlMenuPrincipal()
    {
        SceneManager.LoadScene("InitialScreen"); // Cambia "MenuPrincipal" por el nombre de tu escena de menú principal
    }
}
