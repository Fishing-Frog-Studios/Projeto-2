using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]
public class SlotUI
{

    public GameObject Vazio;
    public GameObject Preenchido;
    public GameObject Mais;
    public GameObject Imagem_Fase;
    public GameObject Delete;
    public TMP_Text textPorcentagem;
    public TMP_Text textLocalSave;
    public TMP_Text textHora;
    public TMP_Text textTempoDeJogo;
    public GameObject Carregar;
    public GameObject Duplicar;

    [SerializeField] Text dataDisplay;

}
public class slotsUI : MonoBehaviour
{


    public SlotUI[] slots;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {

        AtualizaSlot();
    }



    void AtualizaTela()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "save_" + i + ".json");

            if (File.Exists(caminho))
            {
                slots[i].Vazio.SetActive(true);
                slots[i].Preenchido.SetActive(true);

                slots[i].Mais.SetActive(true);
                slots[i].Imagem_Fase.SetActive(true);
                slots[i].Delete.SetActive(true);
                slots[i].textPorcentagem.text = "100%";
                slots[i].textLocalSave.text = "Local do Salvamento:AppData/LocalLow/Fishing Frog Studios/Frog Ninja Odissey";
                slots[i].textHora.text = "Hora: 27.07.2025 - 12h14";
                slots[i].textTempoDeJogo.text = "Tempo de Jogo: 12h30min";
                slots[i].Carregar.SetActive(true);
                slots[i].Duplicar.SetActive(true);
            }
            else
            {
                slots[i].Preenchido.SetActive(false);
                slots[i].Vazio.SetActive(true);
            }
        }
    }

    public void AtualizaSlot()
    {
        
        for (int i = 0; i < slots.Length; i++)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "save_" + i + ".json");
            if (File.Exists(caminho))
            {

                slots[i].Preenchido.SetActive(true);
                slots[i].Vazio.SetActive(false);

                string json = File.ReadAllText(caminho);
                DadosDoSave dados = JsonUtility.FromJson<DadosDoSave>(json);

                slots[i].textLocalSave.text = Application.persistentDataPath;
                //anotado aqui um exemplo de como colocar informacoes no menu, que nao é necessario agr mas pode ajudar dps ainda,
                //caso esqueça.
                //slots[i].OqVoceQuer.text (pra converter pra string) = oq vc quer passar pra string;
                //slots[i].textLocalSave.text = dados.nome;
                slots[i].textTempoDeJogo.text = "Tempo de Jogo: " + System.TimeSpan.FromSeconds(dados.tempoDeJogo).ToString();
                slots[i].textHora.text = "Data: " + dados.dataAtual;
            }
            else
            {
                slots[i].Preenchido.SetActive(false);
                slots[i].Vazio.SetActive(true);
            }
        }
    }

    public void DELETAR(int slotIndex)
    {

        string caminho = Path.Combine(Application.persistentDataPath, "save_" + slotIndex + ".json");
        if (File.Exists(caminho))
        {

            File.Delete(caminho);
            print("DELETOOOO");
        }
        AtualizaSlot();
    }

    public void Mais(int slotIndex)
    {

        // Criar a "ficha" de save
        DadosDoSave dados = new DadosDoSave();


        dados.posicaoX = 0;
        dados.posicaoY = 0;


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
        string caminho = Path.Combine(Application.persistentDataPath, "save_" + slotIndex + ".json");
        File.WriteAllText(caminho, json);

        // debug pra mostrar essa droga
        Debug.Log("Jogo Criado!");

        AtualizaSlot();
   
}
    
}


