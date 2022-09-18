using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverridesApplier
{
    private void SetLoopingConfigStanding(CharacterItemAnimator itemScript, AnimationClip interactionLoop, AnimationClip interactionStart, float interactionLoopTime)
    {
        if (interactionLoop != null)
        {
            itemScript.LoopingAnimationStanding = true;
            if (interactionStart != null)
            {
                itemScript.InteractionStandingStartDuration = interactionStart.length;
                itemScript.LoopingDuration = interactionLoopTime;
            }
        }
        else { itemScript.LoopingAnimationStanding = false; }
    }
    private void SetLoopingConfigCrouching(CharacterItemAnimator itemScript, AnimationClip interactionLoop, AnimationClip interactionStart, float interactionLoopTime)
    {
        if (interactionLoop != null)
        {
            itemScript.LoopingAnimationCrouching = true;
            if (interactionStart != null)
            {
                itemScript.InteractionCrouchingStartDuration = interactionStart.length;
                itemScript.LoopingDuration = interactionLoopTime;
            }
        }
        else { itemScript.LoopingAnimationCrouching = false; }
    }

    private void SetInteractionAnimations(AnimationKey.Hand hand, bool crouching, AnimationClip start, AnimationClip loop, AnimationClip end)
    {
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Interaction, hand, crouching, AnimationKey.Phase.Start), start);
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Interaction, hand, crouching, AnimationKey.Phase.Loop), loop);
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Interaction, hand, crouching, AnimationKey.Phase.End), end);
    }

    private void SetHoldingAnimations(AnimationKey.Hand hand, AnimationClip holding, AnimationClip crouchingHolding, AnimationClip equip, AnimationClip unequip)
    {
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Holding, hand, false), holding);
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Holding, hand, true), crouchingHolding);
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.Equip, hand), equip);
        OverrideAnimation(new AnimationKey(AnimationKey.Animation.UnEquip, hand), unequip);
    }

    private void SetRightHandItemAnimations(CharacterItemAnimator itemScript, ItemAnimationsObject itemAnimations, bool useBothHands)
    {
        SetLoopingConfigStanding(itemScript, itemAnimations.InteractionRightLoop, itemAnimations.InteractionRightStart, itemAnimations.InteractionRightLoopTime);
        SetLoopingConfigCrouching(itemScript, itemAnimations.CrouchingInteractionRightLoop, itemAnimations.CrouchingInteractionRightStart, itemAnimations.CrouchingInteractionRightLoopTime);

        SetInteractionAnimations(AnimationKey.Hand.Right, false, itemAnimations.InteractionRightStart, itemAnimations.InteractionRightLoop, itemAnimations.InteractionRightEnd);
        SetInteractionAnimations(AnimationKey.Hand.Right, true, itemAnimations.CrouchingInteractionRightStart, itemAnimations.CrouchingInteractionRightLoop, itemAnimations.CrouchingInteractionRightEnd);
        SetHoldingAnimations(AnimationKey.Hand.Right, itemAnimations.HoldingRight, itemAnimations.CrouchingHoldingRight, itemAnimations.EquipRight, itemAnimations.UnEquipRight);

        if (useBothHands)
        {
            SetInteractionAnimations(AnimationKey.Hand.Left, false, itemAnimations.InteractionRightStart, itemAnimations.InteractionRightLoop, itemAnimations.InteractionRightEnd);
            SetInteractionAnimations(AnimationKey.Hand.Left, true, itemAnimations.CrouchingInteractionRightStart, itemAnimations.CrouchingInteractionRightLoop, itemAnimations.CrouchingInteractionRightEnd);
            SetHoldingAnimations(AnimationKey.Hand.Both, itemAnimations.HoldingRight, itemAnimations.CrouchingHoldingRight, itemAnimations.EquipRight, itemAnimations.UnEquipRight);
        }
    }

    private void SetLeftHandItemAnimations(CharacterItemAnimator itemScript, ItemAnimationsObject itemAnimations, bool useBothHands)
    {
        SetLoopingConfigStanding(itemScript, itemAnimations.InteractionLeftLoop, itemAnimations.InteractionLeftStart, itemAnimations.InteractionLeftLoopTime);
        SetLoopingConfigCrouching(itemScript, itemAnimations.CrouchingInteractionLeftLoop, itemAnimations.CrouchingInteractionLeftStart, itemAnimations.CrouchingInteractionLeftLoopTime);

        SetInteractionAnimations(AnimationKey.Hand.Left, false, itemAnimations.InteractionLeftStart, itemAnimations.InteractionLeftLoop, itemAnimations.InteractionLeftEnd);
        SetInteractionAnimations(AnimationKey.Hand.Left, true, itemAnimations.CrouchingInteractionLeftStart, itemAnimations.CrouchingInteractionLeftLoop, itemAnimations.CrouchingInteractionLeftEnd);
        SetHoldingAnimations(AnimationKey.Hand.Left, itemAnimations.HoldingLeft, itemAnimations.CrouchingHoldingLeft, itemAnimations.EquipLeft, itemAnimations.UnEquipLeft);

        if (useBothHands)
        {
            SetInteractionAnimations(AnimationKey.Hand.Right, false, itemAnimations.InteractionLeftStart, itemAnimations.InteractionLeftLoop, itemAnimations.InteractionLeftEnd);
            SetInteractionAnimations(AnimationKey.Hand.Right, true, itemAnimations.CrouchingInteractionLeftStart, itemAnimations.CrouchingInteractionLeftLoop, itemAnimations.CrouchingInteractionLeftEnd);
            SetHoldingAnimations(AnimationKey.Hand.Both, itemAnimations.HoldingLeft, itemAnimations.CrouchingHoldingLeft, itemAnimations.EquipLeft, itemAnimations.UnEquipLeft);
        }
    }

    private void ClearAnimations(AnimationKey.Hand hand)
    {
        SetInteractionAnimations(hand, false, null, null, null);
        SetInteractionAnimations(hand, true, null, null, null);
        SetHoldingAnimations(hand, null, null, null, null);
    }

    private List<KeyValuePair<AnimationClip, AnimationClip>> m_getOverridesCache;
    private Dictionary<string, KeyValuePair<AnimationClip, AnimationClip>> m_overrides;

    private void PopulateOverrides(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
    {
        if (m_overrides == null)
        {
            m_overrides = new Dictionary<string, KeyValuePair<AnimationClip, AnimationClip>>(AnimationKey.CAPACITY);
        }
        else
        {
            m_overrides.Clear();
        }

        for (int i = 0; i < overrides.Count; i++)
        {
            KeyValuePair<AnimationClip, AnimationClip> @override = overrides[i];
            string keyString = @override.Key.name;
            if (AnimationKey.IsRelevant(keyString))
            {
                m_overrides.Add(keyString, @override);
            }
        }
    }

    public void SetCorrectAnimations(AnimatorOverrideController animator, ItemLogic itemInHandR, ItemLogic itemInHandL, CharacterItemAnimator itemScriptR, CharacterItemAnimator itemScriptL)
    {
        if (m_getOverridesCache == null)
        {
            m_getOverridesCache = new List<KeyValuePair<AnimationClip, AnimationClip>>(animator.overridesCount);
        }
        else
        {
            m_getOverridesCache.Clear();
        }

        animator.GetOverrides(m_getOverridesCache);
        PopulateOverrides(m_getOverridesCache);

        if (itemInHandR)
        {
            if (itemInHandR.ItemAnimations)
            {
                ItemAnimationsObject itemAnimations = itemInHandR.ItemAnimations;
                SetRightHandItemAnimations(itemScriptR, itemAnimations, itemInHandR.UseBothHands);
            }
            else
            {
                ClearAnimations(AnimationKey.Hand.Right);
            }
        }

        if (itemInHandL)
        {
            if (itemInHandL.ItemAnimations)
            {
                ItemAnimationsObject itemAnimations = itemInHandL.ItemAnimations;
                SetLeftHandItemAnimations(itemScriptL, itemAnimations, itemInHandL.UseBothHands);
            }
            else
            {
                ClearAnimations(AnimationKey.Hand.Left);
            }
        }

        animator.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>(m_overrides.Values));
    }

    private void OverrideAnimation(AnimationKey key, AnimationClip animation)
    {
        string keyString = key.ToString();

        KeyValuePair<AnimationClip, AnimationClip> @override;
        if (m_overrides.TryGetValue(keyString, out @override))
        {
            m_overrides[keyString] = new KeyValuePair<AnimationClip, AnimationClip>(@override.Key, animation);
        }
        else
        {
            Debug.LogWarningFormat("Couldn't find {0} in the animator", keyString);
        }
    }

    private struct AnimationKey : IEquatable<AnimationKey>
    {
        public enum Animation { Interaction, Holding, Equip, UnEquip }
        public enum Hand { Right, Left, Both }
        public enum Phase { Start, End, Loop, None }

        private Animation m_animation;
        private Hand m_hand;
        private bool m_crouching;
        private Phase m_phase;

        public AnimationKey(Animation animation, Hand hand, bool crouching = false, Phase phase = Phase.None)
        {
            if (hand == Hand.Both && animation == Animation.Interaction) { throw new System.Exception("Both hands not supported for interaction animation"); }
            if (phase != Phase.None && animation != Animation.Interaction) { throw new System.Exception("Phases are only supported for interaction animations"); }
            if (crouching && (animation == Animation.UnEquip || animation == Animation.Equip)) { throw new System.Exception("Crouching is not supported for equip and unequip animations"); }

            m_animation = animation;
            m_hand = hand;
            m_crouching = crouching;
            m_phase = phase;
        }

        public bool Equals(AnimationKey key)
        {
            return m_animation == key.m_animation &&
                    m_hand == key.m_hand &&
                    m_crouching == key.m_crouching &&
                    m_phase == key.m_phase;
        }

        public override int GetHashCode()
        {
            int hashCode = 1879084962;
            hashCode = hashCode * -1521134295 + m_animation.GetHashCode();
            hashCode = hashCode * -1521134295 + m_hand.GetHashCode();
            hashCode = hashCode * -1521134295 + m_crouching.GetHashCode();
            hashCode = hashCode * -1521134295 + m_phase.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            string keyString;
            if (_stringCache.TryGetValue(this, out keyString))
            {
                return keyString;
            }
            else
            {
                keyString = ITEM + GetAnimation(m_animation) + (m_crouching ? CROUCH : string.Empty) + GetHand(m_hand) + GetPhase(m_phase);

                _stringCache.Add(this, keyString);
                return keyString;
            }
        }

        private static string GetPhase(Phase phase)
        {
            switch (phase)
            {
                case Phase.End: return END;
                case Phase.Start: return START;
                case Phase.Loop: return LOOP;
                case Phase.None: return string.Empty;
                default: throw new System.Exception("Unsupported phase " + phase);
            }
        }

        private static string GetAnimation(Animation animation)
        {
            switch (animation)
            {
                case Animation.Interaction: return INTERACTION;
                case Animation.Holding: return HOLD;
                case Animation.Equip: return EQUIP;
                case Animation.UnEquip: return UNEQUIP;
                default: throw new System.Exception("Unsupported animation " + animation);
            }
        }

        private static string GetHand(Hand hand)
        {
            switch (hand)
            {
                case Hand.Right: return RIGHT;
                case Hand.Left: return LEFT;
                case Hand.Both: return BOTH;
                default: throw new System.Exception("Unsupported hand " + hand);
            }
        }

        public static bool IsRelevant(string key)
        {
            if (_all.Count == 0) { PopulateAll(); }
            return _all.Contains(key);
        }

        private static void PopulateAll()
        {
            if (_all.Count == 0)
            {
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, false, Phase.Start).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, false, Phase.Start).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, false, Phase.Loop).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, false, Phase.Loop).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, false, Phase.End).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, false, Phase.End).ToString());

                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, true, Phase.Start).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, true, Phase.Start).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, true, Phase.Loop).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, true, Phase.Loop).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Right, true, Phase.End).ToString());
                _all.Add(new AnimationKey(Animation.Interaction, Hand.Left, true, Phase.End).ToString());

                _all.Add(new AnimationKey(Animation.Holding, Hand.Right, false).ToString());
                _all.Add(new AnimationKey(Animation.Holding, Hand.Left, false).ToString());
                _all.Add(new AnimationKey(Animation.Holding, Hand.Both, false).ToString());

                _all.Add(new AnimationKey(Animation.Holding, Hand.Right, true).ToString());
                _all.Add(new AnimationKey(Animation.Holding, Hand.Left, true).ToString());
                _all.Add(new AnimationKey(Animation.Holding, Hand.Both, true).ToString());

                _all.Add(new AnimationKey(Animation.Equip, Hand.Right).ToString());
                _all.Add(new AnimationKey(Animation.Equip, Hand.Left).ToString());
                _all.Add(new AnimationKey(Animation.Equip, Hand.Both).ToString());

                _all.Add(new AnimationKey(Animation.UnEquip, Hand.Right).ToString());
                _all.Add(new AnimationKey(Animation.UnEquip, Hand.Left).ToString());
                _all.Add(new AnimationKey(Animation.UnEquip, Hand.Both).ToString());
            }
        }

        private static readonly Dictionary<AnimationKey, string> _stringCache = new Dictionary<AnimationKey, string>(CAPACITY);
        private static readonly HashSet<string> _all = new HashSet<string>();

        private const string ITEM = "_Item";
        private const string INTERACTION = "Interaction";
        private const string END = "End";
        private const string START = "Start";
        private const string LOOP = "Loop";
        private const string HOLD = "Hold";
        private const string EQUIP = "Equip";
        private const string UNEQUIP = "UnEquip";
        private const string CROUCH = "Crouching";
        private const string RIGHT = "R";
        private const string LEFT = "L";
        private const string BOTH = "B";

        public const int CAPACITY = 24;
    }
}
