using UnityEngine;

public class CoreRushTargetStrategy : EnemyTargetStrategy
{
    public override Transform SelectTarget(
        Transform enemyTransform,
        Transform playerTarget,
        Transform coreTarget
    )
    {
        if (coreTarget != null)
            return coreTarget;

        return playerTarget;
    }
}