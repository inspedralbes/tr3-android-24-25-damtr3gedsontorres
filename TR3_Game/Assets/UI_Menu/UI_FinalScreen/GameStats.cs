using UnityEngine;
using UnityEngine.UIElements;

public class GameStats : MonoBehaviour
{
    private Label playTimeLabel;
    private Label enemiesDefeatedLabel;
    private Label bulletsUsedLabel;
    private Label lastLoginLabel;

    void Start()
    {
        // Obtener la referencia de las etiquetas
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        
        playTimeLabel = rootVisualElement.Q<Label>("DynamicText1");
        enemiesDefeatedLabel = rootVisualElement.Q<Label>("DynamicText2");
        bulletsUsedLabel = rootVisualElement.Q<Label>("DynamicText3");
        lastLoginLabel = rootVisualElement.Q<Label>("DynamicText4");

        // Recuperar los datos de PlayerPrefs
        int playTime = PlayerPrefs.GetInt("playTime", 0);
        int enemiesDefeated = PlayerPrefs.GetInt("enemiesDefeated", 0);
        int bulletsFired = PlayerPrefs.GetInt("bulletsFired", 0);
        string lastLogin = PlayerPrefs.GetString("lastLogin", "00/00/0000");
        
        // Actualizar las etiquetas con los datos
        playTimeLabel.text = "Tiempo Jugado: " + playTime + " seg";
        enemiesDefeatedLabel.text = "Enemigos Derrotados: " + enemiesDefeated;
        bulletsUsedLabel.text = "Balas Usadas: " + bulletsFired;
        lastLoginLabel.text = "Último Acceso: " + lastLogin;
    }
}
