using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour, IInitializable
{

    public void Initialize(GameObject character)
    {
        if (m_movementAnimator == null)
        {
            m_movementAnimator = character.GetComponent<CharacterMovementAnimator>();
        }

        if (m_stanceAnimator == null)
        {
            m_stanceAnimator = character.GetComponent<CharacterStanceAnimator>();
        }

        if (m_rigidbody == null)
        {
            m_rigidbody = character.GetComponent<Rigidbody>();
        }

        if (m_relativeTo == null) { m_relativeTo = transform; }
    }

    public enum InputButtonType
    {
        Mouse,
        Key,
        Button
    }

    [Header("Movement")]
    [SerializeField] private string m_horizontalAxis = "Horizontal";
    [SerializeField] private string m_verticalAxis = "Vertical";
    [SerializeField] private Transform m_relativeTo = null;

    [Header("Walking")]
    [SerializeField] private InputButtonType m_walkButtonType = InputButtonType.Key;
    [SerializeField] private int m_walkMouseButton = 0;
    [SerializeField] private string m_walkButton = "";
    [SerializeField] private KeyCode m_walkKey = KeyCode.LeftShift;

    [Header("Crouching")]
    [SerializeField] private InputButtonType m_crouchButtonType = InputButtonType.Key;
    [SerializeField] private int m_crouchMouseButton = 0;
    [SerializeField] private string m_crouchButton = "";
    [SerializeField] private KeyCode m_crouchKey = KeyCode.LeftControl;

    [Header("Prone")]
    [SerializeField] private InputButtonType m_proneButtonType = InputButtonType.Key;
    [SerializeField] private int m_proneMouseButton = 0;
    [SerializeField] private string m_proneButton = "";
    [SerializeField] private KeyCode m_proneKey = KeyCode.Z;

    [Header("Jumping")]
    [SerializeField] private InputButtonType m_jumpButtonType = InputButtonType.Key;
    [SerializeField] private int m_jumpMouseButton = 0;
    [SerializeField] private string m_jumpButton = "";
    [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;

    [Header("Movement Variables")]
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private float m_minJumpInterval = 0.25f;

    [Header("Components")]
    [SerializeField] private CharacterMovementAnimator m_movementAnimator = null;
    [SerializeField] private CharacterStanceAnimator m_stanceAnimator = null;
    [SerializeField] private Rigidbody m_rigidbody = null;

    private float m_currentV = 0;
    private float m_currentH = 0;
    private Vector3 m_currentMovement = Vector3.zero;

    private float m_jumpTimestamp = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;

    private List<Collider> m_collisions = new List<Collider>();

    private bool m_isGrounded = false;
    private bool m_isWalking = false;

    public bool IsDead { get; set; }
    public bool IsZombie { get; set; }

    private void Awake()
    {
        Initialize(gameObject);
    }

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }
    #endregion

    private void Update()
    {
        if (!IsDead)
        {
            CrouchUpdate();
            ProneUpdate();
            JumpUpdate();
        }
    }

    private void FixedUpdate()
    {
        m_movementAnimator.SetGrounded(m_isGrounded);

        if (!IsDead)
        {
            WalkUpdate();
            MovementUpdate();
        }
    }

    private void MovementUpdate()
    {
        float v = Input.GetAxis(m_verticalAxis);
        float h = Input.GetAxis(m_horizontalAxis);

        if (m_isWalking)
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        switch (m_stanceAnimator.CurrentStance)
        {
            case CharacterStanceAnimator.StanceState.Crouching:
                v *= 0.5f;
                h *= 0.5f;
                break;
            case CharacterStanceAnimator.StanceState.Prone:
                v *= 0.25f;
                h *= 0.25f;
                break;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.fixedDeltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.fixedDeltaTime * m_interpolation);

        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        if (m_relativeTo != null)
        {
            forward = m_relativeTo.forward;
            forward.y = 0.0f;
            forward.Normalize();

            right = m_relativeTo.right;
            right.y = 0.0f;
            right.Normalize();
        }

        Vector3 movement = right * m_currentH + forward * m_currentV;
        if (movement != Vector3.zero)
        {
            m_currentMovement = Vector3.Slerp(m_currentMovement, movement, Time.fixedDeltaTime * m_interpolation);
            m_rigidbody.MovePosition(m_rigidbody.position + m_currentMovement * m_moveSpeed * Time.fixedDeltaTime);
        }

        m_movementAnimator.SetMovement(movement);
    }

    private void WalkUpdate()
    {
        m_isWalking = false;
        switch (m_walkButtonType)
        {
            case InputButtonType.Mouse: m_isWalking = Input.GetMouseButton(m_walkMouseButton); break;
            case InputButtonType.Button: m_isWalking = Input.GetButton(m_walkButton); break;
            case InputButtonType.Key: m_isWalking = Input.GetKey(m_walkKey); break;
        }
    }

    private void CrouchUpdate()
    {
        bool changeStance = false;

        switch (m_crouchButtonType)
        {
            case InputButtonType.Mouse:
                changeStance = Input.GetMouseButtonDown(m_crouchMouseButton);
                break;
            case InputButtonType.Button:
                changeStance = Input.GetButtonDown(m_crouchButton);
                break;
            case InputButtonType.Key:
                changeStance = Input.GetKeyDown(m_crouchKey);
                break;
        }

        if (changeStance)
        {
            switch (m_stanceAnimator.CurrentStance)
            {
                case CharacterStanceAnimator.StanceState.Prone:
                case CharacterStanceAnimator.StanceState.Standing:
                    m_stanceAnimator.Crouch();
                    break;
                case CharacterStanceAnimator.StanceState.Crouching:
                    m_stanceAnimator.Stand();
                    break;
                default:
                    break;
            }
        }
    }

    private void ProneUpdate()
    {
        bool changeStance = false;

        switch (m_proneButtonType)
        {
            case InputButtonType.Mouse:
                changeStance = Input.GetMouseButtonDown(m_proneMouseButton);
                break;
            case InputButtonType.Button:
                changeStance = Input.GetButtonDown(m_proneButton);
                break;
            case InputButtonType.Key:
                changeStance = Input.GetKeyDown(m_proneKey);
                break;
        }

        if (changeStance)
        {
            switch (m_stanceAnimator.CurrentStance)
            {
                case CharacterStanceAnimator.StanceState.Crouching:
                case CharacterStanceAnimator.StanceState.Standing:
                    m_stanceAnimator.Prone();
                    break;
                case CharacterStanceAnimator.StanceState.Prone:
                    m_stanceAnimator.Stand();
                    break;
                default:
                    break;
            }
        }
    }

    private void JumpUpdate()
    {
        bool jump = false;
        switch (m_jumpButtonType)
        {
            case InputButtonType.Mouse: jump = Input.GetMouseButton(m_jumpMouseButton); break;
            case InputButtonType.Button: jump = Input.GetButton(m_jumpButton); break;
            case InputButtonType.Key: jump = Input.GetKey(m_jumpKey); break;
        }

        if (jump && m_stanceAnimator.CurrentStance == CharacterStanceAnimator.StanceState.Standing && !IsZombie)
        {
            bool jumpCooldownOver = (Time.fixedTime - m_jumpTimestamp) >= m_minJumpInterval;
            if (jumpCooldownOver && m_isGrounded)
            {
                m_jumpTimestamp = Time.fixedTime;
                m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            }
        }
    }

}
