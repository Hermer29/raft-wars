using System.Collections;

namespace RaftWars.Infrastructure
{
    public interface ICoroutineRunner
    {
        void StartCoroutine(IEnumerator coroutine);
    }
}