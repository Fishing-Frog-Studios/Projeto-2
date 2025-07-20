// Dentro de PlayerSetup.cs
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    void Start()
    {
        if (DataPlayer.Instace != null)
        {
            // USA AS VARIÁVEIS SEGURAS PARA DEFINIR A POSIÇÃO
            Vector2 posicaoSalva = new Vector2(DataPlayer.Instace.posicaoSalvaX, DataPlayer.Instace.posicaoSalvaY);
            
            // Aplica a posição a ESTE objeto Sapo
            transform.position = posicaoSalva;

            Debug.Log("PlayerSetup: Posição do Sapo definida para " + posicaoSalva);
        }
    }
}