using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class PlayerRegistrar : MonoBehaviour
{
    void Start()
    {
        // <<< CORRIGIDO: "Instance" com 'n' >>>
        if (DataPlayer.Instance != null)
        {
            HealthSystem health = GetComponent<HealthSystem>();
            Transform trans = transform;
            Camera cam = Camera.main; 

            // <<< CORRIGIDO: "Instance" com 'n' >>>
            DataPlayer.Instance.RegisterPlayerReferences(health, trans, cam);
        }
        else
        {
            Debug.LogError("ERRO CRÍTICO: DataPlayer.Instance não foi encontrado!");
        }
    }
}
