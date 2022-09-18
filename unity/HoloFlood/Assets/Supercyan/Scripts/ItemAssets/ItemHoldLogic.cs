using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemHoldLogic : MonoBehaviour, IInitializable
{
    public void Initialize(GameObject character)
    {
        if (m_croucher == null) { m_croucher = character.GetComponent<Croucher>(); }
        if (m_aimScript == null) { m_aimScript = character.GetComponent<CharacterWeaponAnimator>(); }
        if (m_itemScriptR == null || m_itemScriptL == null)
        {
            CharacterItemAnimator[] animators = character.GetComponents<CharacterItemAnimator>();
            foreach (CharacterItemAnimator a in animators)
            {
                if (a.ThisHand == CharacterItemAnimator.Hand.Right) { m_itemScriptR = a; }
                else if (a.ThisHand == CharacterItemAnimator.Hand.Left) { m_itemScriptL = a; }
            }
        }

        Hand[] hands = character.GetComponentsInChildren<Hand>();

        for (int j = 0; j < hands.Length; j++)
        {
            switch (hands[j].Side)
            {
                case Hand.HandSide.Left:
                    m_handBoneL = hands[j].transform;
                    break;

                case Hand.HandSide.Right:
                    m_handBoneR = hands[j].transform;
                    break;

                default:
                    break;
            }
        }

        Animator animator = GetComponent<Animator>();
        m_animator = new AnimatorOverrideController(animator.runtimeAnimatorController);
        m_animator.name = animator.runtimeAnimatorController.name;
        animator.runtimeAnimatorController = m_animator;

        m_animatorOverridesApplier = new AnimatorOverridesApplier();
    }

    private void Awake()
    {
        Initialize(gameObject);
    }

    [SerializeField] private Croucher m_croucher;

    [SerializeField] private CharacterWeaponAnimator m_aimScript;
    [SerializeField] private CharacterItemAnimator m_itemScriptL;
    [SerializeField] private CharacterItemAnimator m_itemScriptR;

    [SerializeField] private Transform m_handBoneL;
    [SerializeField] private Transform m_handBoneR;

    private AnimatorOverrideController m_animator;
    private AnimatorOverridesApplier m_animatorOverridesApplier;

    public CharacterWeaponAnimator AimScript { set { m_aimScript = value; } }
    
    public Transform HandBoneR
    {
        get { return m_handBoneR; }
        set { m_handBoneR = value; }
    }
    public Transform HandBoneL
    {
        get { return m_handBoneL; }
        set { m_handBoneL = value; }
    }

    public ItemLogic m_itemInHandL;
    public ItemLogic m_itemInHandR;

    private bool m_itemUsesBothHands = false;
    private bool m_isHoldingWeapon = false;

    private void Start()
    {
        if (!m_handBoneR || !m_handBoneL)
        {
            Debug.LogError("Handbones not set. Can't hold items.");
            return;
        }

        if (m_itemInHandR == null && m_itemInHandL == null)
        {
            if (m_aimScript) { m_aimScript.SetGunInHand(false, -1); }
        }

        CheckHands();
        if (m_itemInHandR) { AttachItem(m_itemInHandR, CharacterItemAnimator.Hand.Right); }
        if (m_itemInHandL) { AttachItem(m_itemInHandL, CharacterItemAnimator.Hand.Left); }
    }

    private void CheckHands()
    {
        ItemLogic right = m_itemInHandR;
        ItemLogic left = m_itemInHandL;
        ItemLogic either = null;

        if (right)
        {
            if (right.UseBothHands && m_itemInHandL)
            {
                Drop(CharacterItemAnimator.Hand.Left);
                left = null;
            }

            if (right.PreferredHand == ItemLogic.HandPreference.Right) { m_itemInHandR = right; }
            else if (right.PreferredHand == ItemLogic.HandPreference.Left)
            {
                m_itemInHandL = right;
                m_itemInHandR = null;
            }
            else if (right.PreferredHand == ItemLogic.HandPreference.Either)
            {
                m_itemInHandR = right;
                either = right;
            }
        }

        if (left)
        {
            if (left.UseBothHands)
            {
                if (m_itemInHandR) { Drop(CharacterItemAnimator.Hand.Right); }
                if (m_itemInHandL) { Drop(CharacterItemAnimator.Hand.Left); }
                either = null;
            }

            if (left.PreferredHand == ItemLogic.HandPreference.Left) { m_itemInHandL = left; }
            else if (left.PreferredHand == ItemLogic.HandPreference.Right)
            {
                if (m_itemInHandR == null) { m_itemInHandR = left; }
                else { Drop(CharacterItemAnimator.Hand.Left); }

                if (either != null)
                {
                    m_itemInHandR = left;
                    m_itemInHandL = either;
                }
            }
            else if (left.PreferredHand == ItemLogic.HandPreference.Either)
            {
                if (m_itemInHandL != null && m_itemInHandL != left) { m_itemInHandR = left; }
                else { m_itemInHandL = left; }
            }
        }
    }

    public void AttachItem(ItemLogic item, CharacterItemAnimator.Hand handToAttach)
    {
        if (item.PreferredHand == ItemLogic.HandPreference.Left) { handToAttach = CharacterItemAnimator.Hand.Left; }
        else if (item.PreferredHand == ItemLogic.HandPreference.Right) { handToAttach = CharacterItemAnimator.Hand.Right; }

        if (m_itemUsesBothHands || item.UseBothHands)
        {
            if (m_itemInHandL && m_itemInHandL != item) { Drop(CharacterItemAnimator.Hand.Left); }
            if (m_itemInHandR && m_itemInHandR != item) { Drop(CharacterItemAnimator.Hand.Right); }
        }

        if (item == m_itemInHandL && handToAttach == CharacterItemAnimator.Hand.Left) { m_itemInHandL = item; }
        else if (item == m_itemInHandR && handToAttach == CharacterItemAnimator.Hand.Right) { m_itemInHandR = item; }
        else if (m_itemInHandL == null && handToAttach == CharacterItemAnimator.Hand.Left) { m_itemInHandL = item; }
        else if (m_itemInHandR == null && handToAttach == CharacterItemAnimator.Hand.Right) { m_itemInHandR = item; }
        else if (item.PreferredHand == ItemLogic.HandPreference.Right)
        {
            if (m_itemInHandR) { Drop(CharacterItemAnimator.Hand.Right); }
            m_itemInHandR = item;
        }
        else if (item.PreferredHand == ItemLogic.HandPreference.Left ||
                 item.PreferredHand == ItemLogic.HandPreference.Either)
        {
            if (m_itemInHandL) { Drop(CharacterItemAnimator.Hand.Left); }
            m_itemInHandL = item;
        }
        CheckHands();

        bool isWeapon = false;
        if (item.ItemAnimations) { isWeapon = item.ItemAnimations.IsWeapon; }

        m_itemUsesBothHands = item.UseBothHands;
        if (m_itemScriptR != null && item == m_itemInHandR) { m_itemScriptR.UseBothHands = m_itemUsesBothHands; }
        else if (m_itemScriptL != null && item == m_itemInHandL) { m_itemScriptL.UseBothHands = m_itemUsesBothHands; }

        if (item == m_itemInHandR) { Attach(item, m_handBoneR); }
        else if (item == m_itemInHandL) { Attach(item, m_handBoneL); }

        if (m_aimScript)
        {
            if (item.ItemTypeID <= 2) { m_aimScript.SetGunInHand(true, item.ItemTypeID); }
            else { m_aimScript.SetGunInHand(false, -1); }
        }

        if (m_itemScriptL || m_itemScriptR) { m_animatorOverridesApplier.SetCorrectAnimations(m_animator,m_itemInHandR, m_itemInHandL, m_itemScriptR, m_itemScriptL); }
    }

    private void AttachToHand(ItemLogic item, CharacterItemAnimator itemScript, ref ItemLogic itemInHand)
    {
        if(itemScript != null)
        {
            if(item.ItemAnimations) { m_isHoldingWeapon = item.ItemAnimations.IsWeapon; }
            itemScript.SetHolding(true, m_isHoldingWeapon);
            itemInHand = item;
        }
    }

    private void Attach(ItemLogic item, Transform hand)
    {
        Transform itemTransform = item.transform;

        itemTransform.parent = null; //Detach
        itemTransform.localScale = Vector3.one; //Reset scale

        itemTransform.parent = hand; //Parent

        itemTransform.localPosition = Vector3.zero; //Reset position
        itemTransform.localRotation = Quaternion.identity; //and rotation

        if(hand == HandBoneL)
        {
            FlipScale(itemTransform, item.FlipXForHand == ItemLogic.FlipHand.Left, item.FlipYForHand == ItemLogic.FlipHand.Left, item.FlipZForHand == ItemLogic.FlipHand.Left);
            AttachToHand(item, m_itemScriptL, ref m_itemInHandL);
        }
        else if(hand == HandBoneR)
        {
            FlipScale(itemTransform, item.FlipXForHand == ItemLogic.FlipHand.Right, item.FlipYForHand == ItemLogic.FlipHand.Right, item.FlipZForHand == ItemLogic.FlipHand.Right);
            AttachToHand(item, m_itemScriptR, ref m_itemInHandR);
        }
        
        Transform dummyPoint = item.DummyPoint;
        if (dummyPoint != null)
        {
            itemTransform.localRotation = dummyPoint.localRotation; //Rotate using dummy rotation

            Vector3 dummyHandDelta = dummyPoint.position - hand.position;
            itemTransform.position -= dummyHandDelta; //Use dummy as pivot
        }

        item.OnPickup();
    }
    private static void FlipScale(Transform transform, bool x, bool y, bool z)
    {
        Vector3 newLocalScale = transform.localScale;
        if (x) { newLocalScale.x = -newLocalScale.x; }
        if (y) { newLocalScale.y = -newLocalScale.y; }
        if (z) { newLocalScale.z = -newLocalScale.z; }
        transform.localScale = newLocalScale;
    }

    public void Drop(CharacterItemAnimator.Hand hand)
    {
        if (hand == CharacterItemAnimator.Hand.Right)
        {
            ItemLogic itemLogic = m_itemInHandR.GetComponent<ItemLogic>();
            if (m_itemInHandR.transform.parent == HandBoneR)
            {
                //Only null parent if actually attached to hand - this allows accessories to stay on if they are animated by item logic
                m_itemInHandR.transform.parent = null;
            }
            m_itemInHandR = null;
            if (itemLogic) { itemLogic.OnDrop(); }
        }
        else if (hand == CharacterItemAnimator.Hand.Left)
        {
            ItemLogic itemLogic = m_itemInHandL.GetComponent<ItemLogic>();
            if (m_itemInHandL.transform.parent == HandBoneL)
            {
                //Only null parent if actually attached to hand - this allows accessories to stay on if they are animated by item logic
                m_itemInHandL.transform.parent = null;
            }
            m_itemInHandL = null;
            if (itemLogic) { itemLogic.OnDrop(); }
        }
    }

    public void Toggle(CharacterItemAnimator.Hand hand)
    {
        if (hand == CharacterItemAnimator.Hand.Right)
        {
            if (m_itemInHandR.gameObject.activeSelf) { m_itemInHandR.gameObject.SetActive(false); }
            else
            {
                ItemLogic item = m_itemInHandR;
                item.gameObject.SetActive(true);
                m_itemInHandR = null;
                AttachItem(item, hand);
            }
        }
        else if (hand == CharacterItemAnimator.Hand.Left)
        {
            if (m_itemInHandL.gameObject.activeSelf) { m_itemInHandL.gameObject.SetActive(false); }
            else
            {
                ItemLogic item = m_itemInHandL;
                item.gameObject.SetActive(true);
                m_itemInHandL = null;
                AttachItem(item, hand);
            }
        }
    }


}
