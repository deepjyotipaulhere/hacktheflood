using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum HandSide
    {
        Left,
        Right
    }

    [SerializeField] private HandSide m_side = HandSide.Left;

    public HandSide Side
    {
        get
        {
            return m_side;
        }
#if UNITY_EDITOR
        set
        {
            if(UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError("Hand.Side is not accessible in play mode!"); return;
            }
            m_side = value;
        }
#endif
    }
}
