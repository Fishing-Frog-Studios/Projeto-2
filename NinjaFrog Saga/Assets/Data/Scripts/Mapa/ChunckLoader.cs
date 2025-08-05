using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class ChunckLoader : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] chuncks;
    [SerializeField] private ChunckLoaderData data;

    private readonly Dictionary<Vector2Int, ChunckInfo> chunckDic = new();
    private Vector2Int currentPlayerChunck;

    private void Start()
    {
        MapChuncks();
        currentPlayerChunck = GetPlayerChunck();
        UpdateChunck();
    }

    private void Update()
    {
        Vector2Int newPlayerChunck = GetPlayerChunck();

        if (newPlayerChunck != currentPlayerChunck)
        {
            currentPlayerChunck = newPlayerChunck;
            UpdateChunck();
        }
    }

    private void MapChuncks()
    {
        chunckDic.Clear();
        foreach (GameObject chunck in chuncks)
        {
            Vector2Int coords = ParseCoordsFromName(chunck.name);
            if (coords != Vector2Int.one * int.MinValue)
            {
                chunckDic[coords] = new ChunckInfo(chunck, coords.x, coords.y);
            }
            else
            {
                Debug.LogWarning($"nome do chunck invalido: {chunck.name}");
            }
        }
    }

    private Vector2Int GetPlayerChunck()
    {
        int x = Mathf.FloorToInt(player.position.x / data.chunckSize);
        int y = Mathf.FloorToInt(player.position.y / data.chunckSize);
        return new Vector2Int(x, y);
    }

    private void UpdateChunck()
    {
        foreach (var kvp in chunckDic)
        {
            Vector2Int coords = kvp.Key;
            ChunckInfo chunckInfo = kvp.Value;

            int dx = Mathf.Abs(coords.x - currentPlayerChunck.x);
            int dy = Mathf.Abs(coords.y - currentPlayerChunck.y);

            if (dx <= data.renderDistance && dy <= data.renderDistance)
            {
                chunckInfo.chunckObject.SetActive(true);
            }
            else if (dx <= data.renderDistance + data.preloadDistance && dy <= data.renderDistance + data.preloadDistance)
            {
                if (chunckInfo.chunckObject.activeSelf)
                    chunckInfo.chunckObject.SetActive(false);
            }
            else
            {
                if (chunckInfo.chunckObject.activeSelf)
                    chunckInfo.chunckObject.SetActive(false);
            }
        }
    }

    private Vector2Int ParseCoordsFromName(string chunckName)
    {
        string[] parts = chunckName.Split('_');
        if (parts.Length < 4)
            return Vector2Int.one * int.MinValue;

        bool parsedX = int.TryParse(parts[2], out int x);
        bool parsedY = int.TryParse(parts[3], out int y);

        if (parsedX && parsedY)
            return new Vector2Int(x, y);

        return Vector2Int.one * int.MinValue;
    }

    public class ChunckInfo
        {
        public GameObject chunckObject;
        public int x;
        public int y;   

        public ChunckInfo(GameObject obj, int xIndex, int yIndex)
            {  
                chunckObject = 
                    obj; x = 
                    xIndex; y = 
                    yIndex;
            }
        }
    private void OnDrawGizmos()
    {
        if (chuncks == null || chuncks.Length == 0 || data == null) return;

        float size = data.chunckSize;

        // 🔹 Desenhar grid geral contínuo
        DrawGrid(size);

        foreach (GameObject chunk in chuncks)
        {
            if (chunk == null) continue;

            Vector2Int coords = ParseCoordsFromName(chunk.name);
            if (coords == Vector2Int.one * int.MinValue) continue;

            Vector2Int playerChunk = Application.isPlaying
                ? currentPlayerChunck
                : GetPlayerChunck();

            int dx = Mathf.Abs(coords.x - playerChunk.x);
            int dy = Mathf.Abs(coords.y - playerChunk.y);

            Vector3 pos = chunk.transform.position;
            Vector3 center = new(
                pos.x + size / 2f,
                pos.y - size / 2f,
                pos.z
            );

            Color fillColor;
            Color borderColor;

            if (dx <= data.renderDistance && dy <= data.renderDistance)
            {
                fillColor = data.activeColor;
                borderColor = Color.green;
            }
            else if (dx <= data.renderDistance + data.preloadDistance && dy <= data.renderDistance + data.preloadDistance)
            {
                fillColor = data.preloadColor;
                borderColor = Color.yellow;
            }
            else
            {
                fillColor = data.inactiveColor;
                borderColor = Color.red;
            }

            // Área preenchida opcional
            if (data.drawFilledArea)
            {
                Gizmos.color = fillColor;
                Gizmos.DrawCube(center, new Vector3(size, size, 0.1f));
            }

            // Contorno
            Gizmos.color = borderColor;
            Gizmos.DrawWireCube(center, new Vector3(size, size, 0.1f));
        }

    }

    private void DrawGrid(float size)
    {
        if (chuncks == null || chuncks.Length == 0) return;

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (GameObject chunk in chuncks)
        {
            if (chunk == null) continue;

            Vector3 pos = chunk.transform.position;

            if (pos.x < minX) minX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
        }

        Gizmos.color = data.gridLineColor;

        for (float x = minX; x <= maxX + size; x += size)
        {
            Gizmos.DrawLine(new Vector3(x, minY, 0), new Vector3(x, maxY + size, 0));
        }

        for (float y = minY; y <= maxY + size; y += size)
        {
            Gizmos.DrawLine(new Vector3(minX, y, 0), new Vector3(maxX + size, y, 0));
        }
    }
}
#endif



