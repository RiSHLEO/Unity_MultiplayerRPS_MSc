using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Dash Ability")]
public abstract class AbilitySO : ScriptableObject
{
    public float Cooldown = 2f;
    public float Duration = 0.2f;
    private float _lastUsedTime = -Mathf.Infinity;

    public bool IsOnCooldown => Time.time < _lastUsedTime + Cooldown;

    public void TryActivate(Player player)
    {
        if (!IsOnCooldown)
        {
            ActivateAbility(player);
            _lastUsedTime = Time.time;
        }
        else
        {
            Debug.Log($"{name} is on cooldown.");
        }
    }

    public abstract void ActivateAbility(Player player);
}
