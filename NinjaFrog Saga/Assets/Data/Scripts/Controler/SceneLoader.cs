using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Cenas Base do Jogo")]
    [Tooltip("Nomes das cenas que formam o jogo principal (mapa, jogador, etc.)")]
    public List<string> coreGameScenes;

    // Usamos Awake para garantir que execute antes de qualquer Start
    void Awake()
    {
        // Carrega todas as cenas essenciais do jogo
        foreach (string sceneName in coreGameScenes)
        {
            // Verifica se a cena já não está carregada (útil no editor)
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}
