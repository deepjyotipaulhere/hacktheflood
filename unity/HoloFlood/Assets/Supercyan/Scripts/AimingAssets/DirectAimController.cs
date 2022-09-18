using UnityEngine;

public class DirectAimController : MonoBehaviour, IInitializable
{

    public void Initialize(GameObject character)
    {
        InitializeAnimator(character);
        InitializeAimPoint(character);
    }

    private CharacterWeaponAnimator m_animator;
    private Transform m_aimPoint;

    public enum InputButtonType
    {
        Mouse,
        Key,
        Button
    }

    [Header("Shooting")]
    [SerializeField] private InputButtonType m_shootButtonType = InputButtonType.Mouse;
    [SerializeField] private int m_shootMouseButton = 0;
    [SerializeField] private string m_shootButton = "";
    [SerializeField] private KeyCode m_shootKey = KeyCode.Alpha1;

    [Header("Reloading")]
    [SerializeField] private InputButtonType m_reloadButtonType = InputButtonType.Key;
    [SerializeField] private int m_reloadMouseButton = 0;
    [SerializeField] private string m_reloadButton = "";
    [SerializeField] private KeyCode m_reloadKey = KeyCode.R;

    private bool m_weaponInHand;

    private bool m_isDead;
    public bool IsDead
    {
        set
        {
            m_isDead = value;
            if (m_animator != null)
            {
                if (m_isDead) { m_animator.SetGunInHand(false, -1); }
                else { m_animator.SetGunInHand(true, m_animator.GunType); }
            }
        }
    }

    private void Awake()
    {
        Initialize(gameObject);
    }

    private void Update()
    {
        if (!m_isDead)
        {
            ShootUpdate();
            ReloadUpdate();
            AimUpdate();
        }
    }

    private void ShootUpdate()
    {
        bool shoot = false;
        switch (m_shootButtonType)
        {
            case InputButtonType.Mouse: shoot = Input.GetMouseButton(m_shootMouseButton); break;
            case InputButtonType.Button: shoot = Input.GetButton(m_shootButton); break;
            case InputButtonType.Key: shoot = Input.GetKey(m_shootKey); break;
        }

        if (shoot) { m_animator.Shoot(); }
    }

    private void ReloadUpdate()
    {
        bool reload = false;
        switch (m_reloadButtonType)
        {
            case InputButtonType.Mouse: reload = Input.GetMouseButtonDown(m_reloadMouseButton); break;
            case InputButtonType.Button: reload = Input.GetButtonDown(m_reloadButton); break;
            case InputButtonType.Key: reload = Input.GetKeyDown(m_reloadKey); break;
        }

        if (reload) { m_animator.Reload(); }
    }

    private void AimUpdate()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Camera.main.transform.forward, transform.position + Camera.main.transform.forward * 20);

        float distance;
        Vector3? aimPosition = null;
        if (plane.Raycast(mouseRay, out distance))
        {
            aimPosition = mouseRay.GetPoint(distance);
        }

        if (aimPosition.HasValue)
        {
            m_animator.AimDirection = Vector3.Normalize(aimPosition.Value - m_aimPoint.position);
        }
    }

    private void InitializeAnimator(GameObject character)
    {
        if (m_animator != null) { return; }
        m_animator = character.GetComponent<CharacterWeaponAnimator>();
    }

    private void InitializeAimPoint(GameObject character)
    {
        if (m_aimPoint != null) { return; }

        AimPoint point = character.GetComponentInChildren<AimPoint>();
        if (!point)
        {
            GameObject newPoint = new GameObject("AimPoint");
            point = newPoint.AddComponent<AimPoint>();
            newPoint.transform.parent = character.transform;
            newPoint.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        }

        m_aimPoint = point.transform;
    }
}
