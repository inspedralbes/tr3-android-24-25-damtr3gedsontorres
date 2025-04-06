using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu; // Referencia al menú de pausa
    public bool isPaused = false; // Estado de pausa

    void Update()
    {
        // Detectar la tecla de pausa (Escape o P)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    public void Pause()
    {
        Time.timeScale = 0f; // Detener el tiempo del juego
        pauseMenu.SetActive(true); // Mostrar el menú de pausa
        isPaused = true; // Cambiar el estado a pausado
    }
    public void Resume()
    {
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        pauseMenu.SetActive(false); // Ocultar el menú de pausa
        isPaused = false; // Cambiar el estado a no pausado
    }
}
