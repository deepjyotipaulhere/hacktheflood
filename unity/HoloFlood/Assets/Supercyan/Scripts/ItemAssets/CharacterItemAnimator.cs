using System.Collections;
using UnityEngine;

public class CharacterItemAnimator : MonoBehaviour, IInitializable
{

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private ItemHoldLogic m_itemHoldLogic = null;
    [SerializeField] private Croucher m_croucher = null;

    public enum InputButtonType
    {
        Mouse,
        Key,
        Button
    }

    public enum Hand { Right, Left }
    [Space(10)]
    [SerializeField] private Hand m_hand = Hand.Right;
    public Hand ThisHand { get { return m_hand; } set { m_hand = value; } }
    [HideInInspector] public bool UseBothHands = false;
    [SerializeField] private bool m_allowCrouchingInteraction = false;
    private string m_holding = "";
    private string m_interact = "";
    private string m_dropItem = "";

    private bool m_itemActive = false;

    [HideInInspector] public bool LoopingAnimationStanding = false;
    private float m_interactionStandingStartDuration = 0f;
    public float InteractionStandingStartDuration { set { m_interactionStandingStartDuration = value; } }
    [HideInInspector] public bool LoopingAnimationCrouching = false;
    private float m_interactionCrouchingStartDuration = 0f;
    public float InteractionCrouchingStartDuration { set { m_interactionCrouchingStartDuration = value; } }
    private float m_loopingDuration = 1f;
    public float LoopingDuration { set { m_loopingDuration = value; } }

    [Header("Item Interaction")]
    [SerializeField] private InputButtonType m_interactionButtonType = InputButtonType.Key;
    [SerializeField] private int m_interactionMouseButton = 0;
    [SerializeField] private string m_interactionButton = "";
    [SerializeField] private KeyCode m_interactionKey = KeyCode.F;

    [Header("Toggle Item")]
    [SerializeField] private InputButtonType m_toggleItemButtonType = InputButtonType.Key;
    [SerializeField] private int m_toggleItemMouseButton = 0;
    [SerializeField] private string m_toggleItemButton = "";
    [SerializeField] private KeyCode m_toggleItemKey = KeyCode.G;

    [Header("Drop Item")]
    [SerializeField] private InputButtonType m_dropItemButtonType = InputButtonType.Key;
    [SerializeField] private int m_dropItemMouseButton = 0;
    [SerializeField] private string m_dropItemButton = "";
    [SerializeField] private KeyCode m_dropItemKey = KeyCode.T;

    public bool Animating { get { return m_animator.GetBool("Animating"); } }

    private bool m_previousInteract;
    private bool m_currentInteract;
    private bool m_previousToggle;
    private bool m_currentToggle;
    private bool m_previousDrop;
    private bool m_currentDrop;
    private bool m_standing = true;

    private void Awake()
    {
        Initialize(gameObject);
    }

    public void Initialize(GameObject character)
    {
        if (m_animator == null) { m_animator = character.GetComponent<Animator>(); }
        if (m_itemHoldLogic == null) { m_itemHoldLogic = character.GetComponent<ItemHoldLogic>(); }
        if (m_croucher == null) { m_croucher = character.GetComponent<Croucher>(); }

        if (m_animator.runtimeAnimatorController.name != "StrafeMovement" ||
            m_animator == null)
        {
            Debug.LogWarning("Change character animator controller to StrafeMovement in order to use item interaction animations.");
            enabled = false;
            return;
        }

        if (m_hand == Hand.Right)
        {
            if (m_itemHoldLogic.m_itemInHandR != null) { m_itemActive = true; }
            m_holding = "HoldingR";
            m_interact = "InteractR";
            m_dropItem = "DropItemR";
        }
        else if (m_hand == Hand.Left)
        {
            if (m_itemHoldLogic.m_itemInHandL != null) { m_itemActive = true; }
            m_holding = "HoldingL";
            m_interact = "InteractL";
            m_dropItem = "DropItemL";
        }
    }

    private void Update()
    {
        if (m_croucher) { m_standing = m_croucher.IsStanding; }
        if (!Animating)
        {
            if (m_itemActive && (m_allowCrouchingInteraction || m_standing))
            { InteractUpdate(); }

            if (m_standing)
            {
                if (m_itemActive) { ItemDropUpdate(); }
                ItemToggleUpdate();
            }
        }
    }

    public void SetHolding(bool value, bool isWeapon)
    {
        if (m_animator.runtimeAnimatorController.name != "StrafeMovement" ||
            m_animator == null)
        {
            Debug.LogWarning("Change character animator controller to StrafeMovement in order to use item interaction animations.");
            enabled = false;
            return;
        }

        m_itemActive = value;

        if (UseBothHands)
        {
            m_animator.SetBool("HoldingB", value);
            m_animator.SetBool("HoldingL", false);
            m_animator.SetBool("HoldingR", false);
        }
        else
        {
            m_animator.SetBool(m_holding, value);
            m_animator.SetBool("HoldingB", false);
        }

        m_animator.SetBool("HoldingWeapon", isWeapon);
        if (!value)
        {
            m_animator.SetInteger("GunType", -1);
            m_animator.SetLayerWeight(1, 0);
            m_animator.SetLayerWeight(2, 0);
        }
    }

    private void InteractUpdate()
    {
        switch (m_interactionButtonType)
        {
            case InputButtonType.Mouse: m_currentInteract = Input.GetMouseButton(m_interactionMouseButton); break;
            case InputButtonType.Button: m_currentInteract = Input.GetButton(m_interactionButton); break;
            case InputButtonType.Key: m_currentInteract = Input.GetKey(m_interactionKey); break;
        }

        if (m_currentInteract && !m_previousInteract)
        {
            m_animator.SetTrigger(m_interact);
            m_animator.SetBool("Animating", true);

            if (LoopingAnimationStanding &&
                m_croucher.IsStanding)
            {
                m_animator.SetBool("InteractionLoop", true);
                Invoke("StopInteractionLoop", m_loopingDuration + m_interactionStandingStartDuration);
            }
            else if (LoopingAnimationCrouching &&
                !m_croucher.IsStanding)
            {
                m_animator.SetBool("InteractionLoop", true);
                Invoke("StopInteractionLoop", m_loopingDuration + m_interactionCrouchingStartDuration);
            }
        }

        m_previousInteract = m_currentInteract;
        m_currentInteract = false;
    }

    private void StopInteractionLoop() { m_animator.SetBool("InteractionLoop", false); }

    private void ItemDropUpdate()
    {
        switch (m_dropItemButtonType)
        {
            case InputButtonType.Mouse: m_currentDrop = Input.GetMouseButton(m_dropItemMouseButton); break;
            case InputButtonType.Button: m_currentDrop = Input.GetButton(m_dropItemButton); break;
            case InputButtonType.Key: m_currentDrop = Input.GetKey(m_dropItemKey); break;
        }

        if (m_currentDrop && !m_previousDrop)
        {
            if (m_hand == Hand.Left && m_itemHoldLogic.m_itemInHandL != null) { StartCoroutine(DropItem(1.2f)); }
            if (m_hand == Hand.Right && m_itemHoldLogic.m_itemInHandR != null) { StartCoroutine(DropItem(1.2f)); }
        }

        m_previousDrop = m_currentDrop;
        m_currentDrop = false;
    }

    private void ItemToggleUpdate()
    {
        switch (m_toggleItemButtonType)
        {
            case InputButtonType.Mouse: m_currentToggle = Input.GetMouseButton(m_toggleItemMouseButton); break;
            case InputButtonType.Button: m_currentToggle = Input.GetButton(m_toggleItemButton); break;
            case InputButtonType.Key: m_currentToggle = Input.GetKey(m_toggleItemKey); break;
        }

        if (m_currentToggle && !m_previousToggle)
        {
            if (m_hand == Hand.Left && m_itemHoldLogic.m_itemInHandL != null) { ToggleItem(); }
            if (m_hand == Hand.Right && m_itemHoldLogic.m_itemInHandR != null) { ToggleItem(); }
        }

        m_previousToggle = m_currentToggle;
        m_currentToggle = false;
    }

    public void ToggleItem()
    {
        if (!m_itemActive) { StartCoroutine(PullItemFromPocket(0.1f)); }
        else { StartCoroutine(PutItemInPocket(0.3f)); }
    }

    private IEnumerator DropItem(float waitTime)
    {
        m_itemActive = false;
        m_animator.SetTrigger(m_dropItem);
        m_animator.SetBool("Animating", true);
        SetHolding(false, false);

        yield return new WaitForSeconds(waitTime);

        if (m_itemHoldLogic.m_itemInHandR != null && m_hand == Hand.Right) { m_itemHoldLogic.Drop(m_hand); }
        if (m_itemHoldLogic.m_itemInHandL != null && m_hand == Hand.Left) { m_itemHoldLogic.Drop(m_hand); }
    }

    private IEnumerator PutItemInPocket(float waitTime)
    {
        m_itemActive = false;
        m_animator.SetBool("Animating", true);
        SetHolding(false, false);

        yield return new WaitForSeconds(waitTime);

        m_itemHoldLogic.Toggle(m_hand);
    }

    private IEnumerator PullItemFromPocket(float waitTime)
    {
        m_itemActive = true;
        m_animator.SetBool("Animating", true);

        yield return new WaitForSeconds(waitTime);

        m_itemHoldLogic.Toggle(m_hand);
    }
}
