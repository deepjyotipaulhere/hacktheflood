using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDemoSettings : MonoBehaviour
{
    [System.Serializable]
    private struct SetAnimatorBool
    {
        public string BoolName;
        public bool Value;
    }

    [System.Serializable]
    private struct SetAnimatorTrigger
    {
        public string TriggerName;
        public float Delay;
    }

    [SerializeField] private Strafer m_characterStrafer = null;
    [SerializeField] private Animator m_animator = null;

    [Space(10)]
    [SerializeField] private bool m_canMove = true;
    [SerializeField] private SetAnimatorBool[] m_animatorBools = null;
    [SerializeField] private SetAnimatorTrigger[] m_animatorTriggers = null;

    private void Awake()
    {
        if (m_characterStrafer == null) { m_characterStrafer = GetComponent<Strafer>(); }
        if (m_animator == null) { m_animator = GetComponent<Animator>(); }

        m_characterStrafer.CanMove = m_canMove;

        foreach (SetAnimatorBool b in m_animatorBools)
        {
            m_animator.SetBool(b.BoolName, b.Value);
        }

        foreach (SetAnimatorTrigger t in m_animatorTriggers)
        {
            StartCoroutine(TriggerAnimation(t.Delay, t.TriggerName));
        }
    }


    private IEnumerator TriggerAnimation(float delay, string name)
    {
        while (true)
        {
            m_animator.SetTrigger(name);
            yield return new WaitForSeconds(delay);
            if (delay <= 0f) { yield break; }
        }
    }
}
