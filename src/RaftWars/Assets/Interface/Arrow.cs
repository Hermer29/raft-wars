using InputSystem;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Arrow : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _normalizedOffsetFromCenter = 0.6f;
    [SerializeField, Min(.1f)] private float _speed = .1f;
    [SerializeField] private CanvasGroup _fading;
    [Header("Additional target")]
    [SerializeField] private RectTransform _additionalRectTransform;
    [SerializeField, Range(0, 1)] private float _additionalNormalizedOffsetFromCenter = 0.6f;
    
    private RectTransform _rectTransform;
    private Vector2? _targetDirection;
    private Vector2 _previousDirection;
    private Camera _camera;
    private PlayerService _playerService;
    private GameObject _boss;

    public void Construct(PlayerService playerService, Camera camera, Enemy boss)
    {
        SetBoss(boss);
        _playerService = playerService;
        _camera = camera;
        _rectTransform = GetComponent<RectTransform>();
        MapGenerator.BossCreated += SetBoss;
    }

    private void SetBoss(Enemy enemy)
    {
        _boss = enemy.gameObject;
    }
    
    private void Start()
    {
        _fading.alpha = 0;
        _fading.interactable = false;
        _fading.blocksRaycasts = false;
    }

    public void ShowTowards(Vector3 playerPosition, Vector3 targetPosition)
    {
        _fading.alpha = 1;
        Vector2 twoDimDirection = playerPosition.XZ3DDirectionIntoXY2D(targetPosition) * _normalizedOffsetFromCenter;
        _previousDirection = _targetDirection ?? twoDimDirection;
        _targetDirection = twoDimDirection;
    }

    private void Update()
    {
        if (_boss == null)
        {
            _fading.alpha = 0;
            return;
        }
        ShowTowards(_playerService.Position, _boss.transform.position);
        SlerpTowardsTarget(Time.deltaTime);
    }

    private void SlerpTowardsTarget(float deltaTime)
    {
        var direction = (Vector2)Vector3.Slerp(_previousDirection, _targetDirection.Value, deltaTime * _speed);
        Vector2 screenPoint = DirectionToScreenPoint(direction, _normalizedOffsetFromCenter);
        Vector2 additionalScreenPoint = DirectionToScreenPoint(direction, _additionalNormalizedOffsetFromCenter);
        _rectTransform.position = screenPoint;
        _additionalRectTransform.position = additionalScreenPoint;
        _rectTransform.rotation = CalculateRotationTowards(direction);
    }

    private static Quaternion CalculateRotationTowards(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector3.right, direction);
        return Quaternion.Euler(0, 0, angle);
    }

    public void Hide()
    {
        _fading.alpha = 0;
        _targetDirection = null;
    }

    private Vector2 ToScreenPosition(Vector2 viewport)
    {
        return _camera.ViewportToScreenPoint(viewport);
    }

    private void OnDrawGizmos()
    {
        void DrawSphereInDirection(Vector2 direction)
        {
            Gizmos.color = Color.red;
            Vector2 screen = DirectionToScreenPoint(direction, _normalizedOffsetFromCenter);
            
            Gizmos.DrawWireSphere(_camera.ScreenPointToRay(screen).GetPoint(5), .5f);
        }

        if (_camera == null)
            return;
        
        DrawSphereInDirection(Vector2.up);
        DrawSphereInDirection(Vector2.down);
        DrawSphereInDirection(Vector2.right);
        DrawSphereInDirection(Vector2.left);
    }

    private Vector2 DirectionToScreenPoint(Vector2 direction, float normalizedOffsetFromCenter)
    {
        return ToScreenPosition(direction.NormalizedDirectionToViewport(normalizedOffsetFromCenter));
    }
}

public static class Vector2Extensions
{
    public static Vector2 XZ3DDirectionIntoXY2D(this Vector3 worldA, Vector3 worldB)
    {
        worldA.y = 0;
        worldB.y = 0;
        Vector3 direction = worldB - worldA;
        return new Vector2(direction.x, direction.z);
    }
    
    public static Vector2 NormalizedDirectionToViewport(this Vector2 direction, float scale)
    {
        Vector2 normalized = direction.normalized;
        float x = (normalized.x * scale) * 0.5f + 0.5f;
        float y = (normalized.y * scale) * 0.5f + 0.5f;
        return new Vector2(x, y);
    }
}