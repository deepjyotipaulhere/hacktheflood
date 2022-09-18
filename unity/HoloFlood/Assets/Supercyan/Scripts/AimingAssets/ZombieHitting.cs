using UnityEngine;

public class ZombieHitting : MonoBehaviour, IInitializable
{

    [SerializeField] private Animator m_animator;

    public enum InputButtonType
    {
        Mouse,
        Key,
        Button
    }

    [Header("Hitting")]
    [SerializeField] private InputButtonType m_hitButtonType = InputButtonType.Mouse;
    [SerializeField] private int m_hitMouseButton = 0;
    [SerializeField] private string m_hitButton = "";
    [SerializeField] private KeyCode m_hitKey = KeyCode.Alpha1;

    public bool IsDead { get; set; }

    private bool m_currentHit = false;
    private bool m_previousHit = false;

    private void Update()
    {
        if (!IsDead)
        {
            HitUpdate();
        }
    }

    private void HitUpdate()
    {
        switch (m_hitButtonType)
        {
            case InputButtonType.Mouse: m_currentHit = Input.GetMouseButton(m_hitMouseButton); break;
            case InputButtonType.Button: m_currentHit = Input.GetButton(m_hitButton); break;
            case InputButtonType.Key: m_currentHit = Input.GetKey(m_hitKey); break;
        }

        if (m_currentHit && !m_previousHit) { m_animator.SetTrigger("WeaponFire"); }
        m_previousHit = m_currentHit;
        m_currentHit = false;
    }

    private void Awake()
    {
        Initialize(gameObject);
    }

    public void Initialize(GameObject character)
    {
        m_animator = character.GetComponent<Animator>();
    }
}