using UnityEngine;

public class AnimationSetBool : StateMachineBehaviour
{

    [SerializeField] private string m_boolName = "";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(m_boolName, false);
    }
}
