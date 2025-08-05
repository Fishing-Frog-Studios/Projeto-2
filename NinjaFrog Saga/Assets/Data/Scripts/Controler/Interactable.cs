using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    private IInteractable interactable;
    private bool isPlayerNear = false;
    private GameObject player;

    [Header("UI")]
    public GameObject interactionIconPrefab; // Prefab do ícone

    private GameObject spawnedIcon;
    public Vector3 iconOffset = new Vector3(0, 1f, 0); // Posição do ícone acima do objeto

    private void Awake()
    {
        interactable = GetComponent<IInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning($"{name} não implementa IInteractable");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            player = other.gameObject;
            ShowIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            player = null;
            HideIcon();
        }
    }

    public void OnInteractInput()
    {
        if (isPlayerNear && interactable != null)
        {
            interactable.Interact(player);
        }
    }

    private void ShowIcon()
    {
        if (interactionIconPrefab != null && spawnedIcon == null)
        {
            spawnedIcon = Instantiate(interactionIconPrefab, transform.position + iconOffset, Quaternion.identity);
            spawnedIcon.transform.SetParent(transform);
        }
    }

    private void HideIcon()
    {
        if (spawnedIcon != null)
        {
            Destroy(spawnedIcon);
            spawnedIcon = null;
        }
    }
}
