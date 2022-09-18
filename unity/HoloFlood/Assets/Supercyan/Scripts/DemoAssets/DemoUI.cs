using UnityEngine;

public class DemoUI : MonoBehaviour
{
    [SerializeField] private DemoCameraLogic m_cameraLogic = null;
    [SerializeField] private string[] m_instructions = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_cameraLogic.PreviousTarget();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_cameraLogic.NextTarget();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous character (Q)"))
        {
            m_cameraLogic.PreviousTarget();
        }

        if (GUILayout.Button("Next character (E)"))
        {
            m_cameraLogic.NextTarget();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(16);

        Color oldColor = GUI.color;
        GUI.color = Color.black;

        for (int i = 0; i < m_instructions.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_instructions[i]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUI.color = oldColor;

        GUILayout.EndVertical();
    }
}
