using UnityEngine;

public class Croucher : MonoBehaviour, IInitializable
{
    public void Initialize(GameObject character)
    {
        m_animator = character.GetComponent<Animator>();
    }

    private enum MoveState
    {
        Standing,
        Crouching,
        Prone
    }

    private MoveState m_moveState = MoveState.Standing;
    [SerializeField] private Animator m_animator;
    public bool IsStanding { get { return m_moveState == MoveState.Standing; } }

    public Animator Animator { set { m_animator = value; } }

    public void Stand()
    {
        m_moveState = MoveState.Standing;
        m_animator.SetInteger("MoveState", 0);
    }

    public void Crouch()
    {
        m_moveState = MoveState.Crouching;
        m_animator.SetInteger("MoveState", 1);
    }

    public void Prone()
    {
        m_moveState = MoveState.Prone;
        m_animator.SetInteger("MoveState", 2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            switch (m_moveState)
            {
                case MoveState.Prone:
                case MoveState.Standing:
                    Crouch();
                    break;

                case MoveState.Crouching:
                    Stand();
                    break;

                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (m_moveState)
            {
                case MoveState.Prone:
                    Stand();
                    break;

                case MoveState.Crouching:
                case MoveState.Standing:
                    Prone();
                    break;

                default:
                    break;
            }
        }
    }
}
