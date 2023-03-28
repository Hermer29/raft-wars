using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    public delegate void AwaitableCallback();

    public static IEnumerator WaitFor(float seconds, AwaitableCallback callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
        yield break;
    }

    public static IEnumerator WaitFor(float seconds, AwaitableCallback callback, ICoroutineSender sender)
    {
        yield return new WaitForSeconds(seconds);
        callback();
        sender.CoroutineDone();
        yield break;
    }
}
