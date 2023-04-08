using UnityEngine;

namespace Interface
{
    public class TextFacingCamera : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}