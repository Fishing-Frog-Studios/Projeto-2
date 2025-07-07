using UnityEngine;
using UnityEngine.SceneManagement; // Importante para carregar outras cenas

public class UIManager : MonoBehaviour
{
    // Função para fechar a tela atual e voltar ao jogo.
    // Presume que a "cena do jogo" se chama "NPC_andando", ajuste se for outro nome.
    public void BackToGame()
    {
        Debug.Log("Voltando para a cena do jogo...");
        // A forma mais simples é desativar o Canvas atual.
        // Isso funciona se a sua cena de jogo e a de UI forem a mesma.
        // gameObject.SetActive(false); 

        // Se a SkillTree for uma cena separada, você precisa carregar a cena do jogo:
        SceneManager.LoadScene("NPC_andando"); 
    }

    // Função para navegar para a tela de Inventário
    public void GoToInventory()
    {
        Debug.Log("Navegando para o Inventário...");
        // Aqui você carregaria a cena do Inventário, se ela existir.
        // SceneManager.LoadScene("InventoryScene"); 
    }

    // Função para navegar para a tela de Mapa
    public void GoToMap()
    {
        Debug.Log("Navegando para o Mapa...");
        // Aqui você carregaria a cena do Mapa, se ela existir.
        // SceneManager.LoadScene("MapScene"); 
    }
}
