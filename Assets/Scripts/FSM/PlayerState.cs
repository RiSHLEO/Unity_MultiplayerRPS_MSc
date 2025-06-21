using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;

    protected PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.Anim;
        rb = player.rb;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }
}