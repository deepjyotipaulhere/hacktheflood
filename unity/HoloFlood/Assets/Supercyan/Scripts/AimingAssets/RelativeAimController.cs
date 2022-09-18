using UnityEngine;

public class RelativeAimController : MonoBehaviour, IInitializable
{

    public void Initialize(GameObject character)
    {
        InitializeAnimator(character);
        InitializeAimPoint(character);
    }

    public enum InputButtonType
    {
        Mouse,
        Key,
        Button
    }

    [Header("Aiming")]
    [SerializeField] private float m_aimSpeed = 100;
    [SerializeField] private string m_horizontalAxis = "Mouse X";
    [SerializeField] private string m_verticalAxis = "Mouse Y";
    [SerializeField, Range(0,-89)] private float m_minVerticalAngle = -89;
    [SerializeField, Range(0, 89)] private float m_maxVerticalAngle = 89;

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

    private CharacterWeaponAnimator m_animator;
    private Transform m_aimPoint;

    private bool m_isDead;
    public bool IsDead
    {
        set
        {
            m_isDead = value;
            if (m_animator != null)
            {
                if (m_isDead) { m_animator.SetGunInHand(false, m_animator.GunType); }
                else { m_animator.SetGunInHand(true, m_animator.GunType); }
            }
        }
    }

    private void Awake()
    {
        Initialize(gameObject);
        m_animator.AimDirection = transform.forward;
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
        float h = Input.GetAxis(m_horizontalAxis);
        float v = -Input.GetAxis(m_verticalAxis);

        Vector3 eulers = m_animator.AimEulerAngles;
        eulers.y = Mathf.Repeat(eulers.y + h * m_aimSpeed * Time.deltaTime, 360);
        eulers.x = ClampAxis(eulers.x - v * m_aimSpeed * Time.deltaTime, m_minVerticalAngle, m_maxVerticalAngle);
        eulers.z = 0;

        m_animator.AimEulerAngles = eulers;
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

    private static float ClampAxis(float angle, float min, float max, bool repeat = false)
    {
        float repeatedAngle = RepeatRange(angle, -360, 360);

        bool over = repeatedAngle > max;
        bool under = repeatedAngle < min;

        bool valid = !under && !over || repeatedAngle > min + 360;

        if (valid) { return repeat ? repeatedAngle : angle; }

        if (over || under)
        {
            float distToMin = Mathf.Abs(Mathf.DeltaAngle(repeatedAngle, min));
            float distToMax = Mathf.Abs(Mathf.DeltaAngle(repeatedAngle, max));

            if (over) { return distToMax < distToMin ? max : min; }
            else if (under) { return distToMin < distToMax ? min : max; }
        }

        return repeat ? repeatedAngle : angle;
    }

    private static float RepeatRange(float value, float min, float max)
    {
        if (min > 0) { throw new System.Exception("Min has to be a negative value"); }
        if (max < 0) { throw new System.Exception("Max has to be a positive value"); }

        if (value < 0)
        {
            return -Mathf.Repeat(-value, -min);
        }
        return Mathf.Repeat(value, max);
    }
}
