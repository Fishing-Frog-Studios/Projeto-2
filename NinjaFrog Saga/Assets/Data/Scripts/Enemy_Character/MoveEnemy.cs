using UnityEngine;
using UnityEngine.UIElements;

public class MoveEnemy : MonoBehaviour
{
    public float ratation;
    public float speed;
    public Transform Transform;

    private void FixedUpdate()
    {
        rotation();
    }
    void rotation()
    {
        ratation = ratation + speed * Time.deltaTime;
        Transform.rotation = Quaternion.Euler(0f, 0f, ratation);
    }
}
