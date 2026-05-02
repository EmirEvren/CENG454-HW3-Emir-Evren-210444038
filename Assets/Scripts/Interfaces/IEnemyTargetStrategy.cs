using UnityEngine;

public interface IEnemyTargetStrategy
{
    Transform SelectTarget(Transform enemyTransform, Transform playerTarget, Transform coreTarget);
}