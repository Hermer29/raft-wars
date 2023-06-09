using System;
using System.Linq;
using UnityEngine;
using TMPro;
using DefaultNamespace;
using System.Collections.Generic;

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
    private const float HudFollowingSpeed = 15;
    private const float HudZTargetOffset = 5;
    private const float ScreenDistanceBetweenTextsToHideOne = 200 * 200;
    private static List<EnemyHud> _otherHuds = new List<EnemyHud>();

    private Vector2 _screenPoint;
    private Vector2 _actualScreenPosition;
    private float _distanceToCenter;
    private bool _hidden = true;
    private bool _visible;

    public bool CannotBeReplaced;
    public bool WorksInFixedUpdate;
    public bool PrioritizedShow;
    public bool NotParticipateInPrioritization;

    private void Start() => _otherHuds.Add(this);

    private void FixedUpdate()
    {
        if(WorksInFixedUpdate == false)
        {
            return;
        }
        if(Player.instance == null || Player.instance.canPlay == false)
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
        _screenPoint = screenPoint;
        _distanceToCenter = (screenPoint - Vector2.zero).sqrMagnitude;
        var clamped = ClampScreenPoint(screenPoint);
        _actualScreenPosition = clamped;
        if(IsCanBeShown(screenPoint) == false)
        {
            Hide();
            return;
        }
        CanvasGroup.alpha = 1;

        if(_hidden)
        {
            transform.position = clamped;
            _hidden = false;
            return;
        }
        transform.position = Vector3.Lerp(transform.position, clamped, Time.deltaTime * HudFollowingSpeed);
    }

    private void LateUpdate()
    {
        if(WorksInFixedUpdate)
        {
            return;
        }
        if(Player.instance == null || Player.instance.canPlay == false)
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
        _screenPoint = screenPoint;
        _distanceToCenter = (screenPoint - Vector2.zero).sqrMagnitude;
        var clamped = ClampScreenPoint(screenPoint);
        _actualScreenPosition = clamped;
        if(IsCanBeShown(screenPoint) == false)
        {
            Hide();
            return;
        }
        CanvasGroup.alpha = 1;

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

    public bool IsCanBeShown()
    {
        var worldTarget = Target.position;
        worldTarget.z += HudZTargetOffset; 
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldTarget);
        _screenPoint = screenPoint;
        _distanceToCenter = (screenPoint - Vector2.zero).sqrMagnitude;
        var clamped = ClampScreenPoint(screenPoint);
        _actualScreenPosition = clamped;
        return IsCanBeShown(screenPoint);
    }

    private bool IsCanBeShown(Vector2 screenPoint)
    {
        if(CannotBeReplaced)
            return true;
        if (VisibilityListener._visibility.ContainsKey(this) == false)
            return false;
        var near =_otherHuds
            .Where(x => VisibilityListener._visibility.ContainsKey(x))
            .Where(x => VisibilityListener._visibility[x].Any(x => x.IsVisible()))
            .FirstOrDefault(x => (x._actualScreenPosition - _actualScreenPosition).sqrMagnitude < ScreenDistanceBetweenTextsToHideOne);
        
        if(NotParticipateInPrioritization)
            goto Exit;

        if(near != null)
        {
            if(near.NotParticipateInPrioritization)
                goto Exit;
            if(PrioritizedShow)
            {
                return true;
            }
            else if(near.PrioritizedShow)
            {
                return false;
            }
            if(near._distanceToCenter < _distanceToCenter)
            {
                return false;
            }
        }
        
        Exit:
        return VisibilityListener._visibility[this].Any(x => x.IsVisible());
        // var lessThanHorizontalMax = screenPoint.x < Screen.width + HorizontalScreenDistanceToShow;
        // var moreThanHorizontalMin = screenPoint.x > -HorizontalScreenDistanceToShow;
        // var lessThanVerticalMax = screenPoint.y < Screen.height + VerticalScreenDistanceToShow;
        // var moreThanVerticalMin = screenPoint.y > -VerticalScreenDistanceToShow;
        // return lessThanHorizontalMax && moreThanHorizontalMin && lessThanVerticalMax && moreThanVerticalMin; 
    }

    private Vector3 ClampScreenPoint(Vector3 screenPoint)
    {
        var aspectRatioWidthToHeight = Screen.width / Screen.height;
        var aspectRatioHeightToWidth = Screen.height / Screen.width;
        var viewportBoundsXMin = .25f;
        var viewportBoundsXMax = .8f;
        var viewportBoundsYMin = .1f;
        var viewportBoundsYMax = .85f;
        var screenBoundsXMin = 100;
        var screenBoundsXMax = Screen.width - 100;
        var screenBoundsYMin = 100;
        var screenBoundsYMax = Screen.height - 100;
        var x = Mathf.Clamp(screenPoint.x, screenBoundsXMin, screenBoundsXMax);
        var y = Mathf.Clamp(screenPoint.y, screenBoundsYMin, screenBoundsYMax);
        var clamped = new Vector3(x, y);
        return clamped;
    }

    public void BecameInvisible()
    {
        _visible = false;
    }

    public void BecameVisible()
    {
        _visible = true;
    }
}