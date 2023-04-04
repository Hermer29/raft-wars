using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using static Common.GeoMaths;

namespace Common
{
    public class PlatformAngles
    {
        private readonly PlatformEdges _edges;

        public PlatformAngles(PlatformEdges edges)
        {
            _edges = edges;
        }

        public IEnumerable<(Vector3 position, Vector3 direction)> GetOuterAngles()
        {
            var edges = _edges.GetEdges();
            var angles = ToAngles(edges).GroupBy(x => x.b).Select(x => x.First());
            foreach ((Vector3 a, Vector3 b, Vector3 c) angle in angles)
            {
                if (IsAngleDeveloped(angle))
                {
                    continue;
                }

                yield return (angle.b, GetAngleDirection(angle));
            }
        }

        private static Vector3 GetAngleDirection((Vector3 a, Vector3 b, Vector3 c) angle)
        {
            (Vector3 a, Vector3 b, Vector3 c) = angle;
            a.y = 0;
            b.y = 0;
            c.y = 0;
            var direction = (b - a + (b - c));
            return direction.normalized;
        }

        private static bool IsAngleDeveloped((Vector3 a, Vector3 b, Vector3 c) angle)
        {
            const float tolerance = .1f;
            (Vector3 a, Vector3 b, Vector3 c) = angle;
            return (Math.Abs(a.x - b.x) < tolerance && Math.Abs(a.x - c.x) < tolerance) || 
                   (Math.Abs(a.z - b.z) < tolerance && Math.Abs(a.z - c.z) < tolerance);
        }

        private static IEnumerable<(Vector3 a, Vector3 b, Vector3 c)> ToAngles(IEnumerable<(Vector3 a, Vector3 b)> edges)
        {
            var points = edges.SelectMany(x => new Vector3[] { x.a, x.b });
            points = points.GroupBy(x => x).Select(x => x.First());
            foreach (Vector3 point in points)
            {
                var containingPoint = EdgesContainingPoint(edges, point)
                    .SelectMany(x => new[] {x.a, x.b})
                    .GroupBy(x => x)
                    .Select(x => x.First())
                    .Except(new []{point})
                    .ToArray();

                yield return (containingPoint[0], point, containingPoint[1]);
            }
        }

        private static IEnumerable<(Vector3 a, Vector3 b)> EdgesContainingPoint(IEnumerable<(Vector3 a, Vector3 b)> source, Vector3 point)
        {
            return source.Where(edge => AlmostEquals(point, edge.a) || AlmostEquals(point, edge.b));
        }
    }
}