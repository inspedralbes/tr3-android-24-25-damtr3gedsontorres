using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class RegisterPage : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button registerButton;
    private Button backButton;
    private TextField usernameField;
    private TextField emailField;
    private TextField passwordField;
    private Label errorMessageLabel;
    
    private ConnectionManager connectionManager;
    private MenuManager menuManager;
    
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        connectionManager = FindObjectOfType<ConnectionManager>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        
        registerButton = root.Q<Button>("registerButton");
        backButton = root.Q<Button>("backButton");
        usernameField = root.Q<TextField>("usernameField");
        emailField = root.Q<TextField>("emailField");
        passwordField = root.Q<TextField>("passwordField");
        
        errorMessageLabel = root.Q<Label>("error-message") ?? new Label();
        errorMessageLabel.style.color = new Color(1, 0, 0);
        errorMessageLabel.style.display = DisplayStyle.None;
        root.Add(errorMessageLabel);

        registerButton.clicked += OnRegisterButtonClicked;
        backButton.clicked += () => menuManager.CambiarPantalla(menuManager.loginUI);
    }

    private async void OnRegisterButtonClicked()
    {
        string username = usernameField.value;
        string email = emailField.value;
        string password = passwordField.value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorMessage("Todos los campos son obligatorios.");
            return;
        }

        Debug.Log($"Intentando registrar usuario: {username}");

        var registerData = new RegisterData
        {
            username = username,
            email = email,
            password = password
        };

        var response = await connectionManager.SendPostRequest("/api/player-auth/register", registerData);
        Debug.Log(response);
        if (!string.IsNullOrEmpty(response))

        
        {
            if (response.Contains("error"))
            {
                ShowErrorMessage("El usuario o email ya están en uso.");
                Debug.LogError("Error al registrar usuario: " + response);
            }
            else
            {
                Debug.Log("Registro exitoso: " + response);
                menuManager.CambiarPantalla(menuManager.loginUI);
            }
        }
        else
        {
            ShowErrorMessage("Error al registrar. Inténtalo de nuevo.");
            Debug.LogError("Error al registrar usuario.");
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageLabel.text = message;
        errorMessageLabel.style.display = DisplayStyle.Flex;
    }
}

[System.Serializable]
public class RegisterData
{
    public string username;
    public string email;
    public string password;
}