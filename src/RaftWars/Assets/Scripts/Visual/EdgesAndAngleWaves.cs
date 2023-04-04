﻿using System;
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
                var rotation = new ParticleSystem.MinMaxCurve
                {
                    constantMax = 0,
                    constantMin = 0,
                    constant = AngleOffAroundAxis(Vector3.forward, direction, Vector3.up) * Mathf.Deg2Rad
                };
                main.startRotation = rotation;
            }
        }

        public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
        {
            Vector3 right;
            if(clockwise)
            {
                right = Vector3.Cross(forward, axis);
                forward = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, forward);
                forward = Vector3.Cross(right, axis);
            }
            return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
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
            
            foreach ((Vector3 position, Vector3 direction) in _angles.GetOuterAngles())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position, .5f);
                Gizmos.DrawLine(position, position + direction);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(position + direction, .2f);
            }
        }
    }
}