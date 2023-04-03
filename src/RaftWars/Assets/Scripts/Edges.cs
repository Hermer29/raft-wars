using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class Edges : MonoBehaviour
    {
        private PlatformEdges _edges;
        private IPlatformsCarrier _platformsCarrier;
        private Material _color;

        private const float HeightOffset = .6f;

        public void Construct(IPlatformsCarrier platformsCarrier, Material selectedColor)
        {
            _platformsCarrier = platformsCarrier;
            _color = selectedColor;
        }

        public void WarmupEdges()
        {
            _edges = new PlatformEdges(_platformsCarrier.GetPlatforms().Select(x => x.gameObject).ToArray());
            foreach ((Vector3 position, Quaternion rotation) in _edges.GetEdges())
            {
                GameObject edge = CreateEdge();
                edge.transform.position = position + Vector3.up * HeightOffset;
                edge.transform.rotation = rotation;
            }
        }

        public void UpdateEdges(GameObject newPlatform)
        {
            _edges.Add(newPlatform);
            var edgesParent = transform.Cast<Transform>().First(x => x.name == "Edges");
            foreach (Transform childEdge in edgesParent.transform)
            {
                Destroy(childEdge.gameObject);
            }
            WarmupEdges();
        }

        private GameObject CreateEdge()
        {
            var prefab = Resources.Load<GameObject>("Edge");
            var parent = transform.Cast<Transform>().FirstOrDefault(x => x.name == "Edges");
            if (parent == null)
            {
                parent = new GameObject().transform;
                parent.name = "Edges";
                parent.SetParent(transform);
            }

            GameObject edge = Instantiate(prefab, parent);
            edge.GetComponent<MeshRenderer>().material = _color;
            return edge;
        }

        private void OnDrawGizmos()
        {
            if (_edges == null)
                return;

            Gizmos.color = Color.green;
            foreach ((Vector3 position, Quaternion rotation) edge in _edges.GetEdges())
            {
                Gizmos.DrawWireSphere(edge.position, .5f);
                Gizmos.DrawLine(edge.position, edge.position + edge.rotation * Vector3.left);
            }
        }
    }
}