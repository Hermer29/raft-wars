using UnityEngine;

namespace Common
{
    public abstract class FighterRaft : MonoBehaviour
    {
        // TODO: Перенести всё касающееся боя сюда

        public abstract void Die();
        public abstract void StopFight();
    }
}