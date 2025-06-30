using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;
    protected float stateTimer;

    protected PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.Anim;
        rb = player.rb;
        input = player._input;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        if (input.Player.Ability.WasPressedThisFrame())
            stateMachine.ChangeState(player.AbilityState);
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }
}