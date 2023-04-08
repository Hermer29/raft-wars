using System.Linq;
using Cinemachine;
using UnityEngine;

public class FightService 
{
    private readonly CinemachineVirtualCamera _virtualCamera;
    private CinemachineTargetGroup _temporalTargetGroup;
    private readonly CinemachineCameraOffset _cinemachineCameraOffset;

    public FightService(CinemachineVirtualCamera cinemachineVirtualCamera)
    {
        _virtualCamera = cinemachineVirtualCamera;
        _virtualCamera.Priority = -10;
        _cinemachineCameraOffset = cinemachineVirtualCamera.GetComponent<CinemachineCameraOffset>();
        _cinemachineCameraOffset.m_Offset.z = 10;
    }

    public void FightStarted(Player player, Enemy enemy)
    {
        _temporalTargetGroup = new GameObject().AddComponent<CinemachineTargetGroup>();
        _temporalTargetGroup.name = "TemporalTargetGroup";
        _virtualCamera.m_Follow = _temporalTargetGroup.transform;
        _virtualCamera.m_LookAt = _temporalTargetGroup.transform;

        _cinemachineCameraOffset.m_Offset.z = (player.Bounds + enemy.Extents) * -1;
        foreach (GameObject platform in player.GetPlatforms().Concat(enemy.GetPlatforms()))
        {
            _temporalTargetGroup.AddMember(platform.transform, 1, 7f);
        }
        _virtualCamera.Priority = 100;
    }

    public void FightEnded()
    {
        if(_temporalTargetGroup == null)
            return;
        Object.Destroy(_temporalTargetGroup.gameObject);
        _virtualCamera.Priority = -10;
    }
}