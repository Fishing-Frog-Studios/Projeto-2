using UnityEngine;

public class SortingOrderByY : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Quanto menor o Y, maior o sortingOrder → mais "na frente"
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
