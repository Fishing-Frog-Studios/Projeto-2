using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
{
    [Header("Configurações da Cena de UI")]
    public string uiSceneName;

    [Header("Input Action")]
    public InputActionReference toggleAction;

    // Removemos a variável 'isUiSceneOpen'. Vamos checar o estado real da cena.

    private void OnEnable()
    {
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.Disable();
    }

    void Update()
    {
        if (toggleAction.action.WasPressedThisFrame())
        {
            ToggleUiScene();
        }
    }

    public void ToggleUiScene()
    {
        // <<< A GRANDE MUDANÇA ESTÁ AQUI >>>
        // Em vez de usar uma variável booleana, verificamos diretamente se a cena está carregada.
        if (IsUiSceneLoaded())
        {
            // Se já está carregada, fecha.
            CloseUiScene();
        }
        else
        {
            // Se não está carregada, abre.
            OpenUiScene();
        }
    }

    // <<< NOVA FUNÇÃO HELPER >>>
    /// <summary>
    /// Verifica se a cena de UI com o nome especificado já está carregada.
    /// </summary>
    private bool IsUiSceneLoaded()
    {
        // Percorre todas as cenas carregadas.
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            // Se encontrar uma cena com o nome correspondente, retorna verdadeiro.
            if (scene.name == uiSceneName)
            {
                return true;
            }
        }
        // Se o loop terminar e não encontrar, retorna falso.
        return false;
    }

    private void OpenUiScene()
    {
        if (string.IsNullOrEmpty(uiSceneName))
        {
            Debug.LogError("O nome da cena de UI não foi definido!");
            return;
        }
        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
        Time.timeScale = 0f;
        Debug.Log($"Cena '{uiSceneName}' aberta. Jogo pausado.");
    }

    private void CloseUiScene()
    {
        if (string.IsNullOrEmpty(uiSceneName))
        {
            Debug.LogError("O nome da cena de UI não foi definido!");
            return;
        }
        // Chama UnloadSceneAsync para fechar TODAS as instâncias da cena com este nome.
        SceneManager.UnloadSceneAsync(uiSceneName);
        Time.timeScale = 1f;
        Debug.Log($"Cena '{uiSceneName}' fechada. Jogo despausado.");
    }
}
