using System;
using System.Linq;
using Common;
using DefaultNamespace;
using UnityEngine;

namespace Visual
{
    public class EdgesAndAngleWaves : MonoBehaviour
    {
        private PlatformEdges _edges;
        private PlatformAngles _angles;
        private IPlatformsCarrier _platformsCarrier;
        private Material _color;

        private const float HeightOffset = .6f;
        private const string CornerWavesResourcesPath = "CornerWaves";
        private const string EdgeResourcesPath = "Edge";
        private const string EdgesParentName = "Edges";
        private const string WavesParentName = "Waves";

        public void Construct(IPlatformsCarrier platformsCarrier, Material selectedColor)
        {
            _platformsCarrier = platformsCarrier;
            _color = selectedColor;
        }

        public void CreateEdges()
        {
            _edges = new PlatformEdges(_platformsCarrier.GetPlatforms().Select(x => x.gameObject).ToArray());
            foreach ((Vector3 position, Quaternion rotation) in _edges.GetEdgeMiddlePoints())
            {
                GameObject edge = CreateEdge();
                edge.transform.position = position + Vector3.up * HeightOffset;
                edge.transform.rotation = rotation;
            }

            _angles = new PlatformAngles(_edges);
        }

        public void UpdateVisual(GameObject newPlatform)
        {
            _edges.Add(newPlatform);
            DestroyChildrenOfObjectWithName(EdgesParentName);
            CreateEdges();
            DestroyChildrenOfObjectWithName(WavesParentName);
            CreateWaves();
        }

        private void DestroyChildrenOfObjectWithName(string parentName)
        {
            var edgesParent = transform.Cast<Transform>()
                .First(x => x.name == parentName);
            foreach (Transform childEdge in edgesParent.transform)
            {
                Destroy(childEdge.gameObject);
            }
        }

        private GameObject CreateEdge()
        {
            GameObject prefab = GetPrefab(EdgeResourcesPath);
            Transform parent = GetOrCreateParentWithName(EdgesParentName);

            GameObject edge = Instantiate(prefab, parent);
            edge.GetComponent<MeshRenderer>().material = _color;
            return edge;
        }

        private Transform GetOrCreateParentWithName(string parentName)
        {
            Transform parent = transform.Cast<Transform>().FirstOrDefault(x => x.name == parentName);
            if (parent != null) return parent;
            parent = new GameObject().transform;
            parent.name = parentName;
            parent.SetParent(transform);
            return parent;
        }

        private GameObject CreateWave()
        {
            GameObject prefab = GetPrefab(CornerWavesResourcesPath);
            Transform parent = GetOrCreateParentWithName(WavesParentName);
            
            GameObject waves = Instantiate(prefab, parent);
            return waves;
        }

        public void CreateWaves()
        {
            var angles = _angles.GetOuterAngles();
            foreach ((Vector3 position, Vector3 direction) in angles)
            {
                GameObject wave = CreateWave();
                wave.transform.position = position;
                var particleSystem = wave.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = particleSystem.main;
                wave.name = GetRotationAngle(direction).ToString();
                var rotation = new ParticleSystem.MinMaxCurve
                {
                    constantMax = 0,
                    constantMin = 0,
                    constant = GetRotationAngle(direction)
                };
                main.startRotation = rotation;
            }
        }

        private static float GetRotationAngle(Vector3 direction)
        {
            return Mathf.Asin(direction.x) * Mathf.Acos(direction.z);
        }

        private static GameObject GetPrefab(string resourcesPath)
        {
            return Resources.Load<GameObject>(resourcesPath);
        }

        private void OnDrawGizmos()
        {
            if (_edges == null)
                return;
            
            Gizmos.color = Color.green;
            foreach ((Vector3 position, Quaternion rotation) edge in _edges.GetEdgeMiddlePoints())
            {
                Gizmos.DrawWireSphere(edge.position, .5f);
                Gizmos.DrawLine(edge.position, edge.position + edge.rotation * Vector3.left);
            }

            if (_angles == null)
                return;
            
            Gizmos.color = Color.red;
            foreach ((Vector3 position, Vector3 direction) in _angles.GetOuterAngles())
            {
                Gizmos.DrawWireSphere(position, .5f);
                Gizmos.DrawLine(position, position + direction);
            }
        }
    }
}