using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class SaveManager : MonoBehaviour
{

    public GameObject painelPausa;
    private bool jogoPausado = false;




    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (jogoPausado)
            {
                ContinuarJogo();

            }
            else
            {
                PausarJogo();

            }
        }
    }

    void PausarJogo()
    {
        painelPausa.SetActive(true);
        Time.timeScale = 0f;
        jogoPausado = true;
    }

    public void ContinuarJogo()
    {
        painelPausa.SetActive(false);
        Time.timeScale = 1f;
        jogoPausado = false;
    }




    //eu gosto de espaço owo


    public void saveInGame()
    {
        if (DataPlayer.Instace.SlotAtual >= 0 && DataPlayer.Instace.SlotAtual <= 2)
        {

            SaveGame(DataPlayer.Instace.SlotAtual);
        }
        else
        {
            SaveGame(0);
        }

    }

    public void SaveGame(int slotIndex)
    {

        // Verificar se o Sapo foi encontrado
        if (PlayerControllerMOdificado.Instance != null)
        {
            // Criar a "ficha" de save
            DadosDoSave dados = new DadosDoSave();


            dados.posicaoX = PlayerControllerMOdificado.Instance.transform.position.x;
            dados.posicaoY = PlayerControllerMOdificado.Instance.transform.position.y;


            dados.nome = DataPlayer.Instace.nome;
            dados.level = DataPlayer.Instace.level;
            dados.Vida = DataPlayer.Instace.Vida;
            dados.Mana = DataPlayer.Instace.Mana;
            dados.ouro = DataPlayer.Instace.ouro;
            dados.moveSpeed = DataPlayer.Instace.moveSpeed;
            dados.ataqueSpeed = DataPlayer.Instace.ataqueSpeed;
            dados.dano = DataPlayer.Instace.dano;
            dados.tempoDeJogo = DataPlayer.Instace.tempoDeJogo;
            

            //dados.dataAtual = DataPlayer.Instace.dataAtual;
            DataPlayer.Instace.dataAtual = dados.dataAtual = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            // traduzir paraa JSON e salvar
            string json = JsonUtility.ToJson(dados, true);
            //string caminho = Path.Combine(Application.persistentDataPath, "save.json");
            string caminho = Path.Combine(Application.persistentDataPath, "save_" + slotIndex + ".json");
            File.WriteAllText(caminho, json);

            // debug pra mostrar essa droga
            Debug.Log("JOGO SALVO! Posição salva: (" + dados.posicaoX + ", " + dados.posicaoY + ")");
        }
        else
        {
            Debug.LogError("SaveGame ERRO: Sapo (PlayerController) não encontrado na cena!");
        }
    }



    public void LoadGame(int slotIndex)
    {
        //string caminho = Path.Combine(Application.persistentDataPath, "save.json");
        string caminho = Path.Combine(Application.persistentDataPath, "save_" + slotIndex + ".json");

        if (File.Exists(caminho))
        {
            string json = File.ReadAllText(caminho);
            DadosDoSave dados = JsonUtility.FromJson<DadosDoSave>(json);

            // Atualiza as variáveis seguras no DataPlayer.Instace
            DataPlayer.Instace.posicaoSalvaX = dados.posicaoX;
            DataPlayer.Instace.posicaoSalvaY = dados.posicaoY;

            // Atualiza o resto dos dados
            DataPlayer.Instace.nome = dados.nome;
            DataPlayer.Instace.level = dados.level;
            DataPlayer.Instace.Vida = dados.Vida;
            DataPlayer.Instace.Mana = dados.Mana;
            DataPlayer.Instace.ouro = dados.ouro;
            DataPlayer.Instace.moveSpeed = dados.moveSpeed;
            DataPlayer.Instace.ataqueSpeed = dados.ataqueSpeed;
            DataPlayer.Instace.dano = dados.dano;
            DataPlayer.Instace.tempoDeJogo = dados.tempoDeJogo;
            DataPlayer.Instace.SlotAtual = slotIndex;

            //DataPlayer.Instace.dataAtual = dados.dataAtual;
            DataPlayer.Instace.dataAtual = dados.dataAtual = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


            Time.timeScale = 1f;

            SceneManager.LoadScene("Characters");
        }
        else
        {
            Debug.Log("Nenhum arquivo de save encontrado.");
        }
    }

    
    
}
