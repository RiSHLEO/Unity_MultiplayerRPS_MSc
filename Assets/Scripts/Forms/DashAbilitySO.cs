using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash Ability")]
public class DashAbilitySO : AbilitySO
{
    [SerializeField] private float _dashForce = 50f;
    public override void ActivateAbility(Player player)
    {
        Debug.Log(player.photonView.name + " isDashing");

        Vector2 dashDirection = player.MoveInput.normalized;

        if(dashDirection == Vector2.zero)
            dashDirection = player.transform.right;

        player.rb.linearVelocity = dashDirection * _dashForce;
    }
}
