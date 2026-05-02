using UnityEngine;

public class PlayerPriorityTargetStrategy : EnemyTargetStrategy
{
    [Header("Player Priority Settings")]
    [SerializeField] private float playerDetectionRange = 6f;

    public override Transform SelectTarget(
        Transform enemyTransform,
        Transform playerTarget,
        Transform coreTarget
    )
    {
        if (playerTarget == null)
            return coreTarget;

        if (coreTarget == null)
            return playerTarget;

        float distanceToPlayer = GetHorizontalDistance(enemyTransform, playerTarget);

        if (distanceToPlayer <= playerDetectionRange)
            return playerTarget;

        return coreTarget;
    }
}