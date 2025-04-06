using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SelectScreen : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button playButton;
    private Button settingButton;
    private Button goBackButton;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        playButton = root.Q<Button>("playButton");
        settingButton = root.Q<Button>("settingButton");
        goBackButton = root.Q<Button>("goBackButton");

        playButton.clicked += OnPlayButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Game"); // Reemplaza "GameScene" con el nombre real de la escena
    }
}
