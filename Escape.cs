using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Escape")]
public class Escape : Effect
{
    public override void Apply(CardRuntime card, CombatManager target)
    {
        // no usado
    }

    public override void ApplyToEnemy(CardRuntime card, Enemy enemy)
    {
        Execute(enemy, enemy.PCM);
    }

    public void Execute(Enemy enemy, CombatManager pcm)
    {
        Debug.Log($"{enemy.name} escapa del combate");

        enemy.IsDead = true; // importante
        GameObject.Destroy(enemy.gameObject);
    }
}