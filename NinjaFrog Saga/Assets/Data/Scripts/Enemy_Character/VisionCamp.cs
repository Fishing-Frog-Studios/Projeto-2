using UnityEngine;

// Garante que o GameObject tenha MeshFilter e MeshRenderer
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCamp : MonoBehaviour
{
    [Header("Configurações de Visão")]
    public float distanceVision;
    [Range(0, 360)]
    public float angleVision;
    public int QuantDetection;
    public LayerMask obstacle;
    
    // A referência ao jogador será preenchida via código a partir do DataPlayer.
    public Transform player;

    private Mesh visionMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float angleIncrement;
    private float startAngle;

    private void Awake()
    {
        visionMesh = new Mesh() { name = "Enemy Vision Mesh" };
        GetComponent<MeshFilter>().mesh = visionMesh;
        if (TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            Material mat = new(Shader.Find("Sprites/Default"))
            {
                color = new Color(1, 0, 0, 0.3f)
            };
            meshRenderer.material = mat;
        }
        
        // Garante que os arrays não sejam nulos se QuantDetection for 0
        if (QuantDetection > 0)
        {
            vertices = new Vector3[QuantDetection + 2];
            triangles = new int[QuantDetection * 3];
        }
    }
    
    // Método Start para buscar a referência do jogador.
    private void Start()
    {
        // Se a referência do jogador está vazia, busca no DataPlayer.
        if (player == null)
        {
            // <<< CORRIGIDO: Usa DataPlayer.Instance com 'n' >>>
            if (DataPlayer.Instance != null) 
            {
                player = DataPlayer.Instance.playerTransform;
            }
            else
            {
                Debug.LogError("ERRO CRÍTICO: " + gameObject.name + " não conseguiu encontrar o DataPlayer.Instance!");
            }
        }
    }

    private void LateUpdate()
    {
        // Se o jogador não foi encontrado, não executa a lógica de visão.
        if (player == null || QuantDetection <= 0)
        {
            if(visionMesh != null) visionMesh.Clear();
            return;
        }

        if (PlayerInSight()) { /* print("inimigo a vista"); */ }
        DrawVision();
    }

    void DrawVision()
    {
        angleIncrement = angleVision / QuantDetection;
        startAngle = -angleVision / 2f;
        Vector3 origin = Vector3.zero;
        vertices[0] = origin;
        for (int i = 0; i <= QuantDetection; i++)
        {
            float angle = startAngle + i * angleIncrement;
            Vector3 dir = DirFromAngle(angle, true);
            Vector3 globalDir = transform.rotation * dir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, globalDir, distanceVision, obstacle);
            Vector3 vertex = hit
                ? transform.InverseTransformPoint(hit.point)
                : dir * distanceVision;
            vertices[i + 1] = vertex;
            if (i < QuantDetection)
            {
                int triIndex = i * 3;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;
            }
        }
        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
    }

    public Vector3 DirFromAngle(float angleDegrees, bool isGlobal)
    {
        if (!isGlobal)
            angleDegrees += transform.eulerAngles.z;
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public bool PlayerInSight()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer > distanceVision)
            return false;
        Vector3 viewDir = transform.rotation * DirFromAngle(0, true);
        float angleToPlayer = Vector3.Angle(viewDir, dirToPlayer);
        if (angleToPlayer > angleVision / 2f)
            return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distToPlayer, obstacle);
        // Garante que o hit não é no próprio inimigo e que acertou o jogador
        if (hit.collider != null && hit.collider.transform != player)
            return false;
        return true;
    }

    void OnDrawGizmos()
    {
        if (transform == null) return;
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Gizmos.DrawWireSphere(pos, distanceVision);
        Vector3 viewAngleA = DirFromAngleForGizmo(-angleVision / 2);
        Vector3 viewAngleB = DirFromAngleForGizmo(angleVision / 2);
        Gizmos.DrawLine(pos, pos + viewAngleA * distanceVision);
        Gizmos.DrawLine(pos, pos + viewAngleB * distanceVision);
    }

    Vector3 DirFromAngleForGizmo(float angleInDegrees)
    {
        float angleRad = (angleInDegrees + transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
    }
}
