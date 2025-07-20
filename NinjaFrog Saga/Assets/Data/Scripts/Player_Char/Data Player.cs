using UnityEngine;

public class DataPlayer : MonoBehaviour
{
    public static DataPlayer Instace { get; private set; }

    //madao colocou essas duas variaveis pra pegar a posicao do sapo pra salvar dps.
    public float posicaoSalvaX;
    public float posicaoSalvaY;


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


}
