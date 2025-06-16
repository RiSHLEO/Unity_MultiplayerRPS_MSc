using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        if(player.MoveInput.x == 0 && player.MoveInput.y == 0)
            stateMachine.ChangeState(player.IdleState);

        player.SetVelocity(player.MoveInput.x * player.MoveSpeed, player.MoveInput.y * player.MoveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
