using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlatformEdges
    {
        private readonly List<(Vector3[] corners, Vector3 normal)> _edges;

        public PlatformEdges(IEnumerable<GameObject> platforms)
        {
            _edges = new List<(Vector3[] corners, Vector3 normal)>();
            foreach (GameObject platform in platforms)
            {
                ProcessPlatform(platform);
            }
        }

        private void ProcessPlatform(GameObject platform)
        {
            Vector3 center = platform.transform.localPosition;
            Vector3 minXminZ = center + new Vector3(-Constants.PlatformSize / 2, 0, -Constants.PlatformSize / 2);
            Vector3 minXmaxZ = center + new Vector3(-Constants.PlatformSize / 2, 0, Constants.PlatformSize / 2);
            Vector3 maxXminZ = center + new Vector3(Constants.PlatformSize / 2, 0, -Constants.PlatformSize / 2);
            Vector3 maxXmaxZ = center + new Vector3(Constants.PlatformSize / 2, 0, Constants.PlatformSize / 2);
            _edges.Add((new []{minXminZ, minXmaxZ}, Vector3.left));
            _edges.Add((new []{minXmaxZ, maxXmaxZ}, Vector3.up));
            _edges.Add((new []{maxXmaxZ, maxXminZ}, Vector3.right));
            _edges.Add((new []{maxXminZ, minXminZ}, Vector3.down));
        }

        public void Add(GameObject platform)
        {
            ProcessPlatform(platform);
        }

        public IEnumerable<(Vector3 position, Quaternion rotation)> GetEdgeMiddlePoints()
        {
            var result = ExcludeIntersecting(_edges);
            foreach (var edge in result)
            {
                yield return (Vector3.Lerp(edge.corners[0], edge.corners[1], .5f),
                    Quaternion.LookRotation(edge.normal));
            }
        }

        public IEnumerable<(Vector3 a, Vector3 b)> GetEdges()
        {
            var result = ExcludeIntersecting(_edges);
            foreach (var edge in result)
            {
                yield return (edge.corners[0], edge.corners[1]);
            }
        }

        public IEnumerable<Vector3> GetPointsOnEdges()
        {
            return GetEdges()
                .SelectMany(x => new[] { x.a, x.b })
                .GroupBy(x => x)
                .Select(x => x.Key);
        }

        private static IEnumerable<(Vector3[] corners, Vector3 normal)> ExcludeIntersecting(IEnumerable<(Vector3[] corners, Vector3 normal)> source)
        {
            var result = source.Select(x => x);

            return source.Aggregate(result, 
                (current, edge) => 
                    current.Where(sample => edge.normal == sample.normal || 
                                            Approximately(edge.corners, sample.corners) == false));
        }

        private static bool Approximately(IReadOnlyList<Vector3> cornersA, IReadOnlyList<Vector3> cornersB)
        {
            return (GeoMaths.AlmostEquals(cornersA[0], cornersB[0]) && 
                    GeoMaths.AlmostEquals(cornersA[1], cornersB[1])) ||
                   (GeoMaths.AlmostEquals(cornersA[0], cornersB[1]) && 
                    GeoMaths.AlmostEquals(cornersA[1], cornersB[0]));
        }
    }
}