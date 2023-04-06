using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIWarmupper : MonoBehaviour
    {
        private void Start()
        {
            Process();
        }

        private void Process()
        {
            Canvas.ForceUpdateCanvases();
            Transform parent = transform.parent;
            parent.GetComponent<LayoutGroup>().enabled = false;
            parent.GetComponent<LayoutGroup>().enabled = true;
        }
    }
}