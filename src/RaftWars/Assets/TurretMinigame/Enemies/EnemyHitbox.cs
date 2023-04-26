using UnityEngine;

namespace TurretMinigame.Enemies
{
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] private TurretMinigameEnemy _enemy;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
                _enemy.TakeHit(collision.gameObject);
        }
    }
}