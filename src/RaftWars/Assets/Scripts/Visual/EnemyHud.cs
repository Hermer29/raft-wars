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

    private const float NormalizedDistanceToShow = .2f;
    private static float HorizontalScreenDistanceToShow = Screen.width * NormalizedDistanceToShow;
    private static float VerticalScreenDistanceToShow = Screen.height * NormalizedDistanceToShow;
    private const float HudFollowingSpeed = 6;
    private const float HudZTargetOffset = 2;

    private bool _hidden = true;

    private void LateUpdate()
    {
        if(Player.instance.canPlay == false)
        {
            Hide();
            return;
        }
        if(Target == null)
        {
            Hide();
            return;
        }
        var worldTarget = Target.position;
        worldTarget.z += HudZTargetOffset; 
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldTarget);
        if(IsCanBeShown(screenPoint) == false)
        {
            Hide();
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
        transform.position = Vector3.Lerp(transform.position, clamped, Time.deltaTime * HudFollowingSpeed);
    }

    private void Hide()
    {
        CanvasGroup.alpha = 0;
        _hidden = true;
    }

    private bool IsCanBeShown(Vector2 screenPoint)
    {
        var lessThanHorizontalMax = screenPoint.x < Screen.width + HorizontalScreenDistanceToShow;
        var moreThanHorizontalMin = screenPoint.x > -HorizontalScreenDistanceToShow;
        var lessThanVerticalMax = screenPoint.y < Screen.height + VerticalScreenDistanceToShow;
        var moreThanVerticalMin = screenPoint.y > -VerticalScreenDistanceToShow;
        return lessThanHorizontalMax && moreThanHorizontalMin && lessThanVerticalMax && moreThanVerticalMin; 
    }

    private Vector3 ClampScreenPoint(Vector3 screenPoint)
    {
        var viewportBoundsXMin = .2f;
        var viewportBoundsXMax = .8f;
        var viewportBoundsYMin = .1f;
        var viewportBoundsYMax = .9f;
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