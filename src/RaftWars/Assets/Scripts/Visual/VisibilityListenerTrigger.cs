using System;
using UnityEngine;

namespace Visual
{
    public class VisibilityListenerTrigger : MonoBehaviour
    {
        private void OnBecameInvisible()
        {
            Debug.Log("Visible");
        }

        private void OnBecameVisible()
        {
            Debug.Log("Invisible");
        }
    }
}