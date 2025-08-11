using UnityEngine;

public class PlayerInputToAnimator : MonoBehaviour
{
    public Animator animator;

    //public bool podeLer = true;
    

    void Update()
    {
    
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
        }
        
    }
}
