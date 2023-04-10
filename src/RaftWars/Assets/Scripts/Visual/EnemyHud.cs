using System;
using RaftWars.Infrastructure;
using UnityEngine;
using TMPro;

public class EnemyHud : MonoBehaviour
{
    public Transform Target;
    public TMP_Text hpText;
    public TMP_Text damageText;
    public CanvasGroup CanvasGroup;
    public TMP_Text nickname;

    private void LateUpdate()
    {
        if(Target == null)
        {
            CanvasGroup.alpha = 0;
            return;
        }

        float distance = (Game.PlayerService.PlayerInstance.transform.position - Target.transform.position).sqrMagnitude;
        if (!(distance < 800))
        {
            CanvasGroup.alpha = 0;
            return;
        }
        CanvasGroup.alpha = 1;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(Target.position);
        var viewportBoundsXMin = .2f;
        var viewportBoundsXMax = .8f;
        var viewportBoundsYMin = .2f;
        var viewportBoundsYMax = .8f;
        var screenBoundsXMin = Screen.width * viewportBoundsXMin;
        var screenBoundsXMax = Screen.width * viewportBoundsXMax;
        var screenBoundsYMin = Screen.height * viewportBoundsYMin;
        var screenBoundsYMax = Screen.height * viewportBoundsYMax;
        var x = Mathf.Clamp(screenPoint.x, screenBoundsXMin, screenBoundsXMax);
        var y = Mathf.Clamp(screenPoint.y, screenBoundsYMin, screenBoundsYMax);
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y), Time.deltaTime * 5);
    }
}