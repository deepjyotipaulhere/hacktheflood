#if UNITY_EDITOR

using UnityEngine;

[CreateAssetMenu()]
public class CharacterAppearanceObject : ScriptableObject
{
    [SerializeField] private GameObject m_model = null;

    public GameObject Model { get { return m_model; } }
}

#endif