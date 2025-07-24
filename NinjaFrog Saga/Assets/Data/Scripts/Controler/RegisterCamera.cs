using UnityEngine;

// Script simples para registrar a câmera principal no DataPlayer.
public class RegisterCamera : MonoBehaviour
{
    void Awake()
    {
        // Garante que o DataPlayer exista e que a câmera não foi registrada ainda.
        if (DataPlayer.Instance != null && DataPlayer.Instance.mainCamera == null)
        {
            DataPlayer.Instance.mainCamera = GetComponent<Camera>();
            Debug.Log("Câmera principal registrada no DataPlayer.");
        }
    }
}
