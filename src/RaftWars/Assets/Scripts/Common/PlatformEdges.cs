using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlatformEdges
    {
        private List<(Vector3[] corners, Vector3 normal)> _edges;

        public PlatformEdges(GameObject[] platforms)
        {
            _edges = new List<(Vector3[] corners, Vector3 normal)>();
            foreach (GameObject platform in platforms)
            {
                ProcessPlatform(platform);
            }
        }

        private void ProcessPlatform(GameObject platform)
        {
            Vector3 center = platform.transform.position;
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

        public IEnumerable<(Vector3 position, Quaternion rotation)> GetEdges()
        {
            var result = ExcludeIntersecting(_edges);
            foreach (var edge in result)
            {
                yield return (Vector3.Lerp(edge.corners[0], edge.corners[1], .5f),
                    Quaternion.LookRotation(edge.normal));
            }
        }

        private static IEnumerable<(Vector3[] corners, Vector3 normal)> ExcludeIntersecting(IEnumerable<(Vector3[] corners, Vector3 normal)> source)
        {
            var result = source.Select(x => x);
            foreach (var edge in source)
            {
                result = result.Where(sample => edge.normal == sample.normal || Approximately(edge.corners, sample.corners) == false);
            }

            return result;
        }

        private static bool Approximately(Vector3[] cornersA, Vector3[] cornersB)
        {
            return (AlmostEquals(cornersA[0], cornersB[0]) && AlmostEquals(cornersA[1], cornersB[1])) ||
                   (AlmostEquals(cornersA[0], cornersB[1]) && AlmostEquals(cornersA[1], cornersB[0]));
        }

        private static bool AlmostEquals(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude < 2;
        }
    }
}