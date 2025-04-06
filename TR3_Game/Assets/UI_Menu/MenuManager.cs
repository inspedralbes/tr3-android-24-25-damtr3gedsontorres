using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public UIDocument initialScreenUI; // Pantalla inicial
    public UIDocument loginUI;         // Pantalla de login
    public UIDocument registerUI;      // Pantalla de registro
    public UIDocument selectScreenUI;  // Pantalla de selección (después del login)
    
    private void Start()
    {
        // Ocultar todas las pantallas inicialmente
        OcultarPantallas();
        
        // Mostrar la pantalla inicial
        CambiarPantalla(initialScreenUI);
        
        // Configurar los botones
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (initialScreenUI != null)
        {
            var root = initialScreenUI.rootVisualElement;
            Button nextScreenButton = root.Q<Button>("nextScreenButton");
            if (nextScreenButton != null)
            {
                nextScreenButton.clicked += () => CambiarPantalla(loginUI);
            }
        }
        
        if (loginUI != null)
        {
            var root = loginUI.rootVisualElement;
            Button goToRegisterButton = root.Q<Button>("go-to-register-button");
            if (goToRegisterButton != null)
            {
                goToRegisterButton.clicked += () => CambiarPantalla(registerUI);
            }
        }
        
        if (registerUI != null)
        {
            var root = registerUI.rootVisualElement;
            Button backToLoginButton = root.Q<Button>("back-to-login-button");
            if (backToLoginButton != null)
            {
                backToLoginButton.clicked += () => CambiarPantalla(loginUI);
            }
        }
        
        if (selectScreenUI != null)
        {
            var root = selectScreenUI.rootVisualElement;
            Button backButton = root.Q<Button>("goBackButton");
            if (backButton != null)
            {
                backButton.clicked += () => CambiarPantalla(loginUI);
            }
        }
    }
    
    // Método para ocultar todas las pantallas
    void OcultarPantallas()
    {
        if (initialScreenUI != null)
            initialScreenUI.rootVisualElement.style.display = DisplayStyle.None;
        
        if (loginUI != null)
            loginUI.rootVisualElement.style.display = DisplayStyle.None;
        
        if (registerUI != null)
            registerUI.rootVisualElement.style.display = DisplayStyle.None;
        
        if (selectScreenUI != null)
            selectScreenUI.rootVisualElement.style.display = DisplayStyle.None;
    }
    
    // Método público para cambiar de pantalla
    public void CambiarPantalla(UIDocument nuevaPantalla)
    {
        OcultarPantallas();
        
        if (nuevaPantalla != null)
        {
            nuevaPantalla.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }
}
