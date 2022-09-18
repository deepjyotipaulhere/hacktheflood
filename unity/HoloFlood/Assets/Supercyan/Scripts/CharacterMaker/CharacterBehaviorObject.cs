#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterBehaviorObject : ScriptableObject
{
    [SerializeField] private RuntimeAnimatorController m_animatorController = null;
    [SerializeField] private MonoScript[] m_scripts = null;

    public RuntimeAnimatorController AnimatorController { get { return m_animatorController; } }
    public MonoScript[] Scripts { get { return m_scripts; } }

    public void InitializeBehaviour(GameObject character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (!animator) { animator = character.AddComponent<Animator>(); }

        animator.runtimeAnimatorController = m_animatorController;

        List<MonoBehaviour> createdScripts = new List<MonoBehaviour>();

        for (int i = 0; i < m_scripts.Length; i++)
        {
            if (m_scripts[i] == null)
            {
                Debug.LogError("The script is set to null!");
            }
            else
            {
                createdScripts.Add((MonoBehaviour)character.AddComponent(m_scripts[i].GetClass()));
            }
        }

        SetScriptConnections(character, createdScripts);
    }

    private void SetScriptConnections(GameObject character, List<MonoBehaviour> scripts)
    {
        for (int i = 0; i < scripts.Count; i++)
        {
            IInitializable initializable = scripts[i] as IInitializable;
            if (initializable != null)
            {
                initializable.Initialize(character);
            }
            else
            {
                Debug.LogError("The script is not initializable!");
            }
        }
    }
}

#endif