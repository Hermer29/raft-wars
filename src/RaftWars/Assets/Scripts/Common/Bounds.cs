using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private Vector2 _maxXZ;
    [SerializeField] private Vector2 _minXZ;

    private static Bounds _instance;

    public static bool IsInBounds(Transform transform)
    {
        if(_instance == null)
        {
            _instance = FindObjectOfType<Bounds>();
        }

        bool notBiggerThanMax = transform.position.x <= _instance._maxXZ.x && transform.position.z <= _instance._maxXZ.y;
        bool notLessThanMin = transform.position.x >= _instance._minXZ.x && transform.position.z >= _instance._minXZ.y;
        return notBiggerThanMax && notLessThanMin;
    }

    public static Vector3 VectorToCenter(Vector3 worldPosition)
    {
        return -worldPosition;
    }

    private void OnDrawGizmos()
    {
        const float height = 1f;
        var a = new Vector3(_maxXZ.x, height, _maxXZ.y);
        var b = new Vector3(_minXZ.x, height, _minXZ.y);
        var c = new Vector3(_maxXZ.x, height, _minXZ.y);
        var d = new Vector3(_minXZ.x, height, _maxXZ.y);

        Gizmos.DrawLine(a, c);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(a, d);
        Gizmos.DrawLine(b, d);
    }
}