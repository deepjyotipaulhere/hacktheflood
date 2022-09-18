using UnityEngine;

public class CharacterMovementAnimator : MonoBehaviour, IInitializable
{

    public void Initialize(GameObject character)
    {
        if (m_animator == null)
        {
            m_animator = character.GetComponent<Animator>();
        }
    }

    [SerializeField] private Animator m_animator;

    //private bool m_isGrounded;

    public Animator Animator { set { m_animator = value; } }

    private void Awake()
    {
        Initialize(gameObject);
    }

    public void SetMovement(Vector3 movement)
    {
        Vector3 animationDirection = Quaternion.Inverse(transform.rotation) * movement;
        m_animator.SetFloat("MoveHorizontal", animationDirection.x);
        m_animator.SetFloat("MoveVertical", animationDirection.z);
    }

    public void SetGrounded(bool value)
    {
        m_animator.SetBool("Grounded", value);
        //m_isGrounded = value;
    }
}
