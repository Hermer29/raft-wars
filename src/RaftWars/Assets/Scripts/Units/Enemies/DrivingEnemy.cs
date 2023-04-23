using System;
using InputSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrivingEnemy : MonoBehaviour
{
    private Enemy _relatedEnemy;
    private PlayerService _player;
    private Vector3? _moveDirection;

    private const float SqrMagnitudeDistanceToReactOnPlayer = 10 * 10;
    private const int ExclusionSqrDistanceToPlayer = 300*2;

    public DrivingEnemy Construct(Enemy enemy, PlayerService playerService)
    {
        _player = playerService;
        _relatedEnemy = enemy;
        return this;
    }

    private void Update()
    {
        TeleportFromOutOfBounds();
            
        if(_relatedEnemy.InBattle == false)
            TryMoveEnemy(Time.deltaTime);
    }

    public void TeleportFromOutOfBounds()
    {
        bool IsOutOfBounds()
        {
            return (transform.position.x > 45f || transform.position.x < -45f) && (transform.position.z > 45f ||
                transform.position.z < -45f);
        }

        if (!IsOutOfBounds()) return;
        Vector3 pointOnBound = GetNearestPointOnBound();
        Vector3 vectorToCenter = (Vector3.zero - pointOnBound).normalized;
        const int distanceFromBounds = 5;
        _moveDirection = -_moveDirection;
        transform.position = vectorToCenter * distanceFromBounds + pointOnBound;
    }

    public void TryMoveEnemy(float deltaTime)
    {
        if(_relatedEnemy.boss5Stage)
            return;
        if (_relatedEnemy.InBattle)
            return;
        if (_relatedEnemy.IsDead)
        {
        }
        else if (_player.GameStarted == false)
            return;

        if (_relatedEnemy.isDead)
            return;

        if (Bounds.IsInBounds(transform) == false)
        {
            _moveDirection = (Bounds.VectorToCenter(transform.position) +
                              new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5))).normalized;
        }
        else
        {
            if (_moveDirection == null)
            {
                MoveInRandomDirection(_relatedEnemy);
            }

            float sqrMagnitudeToPlayer = (_player.Position - transform.position).sqrMagnitude;
            Vector3 escapeVector = -(_player.Position - transform.position).normalized;

            if (sqrMagnitudeToPlayer < ExclusionSqrDistanceToPlayer)
            {
                if (_player.ExistsEnemyThatAlreadyInExclusionZone && _player.EnemyInExclusionZone != _relatedEnemy)
                {
                    _moveDirection = escapeVector;
                }
                else
                {
                    _player.RegisterAsEnemyInExclusionZone(_relatedEnemy);
                }
            }
            else if (_player.EnemyInExclusionZone == this)
            {
                _player.UnregisterEnemyInExclusionZone();
            }

            if (_player.IsDead)
            {
            }
            else if (sqrMagnitudeToPlayer < SqrMagnitudeDistanceToReactOnPlayer)
            {
                bool playerSuperior = _player.PlayerStatsSum >= _relatedEnemy.StatsSum;
                _moveDirection = playerSuperior ? escapeVector : (_player.Position - transform.position).normalized;
            }
        }

        if (GameManager.instance.GamePaused)
            return;
        transform.position += _moveDirection.Value * (deltaTime * _relatedEnemy.Speed);
    }

    private Vector3 GetNearestPointOnBound()
    {
        Vector3 position = transform.position;
        float distanceToZMaxBound = Mathf.Abs(45 - position.z);
        float distanceToZMinBound = Mathf.Abs(-45 - position.z);
        float distanceToXMinBound = Mathf.Abs(-45 - position.x);
        float distanceToXMaxBound = Mathf.Abs(45 - position.x);

        float minimal = Mathf.Min(distanceToZMaxBound, distanceToZMinBound, distanceToXMinBound, distanceToXMaxBound);
        Vector3 result = Vector3.zero;
        if (distanceToZMaxBound == minimal)
        {
            result = new Vector3(position.x, 0, 45);
        }

        if (distanceToZMinBound == minimal)
        {
            result = new Vector3(position.x, 0, -45);
        }

        if (distanceToXMinBound == minimal)
        {
            result = new Vector3(-45, 0, position.z);
        }

        if (distanceToXMaxBound == minimal)
        {
            result = new Vector3(45, 0, position.z);
        }

        return result;
    }

    private Vector3 GetNormalToNearestBound()
    {
        Vector3 position = transform.position;
        float distanceToZMaxBound = Mathf.Abs(45 - position.z);
        float distanceToZMinBound = Mathf.Abs(-45 - position.z);
        float distanceToXMinBound = Mathf.Abs(-45 - position.x);
        float distanceToXMaxBound = Mathf.Abs(45 - position.x);
        var minimal = Mathf.Min(distanceToZMaxBound, distanceToZMinBound, distanceToXMinBound, distanceToXMaxBound);
        if (distanceToZMaxBound == minimal)
        {
            return Vector3.back;
        }

        if (distanceToZMinBound == minimal)
        {
            return Vector3.forward;
        }

        if (distanceToXMinBound == minimal)
        {
            
            return Vector3.right;
        }

        if (distanceToXMaxBound == minimal)
        {
            return Vector3.left;
        }

        throw new Exception("Unreachable");
    }

    public void MoveInRandomDirection(Enemy enemy)
    {
        float radians = Random.Range(0, 360f) * Mathf.Deg2Rad;
        _moveDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
    }
}