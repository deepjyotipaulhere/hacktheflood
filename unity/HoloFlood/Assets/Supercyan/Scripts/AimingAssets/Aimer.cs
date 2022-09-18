using UnityEngine;

public class Aimer : MonoBehaviour, IInitializable
{
    public void Initialize(GameObject character)
    {
        m_animator = character.GetComponent<Animator>();

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

    private enum AimMode
    {
        Free,
        Horizontal
    }

    [SerializeField] private AimMode m_mode = AimMode.Free;
    [SerializeField] private float m_aimAngle = 0;
    [SerializeField] private Vector3 m_aimDir;
    [SerializeField] private Vector3 m_aimPos;
    [SerializeField] private Vector3 m_hitNormal;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_aimPoint;
    [SerializeField] private bool m_gunInHand = false;

    private int m_aimAnimLayerNum;
    private int m_weaponFireAnimLayerNum;
    private int m_gunType;

    public Animator Animator { set { m_animator = value; } }
    public Transform AimPoint { set { m_aimPoint = value; } }
    public Vector3 AimDir { get { return m_aimDir; } }

    private void Awake()
    {
        m_aimAnimLayerNum = m_animator.GetLayerIndex("AimOverride");
        m_weaponFireAnimLayerNum = m_animator.GetLayerIndex("WeaponFireAdditive");
    }

    public void SetGunInHand(bool value, int gunType)
    {
        m_gunInHand = value;
        if (m_gunInHand)
        {
            m_animator.SetLayerWeight(m_aimAnimLayerNum, 1f);
            m_animator.SetLayerWeight(m_weaponFireAnimLayerNum, 1f);
            m_animator.SetInteger("GunType", gunType);
        }

        else
        {
            m_animator.SetLayerWeight(m_aimAnimLayerNum, 0f);
            m_animator.SetLayerWeight(m_weaponFireAnimLayerNum, 0f);
        }

        m_gunType = gunType;
    }

    private void Update()
    {
        switch (m_gunType)
        {
            case 0:
                if (Input.GetMouseButton(0))
                {
                    m_animator.SetTrigger("WeaponFire");
                }
                break;

            case 1:
            case 2:
                if (Input.GetMouseButtonDown(0))
                {
                    m_animator.SetTrigger("WeaponFire");
                }
                break;

            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_animator.SetTrigger("WeaponReload");
        }
    }

    private void FixedUpdate()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Camera.main.transform.forward, transform.position + Camera.main.transform.forward * 20);

        float distance;
        if (plane.Raycast(mouseRay, out distance)) { m_aimPos = mouseRay.GetPoint(distance); }

        RaycastHit hitInfo;
        if (Physics.Raycast(mouseRay, out hitInfo, distance))
        {
            m_aimPos = hitInfo.point;
            m_hitNormal = hitInfo.normal;
        }

        m_aimDir = Vector3.Normalize(m_aimPos - m_aimPoint.position);

        switch (m_mode)
        {
            case AimMode.Horizontal:
                HorizontalAimUpdate(hitInfo);
                break;

            case AimMode.Free:
                FreeAimUpdate(hitInfo);
                break;

            default:
                Debug.LogError("Enum not supported");
                break;
        }

        m_animator.SetFloat("AimAngle", m_aimAngle);
    }

    private void HorizontalAimUpdate(RaycastHit hitInfo)
    {
        m_aimDir.y = 0.0f;

        Quaternion aimRotation = Quaternion.LookRotation(m_aimDir);

        transform.rotation = Quaternion.Euler(0, aimRotation.eulerAngles.y, 0);

        m_aimAngle = 0.45f;
    }

    private void FreeAimUpdate(RaycastHit hitInfo)
    {
        Quaternion aimRotation = Quaternion.LookRotation(m_aimDir);

        transform.rotation = Quaternion.Euler(0, aimRotation.eulerAngles.y, 0);

        m_aimAngle = aimRotation.eulerAngles.x;
        if (m_aimAngle > 90) { m_aimAngle = (m_aimAngle - 360); }
        m_aimAngle = 1 - ((m_aimAngle + 90) / 180);
    }

    private void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;

        if (normal.normalized != Vector3.forward)
        {
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        }
        else
        {
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
        }

        Vector3 corner0 = position + v3;
        Vector3 corner2 = position - v3;
        Quaternion q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        Vector3 corner1 = position + v3;
        Vector3 corner3 = position - v3;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(position, normal);
    }

    private void OnDrawGizmos()
    {
        DrawPlane(transform.position + Camera.main.transform.forward * 5, Camera.main.transform.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_aimPoint.position, m_aimPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(m_aimPos, m_aimPos + m_hitNormal);
    }
}
