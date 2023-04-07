using System;
using System.Linq;
using Common;
using DefaultNamespace;
using RaftWars.Infrastructure;
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
        private const string EdgesParentName = "Edges";
        private const string WavesParentName = "Waves";

        public void Construct(IPlatformsCarrier platformsCarrier, Material selectedColor = null)
        {
            _platformsCarrier = platformsCarrier;
            _color = selectedColor;
            _edges = new PlatformEdges(_platformsCarrier.GetPlatforms().Select(x => x.gameObject).ToArray());
            _angles = new PlatformAngles(_edges);
        }
        
        public bool EdgesDisabled { get; private set; }

        public void CreateEdges()
        {
            if (EdgesDisabled)
                return;
            _edges = new PlatformEdges(_platformsCarrier.GetPlatforms().ToArray());
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
            if (EdgesDisabled == false)
            {
                DestroyChildrenOfObjectWithName(EdgesParentName);
                CreateEdges();
            }
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
            GameObject edge = GameFactory.CreatePlatformEdge();
            edge.transform.parent = GetOrCreateParentWithName(EdgesParentName);
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
            GameObject waves = GameFactory.CreateCornerWaves().gameObject;
            waves.transform.parent = GetOrCreateParentWithName(WavesParentName);
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
                var rotation = new ParticleSystem.MinMaxCurve
                {
                    constantMax = 0,
                    constantMin = 0,
                    constant = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up) * Mathf.Deg2Rad
                        
                };
                main.startRotation = rotation;
            }
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
            
            foreach ((Vector3 position, Vector3 direction) in _angles.GetOuterAngles())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position, .5f);
                Gizmos.DrawLine(position, position + direction);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(position + direction, .2f);
            }
        }

        public void ChangeMaterial(Material material)
        {
            _color = material;
            RepaintEdges(material);
        }

        private void RepaintEdges(Material material)
        {
            var edgesParent = GetOrCreateParentWithName(EdgesParentName);
            foreach (MeshRenderer edge in edgesParent.Cast<Transform>().Select(x => x.GetComponent<MeshRenderer>()))
            {
                edge.material = material;
            }
        }

        public void DisableEdges()
        {
            if (EdgesDisabled)
                throw new InvalidOperationException("Already disabled");
            Transform edgesParent = GetOrCreateParentWithName(EdgesParentName);
            EdgesDisabled = true;
            foreach (Transform edge in edgesParent.Cast<Transform>())
            {
                Destroy(edge.gameObject);
            }
        }
        
        public void EnableEdges()
        {
            if (EdgesDisabled == false)
                throw new InvalidOperationException("Already enabled");
            EdgesDisabled = false;
            CreateEdges();
        }
    }
}