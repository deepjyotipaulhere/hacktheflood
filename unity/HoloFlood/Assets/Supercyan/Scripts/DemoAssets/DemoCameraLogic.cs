using System.Collections.Generic;
using UnityEngine;

public class DemoCameraLogic : MonoBehaviour
{
    private Transform m_currentTarget = null;
    private float m_distance = 4.0f;
    private float m_height = 1;

    [SerializeField] private List<Transform> m_targets = null;
    private int m_currentIndex = 0;

    private Vector3 m_correctPosition = Vector3.zero;
    private Quaternion m_correctRotation = Quaternion.identity;

    [SerializeField] private ThirdPersonCamera m_cameraController = null;

    private void ToggleRenderers(Transform trans, bool enabled = true)
    {
        Renderer[] renderers = trans.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = enabled;
        }

        CapsuleCollider collider = trans.gameObject.GetComponent<CapsuleCollider>();
        collider.enabled = enabled;

        Rigidbody rigidbody = trans.gameObject.GetComponent<Rigidbody>();
        rigidbody.useGravity = enabled;
        rigidbody.isKinematic = !enabled;
    }

    private void ToggleTarget(GameObject gameObject, bool enabled = true)
    {
        gameObject.SetActive(enabled);
    }

    private void Start()
    {
        if (m_targets.Count > 0)
        {
            m_currentIndex = 0;
            for (int i = 0; i < m_targets.Count; i++)
            {
                ToggleRenderers(m_targets[i], false);
            }
            m_currentTarget = m_targets[m_currentIndex];
            ToggleRenderers(m_currentTarget, true);
        }
    }

    public void PreviousTarget()
    {
        m_correctPosition = m_targets[m_currentIndex].position;
        m_correctRotation = m_targets[m_currentIndex].rotation;

        ChangeTarget(-1);
    }

    public void NextTarget()
    {
        m_correctPosition = m_targets[m_currentIndex].position;
        m_correctRotation = m_targets[m_currentIndex].rotation;

        ChangeTarget(1);
    }

    private void ChangeTarget(int step)
    {
        m_currentIndex += step;
        if (m_currentIndex == m_targets.Count) { m_currentIndex = 0; }
        else if (m_currentIndex < 0) { m_currentIndex = m_targets.Count - 1; }
        for (int i = 0; i < m_targets.Count; i++)
        {
            ToggleRenderers(m_targets[i], false);
        }

        m_currentTarget = m_targets[m_currentIndex];
        m_currentTarget.position = m_correctPosition;
        m_currentTarget.rotation = m_correctRotation;
        ToggleRenderers(m_currentTarget, true);
        m_cameraController.ChangeCharacter(m_currentTarget.gameObject);
    }

    private void LateUpdate()
    {
        if (m_currentTarget == null) { return; }

        float targetHeight = m_currentTarget.position.y + m_height;

        Quaternion currentRotation = Quaternion.Euler(0, 0, 0);

        Vector3 position = m_currentTarget.position;

        position -= currentRotation * Vector3.forward * m_distance;

        position.y = targetHeight;

        transform.position = position;
        transform.LookAt(m_currentTarget.position + new Vector3(0, m_height, 0));
        transform.Rotate(new Vector3(0, 0, 0));
    }
}
