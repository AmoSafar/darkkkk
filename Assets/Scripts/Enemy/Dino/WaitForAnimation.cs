using UnityEngine;
using System.Collections;

public class WaitForAnimation : CustomYieldInstruction
{
    private Animator animator;
    private int layer;
    private string stateName;

    public WaitForAnimation(Animator animator, string stateName, int layer = 0)
    {
        this.animator = animator;
        this.stateName = stateName;
        this.layer = layer;
    }

    public override bool keepWaiting
    {
        get
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            return !info.IsName(stateName) || info.normalizedTime < 1.0f;
        }
    }
}
