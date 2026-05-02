using UnityEngine;

public abstract class EnemyTargetStrategy : MonoBehaviour, IEnemyTargetStrategy
{
    public abstract Transform SelectTarget(
        Transform enemyTransform,
        Transform playerTarget,
        Transform coreTarget
    );

    protected float GetHorizontalDistance(Transform a, Transform b)
    {
        if (a == null || b == null)
            return float.MaxValue;

        Vector3 aPosition = a.position;
        Vector3 bPosition = b.position;

        aPosition.y = 0f;
        bPosition.y = 0f;

        return Vector3.Distance(aPosition, bPosition);
    }
}