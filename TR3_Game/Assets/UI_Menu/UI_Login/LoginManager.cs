using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class LoginManager : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button loginButton;
    private Button goToRegisterButton;
    private TextField emailField;
    private TextField passwordField;
    private Label errorMessageLabel;
    
    private ConnectionManager connectionManager;
    private MenuManager menuManager; // Referencia a MenuManager para cambiar de pantalla

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        connectionManager = FindObjectOfType<ConnectionManager>();
        menuManager = FindObjectOfType<MenuManager>(); // Buscar el gestor de pantallas
    }

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        
        loginButton = root.Q<Button>("login-button");
        goToRegisterButton = root.Q<Button>("go-to-register-button");
        emailField = root.Q<TextField>("user-field");
        passwordField = root.Q<TextField>("password-field");

        // Crear un mensaje de error dinámico si no existe
        errorMessageLabel = root.Q<Label>("error-message") ?? new Label();
        errorMessageLabel.style.color = new Color(1, 0, 0); // Rojo
        errorMessageLabel.style.display = DisplayStyle.None;
        root.Add(errorMessageLabel);

        loginButton.clicked += OnLoginButtonClicked;
    }

    private async void OnLoginButtonClicked()
    {
        string email = emailField.value;
        string password = passwordField.value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorMessage("Por favor, completa todos los campos.");
            return;
        }

        Debug.Log($"Intentando iniciar sesión con: {email}");

        var loginData = new LoginData
        {
            username = email,
            password = password
        };

        // Realizar la petición al backend
        var response = await connectionManager.SendPostRequest("/api/player-auth/login", loginData);

        if (!string.IsNullOrEmpty(response) && response.Contains("token"))
        {
            Debug.Log("Inicio de sesión exitoso.");

            try
            {
                // Intentamos deserializar la respuesta del servidor
                var jsonResponse = JsonUtility.FromJson<LoginResponse>(response);
                if (jsonResponse != null && jsonResponse.token != null)
                {
                    // Guardar el token y el email en PlayerPrefs
                    PlayerPrefs.SetString("auth_token", jsonResponse.token);
                    PlayerPrefs.SetString("user_email", jsonResponse.player.email); // Guardar el email
                    PlayerPrefs.Save();

                    Debug.Log("Email guardado en PlayerPrefs: " + jsonResponse.player.email);

                    // 🚀 CAMBIAR A LA SIGUIENTE PANTALLA SOLO SI EL LOGIN ES VÁLIDO
                    menuManager.CambiarPantalla(menuManager.selectScreenUI);
                }
                else
                {
                    ShowErrorMessage("Error en los datos de la respuesta.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al deserializar la respuesta: " + ex.Message);
                ShowErrorMessage("Error al procesar la respuesta del servidor.");
            }
        }
        else
        {
            ShowErrorMessage("Credenciales incorrectas o no autorizadas.");
            Debug.LogError("Error al iniciar sesión.");
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageLabel.text = message;
        errorMessageLabel.style.display = DisplayStyle.Flex;
    }
}

// Clase para manejar los datos del login
[System.Serializable]
public class LoginData
{
    public string username;
    public string password;
}

// Clase para deserializar la respuesta del servidor
[System.Serializable]
public class LoginResponse
{
    public string token;
    public UserData player;
}

[System.Serializable]
public class UserData
{
    public string email;
    public string username;
}
