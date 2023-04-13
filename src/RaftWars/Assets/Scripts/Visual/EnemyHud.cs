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
    private const float HudFollowingSpeed = 10;
    private const float HudZTargetOffset = 4;
    private const float ScreenDistanceBetweenTextsToHideOne = 200 * 200;
    private static List<EnemyHud> _otherHuds = new List<EnemyHud>();

    private Vector2 _screenPoint;
    private Vector2 _actualScreenPosition;
    private float _distanceToCenter;
    private bool _hidden = true;
    private bool _visible;

    public bool CannotBeReplaced;

    private void Start()
    {
        _otherHuds.Add(this);
    }

    private void LateUpdate()
    {
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

    private bool IsCanBeShown(Vector2 screenPoint)
    {
        if(CannotBeReplaced)
            return true;
        if (VisibilityListener._visibility.ContainsKey(this) == false)
            return false;
        var near =_otherHuds.FirstOrDefault(x => (x._actualScreenPosition - _actualScreenPosition).sqrMagnitude < ScreenDistanceBetweenTextsToHideOne);
        if(near != null && near._distanceToCenter < _distanceToCenter)
        {
            return false;
        }
        return VisibilityListener._visibility[this].Any(x => x.IsVisible());
        // var lessThanHorizontalMax = screenPoint.x < Screen.width + HorizontalScreenDistanceToShow;
        // var moreThanHorizontalMin = screenPoint.x > -HorizontalScreenDistanceToShow;
        // var lessThanVerticalMax = screenPoint.y < Screen.height + VerticalScreenDistanceToShow;
        // var moreThanVerticalMin = screenPoint.y > -VerticalScreenDistanceToShow;
        // return lessThanHorizontalMax && moreThanHorizontalMin && lessThanVerticalMax && moreThanVerticalMin; 
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

    public void BecameInvisible()
    {
        _visible = false;
    }

    public void BecameVisible()
    {
        _visible = true;
    }
}