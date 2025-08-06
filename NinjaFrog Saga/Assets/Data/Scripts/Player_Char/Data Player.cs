using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DataPlayer : MonoBehaviour
{
    public static DataPlayer Instace { get; private set; }

    //madao colocou essas duas variaveis pra pegar a posicao do sapo pra salvar dps.
    public float posicaoSalvaX = 0;
    public float posicaoSalvaY = 0;


    // Status do Jogador

    [Header("Nome do Personagem")]
    public string nome = "Werbet";

    [Header("Level do Jogador")]
    public int level = 0;

    [Header("Vida Maxima")]
    public float Vida = 1f;

    [Header("Mana Maxima")]
    public float Mana = 1f;

    [Header("Quantidade de Ouro")]
    public int ouro = 0;

    [Header("Velocidade do jogador")]
    public float moveSpeed = 1f;

    [Header("velocidade de ataque")]
    public float ataqueSpeed = 0.5f;

    [Header("damege base")] //david, é DAMAGE
    public float dano;

//EU TO ME PERDENDO TODA HORA EU CRIEI ESSE COMENTARIO PRA ME ACHAR
    [Header("Tempo de Jogo")]
    public float tempoDeJogo;

    [Header("Data Atual")]
    public string dataAtual;

    public Text dateDisplay;

    public TextMeshProUGUI timerText;
    
    private float startTime;

    public int SlotAtual = -1;

    void Start()
    {
        
        startTime = Time.time;
    }


    private void Awake()
    {
        //Detecta e exclui se ja presente
        if (Instace != null && Instace != this)
        {
            Destroy(gameObject);
            return;
        }

        Instace = this;

        //Continua entre as cenas
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {

        if (SceneManager.GetActiveScene().name == "Characters")
        {
            tempoDeJogo += Time.deltaTime; 
        }




        //timerText.text = minutes + ":" + seconds;

        //esse é o calendario atual, o dateDisplay vai pegar a data atual do sistema do pc.
        DateTime dataAtual = DateTime.Now;
        //dateDisplay.text = tempoDeJogo.ToString("dddd, MMMM dd, yyyy HH:mm:ss");
    }


}
