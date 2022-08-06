using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateListener : StateMachineBehaviour
{


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("AnimatorStateListener: OnStateMachineEnter :" + animator.GetCurrentAnimatorClipInfo(layerIndex));
    }
}
