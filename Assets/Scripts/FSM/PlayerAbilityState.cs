using System.Collections;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1;
        player.StartCoroutine(AbilityRoutine());
    }

    private IEnumerator AbilityRoutine()
    {
        player.CurrentForm.Ability.TryActivate(player);
        yield return new WaitForSeconds(player.CurrentForm.Ability.Duration);
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
            stateMachine.ChangeState(player.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
