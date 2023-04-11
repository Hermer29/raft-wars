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

    private bool _hidden;

    private void LateUpdate()
    {
        if(Target == null)
        {
            CanvasGroup.alpha = 0;
            return;
        }

        float distance = (Game.PlayerService.PlayerInstance.transform.position - Target.transform.position).sqrMagnitude;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(Target.position);
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        if((center - screenPoint).sqrMagnitude > 1500000)
        {
            CanvasGroup.alpha = 0;
            _hidden = true;
            return;
        }
        CanvasGroup.alpha = 1;
        var clamped = ClampScreenPoint(screenPoint);

        if(_hidden)
        {
            transform.position = clamped;
            _hidden = false;
            return;
        }
        transform.position = Vector3.Lerp(transform.position, clamped, Time.deltaTime * 5);
    }

    private Vector3 ClampScreenPoint(Vector3 screenPoint)
    {
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
        var clamped = new Vector3(x, y);
        return clamped;
    }
}