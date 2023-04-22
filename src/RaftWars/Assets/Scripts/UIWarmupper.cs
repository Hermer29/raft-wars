using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIWarmupper : MonoBehaviour
    {
        private void Start()
        {
            var parent = transform.parent;
            if (parent == null)
                return;
            var layoutGroup = parent.GetComponent<LayoutGroup>();
            if (layoutGroup == null)
                return;
            Process(layoutGroup);
        }

        private void Process(LayoutGroup layoutGroup)
        {
            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
    }
}