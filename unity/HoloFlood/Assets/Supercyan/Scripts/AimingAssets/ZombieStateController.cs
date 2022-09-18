using UnityEngine;

public class ZombieStateController : MonoBehaviour, IInitializable
{

    [SerializeField] private Animator m_animator;
    [SerializeField] private CharacterMovement m_characterMovement;
    [SerializeField] private Strafer m_strafer;

    [SerializeField] private bool m_isZombie = true;

    public bool IsZombie
    {
        get
        {
            return m_isZombie;
        }
        set
        {
            m_isZombie = value;
            m_animator.SetBool("Zombie", m_isZombie);

            if (m_characterMovement != null) { m_characterMovement.IsZombie = m_isZombie; }
            if (m_strafer != null) { m_strafer.IsZombie = m_isZombie; }
        }
    }

    public void Initialize(GameObject character)
    {
        if (m_animator == null) { m_animator = character.GetComponent<Animator>(); }
        if (m_characterMovement == null) { m_characterMovement = character.GetComponent<CharacterMovement>(); }
        if (m_strafer == null) { m_strafer = character.GetComponent<Strafer>(); }
        IsZombie = m_isZombie;
    }

    private void Awake()
    {
        Initialize(gameObject);
    }
}
