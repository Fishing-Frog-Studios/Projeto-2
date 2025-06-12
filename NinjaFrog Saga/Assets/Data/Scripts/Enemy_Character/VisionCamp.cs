using UnityEngine;       // Necessário para a maioria das funcionalidades da Unity

// Garante que o GameObject tenha MeshFilter e MeshRenderer
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCamp : MonoBehaviour
{
    [Header("Configurações de Visão")]
    public float distanceVision;            // Distância máxima da visão (raio do cone)
    [Range(0, 360)]
    public float angleVision;               // Ângulo de visão em graus (abertura do cone)
    public int QuantDetection;              // Número de raycasts (linhas) usadas para formar o cone
    public LayerMask obstacle;              // Máscara de layers que bloqueiam a visão
    public Transform player;                // Referência ao jogador (pode ser atribuída na Unity)

    // --- Variáveis internas para construir e renderizar a malha do cone de visão ---

    private Mesh visionMesh;                // A malha que será desenhada no MeshRenderer
    private Vector3[] vertices;             // Vértices (pontos) da malha em espaço local
    private int[] triangles;                // Índices que formam triângulos a partir dos vértices

    private float angleIncrement;           // Incremento de ângulo entre cada raycast
    private float startAngle;               // Ângulo inicial (lado esquerdo do cone)

    // Arrays vazios de fallback (caso QuantDetection seja zero)
    private static readonly Vector3[] emptyVertices = new Vector3[0];
    private static readonly int[] emptyTriangles = new int[0];

    // Método chamado antes de Start(), ideal para inicializações que não dependem de outros scripts
    private void Awake()
    {
        // Cria uma nova malha para a visão e dá um nome para facilitar a depuração
        visionMesh = new Mesh() { name = "Enemy Vision Mesh" };
        // Atribui a malha criada ao MeshFilter deste GameObject
        GetComponent<MeshFilter>().mesh = visionMesh;

        // Configura o material (vermelho semi-transparente) no MeshRenderer
        if (TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            // Usa shader de sprites padrão para simplicidade
            Material mat = new(Shader.Find("Sprites/Default"))
            {
                // Define cor RGBA: vermelho (1,0,0) com alpha 0.3 (transparente)
                color = new Color(1, 0, 0, 0.3f)
            };
            meshRenderer.material = mat;
        }

        // Prepara os arrays de vértices e triângulos
        // +2 porque temos o vértice central (0) e QuantDetection+1 pontos na borda
        vertices = new Vector3[QuantDetection + 2];
        // Cada triângulo usa 3 índices, e teremos QuantDetection triângulos
        triangles = new int[QuantDetection * 3];
    }

    // LateUpdate é chamado após todos os Updates, garantindo que transformações já estejam aplicadas
    private void LateUpdate()
    {
        if (PlayerInSight() == true) { print("inimigo a vista"); }
        DrawVision();  // Atualiza e redesenha o cone de visão
    }

    // Constrói e aplica a malha do cone de visão a cada frame
    void DrawVision()
    {
        // Calcula o espaçamento angular entre cada raycast: Δθ = angleVision / QuantDetection
        angleIncrement = angleVision / QuantDetection;
        // Define o ângulo inicial como metade para a esquerda: -angleVision/2
        startAngle = -angleVision / 2f;
        // Origem da malha em espaço local (centro do cone)
        Vector3 origin = Vector3.zero;
        vertices[0] = origin;  // Primeiro vértice é o centro

        // Loop de 0 até QuantDetection (inclusive) para fechar o cone no final
        for (int i = 0; i <= QuantDetection; i++)
        {
            // Calcula o ângulo atual: θ = θ_inicial + i * Δθ
            float angle = startAngle + i * angleIncrement;
            // Converte ângulo em vetor direcional no plano XY
            Vector3 dir = DirFromAngle(angle, true);
            // Rotaciona pelo ângulo de rotação do objeto
            Vector3 globalDir = transform.rotation * dir;

            // Lança um raycast 2D na direção global até distanceVision
            RaycastHit2D hit = Physics2D.Raycast(transform.position, globalDir, distanceVision, obstacle);

            // Se colidir, usa ponto de impacto; senão, usa ponto no alcance máximo
            Vector3 vertex = hit
                ? transform.InverseTransformPoint(hit.point)  // converte world → local
                : dir * distanceVision;                        // ponto final do raio

            vertices[i + 1] = vertex;

            // Define índices dos triângulos: cada triângulo liga o centro a dois vértices consecutivos
            if (i < QuantDetection)
            {
                int triIndex = i * 3;
                triangles[triIndex] = 0;        // vértice central
                triangles[triIndex + 1] = i + 1;    // vértice atual
                triangles[triIndex + 2] = i + 2;    // próximo vértice
            }
        }

        // Apaga dados antigos e atualiza a mesh com novos vértices e triângulos
        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();  // recalcula normais para iluminação correta
    }

    // Converte um ângulo (em graus) em um vetor unitário no plano XY
    public Vector3 DirFromAngle(float angleDegrees, bool isGlobal)
    {
        if (!isGlobal)
            // se for local, compõe com a rotação Z do objeto
            angleDegrees += transform.eulerAngles.z;

        // Mathf.Deg2Rad = π/180; converte graus em radianos
        float rad = angleDegrees * Mathf.Deg2Rad;
        // cos(rad) = eixo X, sin(rad) = eixo Y
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    // Verifica se o jogador está dentro do cone de visão e sem obstáculos,
    // usando a mesma direção inicial que DrawVision (0° local rotacionado)
    public bool PlayerInSight()
    {
        // Direção normalizada até o jogador
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        // Distância até o jogador
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Fora do alcance?
        if (distToPlayer > distanceVision)
            return false;

        // Define a direção "zero grau" do cone: DirFromAngle(0, true) em espaço local,
        // depois aplica a rotação do objeto para obter a direção inicial global
        Vector3 viewDir = transform.rotation * DirFromAngle(0, true);

        // Calcula o ângulo entre essa direção inicial e a direção ao jogador
        float angleToPlayer = Vector3.Angle(viewDir, dirToPlayer);
        // Jogador fora do meio-ângulo do cone?
        if (angleToPlayer > angleVision / 2f)
            return false;

        // Raycast para verificar obstáculo entre inimigo e jogador
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distToPlayer, obstacle);
        if (hit.collider != null && hit.collider.transform != player)
            return false;

        return true;  // Jogador está visível
    }

    // Desenha gizmos no editor para visualização do cone de visão
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;  // Cor vermelha para os gizmos
        Vector3 pos = transform.position;

        // Círculo mostrando o alcance máximo
        Gizmos.DrawWireSphere(pos, distanceVision);

        // Direções limites do cone (já considera transform.eulerAngles.z)
        Vector3 viewAngleA = DirFromAngle(-angleVision / 2);
        Vector3 viewAngleB = DirFromAngle(angleVision / 2);

        Gizmos.DrawLine(pos, pos + viewAngleA * distanceVision);
        Gizmos.DrawLine(pos, pos + viewAngleB * distanceVision);
    }

    // Sobrecarga de DirFromAngle para uso em OnDrawGizmos (retorna Z=0)
    Vector3 DirFromAngle(float angleInDegrees)
    {
        float angleRad = (angleInDegrees + transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
    }
}
