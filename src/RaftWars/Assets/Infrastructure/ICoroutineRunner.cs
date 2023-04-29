using System.Collections;
using UnityEngine;

namespace RaftWars.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}