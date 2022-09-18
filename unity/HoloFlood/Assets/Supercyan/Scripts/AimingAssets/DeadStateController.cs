using UnityEngine;

public class DeadStateController : MonoBehaviour, IInitializable
{

    [SerializeField] private Animator m_animator;

    [SerializeField] private RelativeAimController m_relativeAimController;
    [SerializeField] private DirectAimController m_directAimController;
    [SerializeField] private CharacterMovement m_characterMovement;
    [SerializeField] private Strafer m_strafer;

    private bool m_isDead = false;
    public bool IsDead
    {
        get { return m_isDead; }
        set
        {
            m_isDead = value;
            if (m_isDead) { m_animator.SetTrigger("Death" + Random.Range(1, 4)); }

            if (m_relativeAimController != null) { m_relativeAimController.IsDead = m_isDead; }
            if (m_directAimController != null) { m_directAimController.IsDead = m_isDead; }
            if (m_characterMovement != null) { m_characterMovement.IsDead = m_isDead; }
            if (m_strafer != null) { m_strafer.IsDead = m_isDead; }

            m_animator.SetBool("IsDead", m_isDead);
        }
    }

    public void Initialize(GameObject character)
    {
        if (m_animator == null) { m_animator = character.GetComponent<Animator>(); }
        if (m_relativeAimController == null) { m_relativeAimController = character.GetComponent<RelativeAimController>(); }
        if (m_directAimController == null) { m_directAimController = character.GetComponent<DirectAimController>(); }
        if (m_characterMovement == null) { m_characterMovement = character.GetComponent<CharacterMovement>(); }
        if (m_strafer == null) { m_strafer = character.GetComponent<Strafer>(); }
        IsDead = m_isDead;
    }

    private void Awake()
    {
        Initialize(gameObject);
    }
}
