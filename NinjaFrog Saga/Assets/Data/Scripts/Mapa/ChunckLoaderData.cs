using UnityEngine;

[CreateAssetMenu(fileName = "ChunckLoaderData", menuName = "MapConfig/ChunckLoaderData")]
public class ChunckLoaderData : ScriptableObject
{
    [Header("Render Settings")]
    [Range (1,5)]
    public int renderDistance = 1; // Zona carregada
    public int preloadDistance = 1; //Zona pre-carregada
    public float chunckSize = 20f; // tamanho do chunck

    [Header("Gizmo Settings")]
    public bool drawFilledArea = true;

    [Header("Gizmo Colors")]
    public Color activeColor = new(0f, 1f, 0f, 0.25f);
    public Color preloadColor = new(1f, 1f, 0f, 0.25f);
    public Color inactiveColor = new(1f, 0f, 0f, 0.25f);
    public Color gridLineColor = Color.gray;
}
