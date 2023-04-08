
using UnityEngine;

namespace Visual
{
    public class HUDsScaler : MonoBehaviour
    {
        private CinemachineCameraOffset _playerCamera;

        public void Construct(CinemachineCameraOffset playerCamera)
        {
            _playerCamera = playerCamera; 
        }

        private void Update()
        {
            foreach(var obj in GameObject.FindGameObjectsWithTag("WorldSpaceHud"))
            {
                var prev = obj.transform.position;
                prev.y = -_playerCamera.m_Offset.z;
                obj.transform.position = prev;
            }
        }
    }
}