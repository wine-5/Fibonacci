using System.Collections.Generic;
using UnityEngine;

namespace BorderLine
{
    /// <summary>
    /// 2点を通る直線でプレイ領域(矩形)を2分割するための純粋ロジック。
    /// - 入力・描画・GameObject生成は行わない
    /// - 分割結果として、交点2つ/2領域ポリゴン/重心を返す
    /// </summary>
    public static class BorderLineRegionSplitter
    {
        public struct SplitResult
        {
            public Rect Rect;
            public Vector2 Intersection0;
            public Vector2 Intersection1;
            public List<Vector2> Polygon1;
            public List<Vector2> Polygon2;
            public Vector2 Centroid1;
            public Vector2 Centroid2;
        }

        public static bool TryGetPlayAreaRectFromTargets(string targetTag, out Rect rect)
        {
            rect = default;

            GameObject[] targets;
            try
            {
                targets = GameObject.FindGameObjectsWithTag(targetTag);
            }
            catch (UnityException)
            {
                return false;
            }

            if (targets == null || targets.Length == 0) return false;

            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;

            foreach (var go in targets)
            {
                if (go == null) continue;

                var col = go.GetComponent<Collider2D>();
                if (col != null)
                {
                    var b = col.bounds;
                    minX = Mathf.Min(minX, b.min.x);
                    maxX = Mathf.Max(maxX, b.max.x);
                    minY = Mathf.Min(minY, b.min.y);
                    maxY = Mathf.Max(maxY, b.max.y);
                }
                else
                {
                    var p = (Vector2)go.transform.position;
                    minX = Mathf.Min(minX, p.x);
                    maxX = Mathf.Max(maxX, p.x);
                    minY = Mathf.Min(minY, p.y);
                    maxY = Mathf.Max(maxY, p.y);
                }
            }

            if (!float.IsFinite(minX) || !float.IsFinite(maxX) || !float.IsFinite(minY) || !float.IsFinite(maxY))
            {
                return false;
            }

            rect = Rect.MinMaxRect(minX, minY, maxX, maxY);
            return rect.width > 0f && rect.height > 0f;
        }

        public static bool TrySplitRectByLine(Rect rect, Vector2 p0, Vector2 p1, out SplitResult result)
        {
            result = default;

            if (!TryGetLineRectIntersections(p0, p1, rect, out var i0, out var i1))
            {
                return false;
            }

            var perim = BuildRectPerimeterWithIntersections(rect, i0, i1);
            int idx0 = FindPointIndex(perim, i0);
            int idx1 = FindPointIndex(perim, i1);
            if (idx0 < 0 || idx1 < 0) return false;

            var poly1 = BuildPath(perim, idx0, idx1, step: +1);
            var poly2 = BuildPath(perim, idx0, idx1, step: -1);

            result = new SplitResult
            {
                Rect = rect,
                Intersection0 = i0,
                Intersection1 = i1,
                Polygon1 = poly1,
                Polygon2 = poly2,
                Centroid1 = ComputePolygonCentroid(poly1),
                Centroid2 = ComputePolygonCentroid(poly2)
            };
            return true;
        }

        private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        private static bool TryLineSegmentIntersection(Vector2 lineP, Vector2 lineDir, Vector2 segA, Vector2 segB, out Vector2 hit)
        {
            hit = default;
            Vector2 e = segB - segA;
            float den = Cross(lineDir, e);
            if (Mathf.Abs(den) < 1e-6f) return false;

            Vector2 r = segA - lineP;
            float t = Cross(r, e) / den;
            float u = Cross(r, lineDir) / den;
            if (u < -1e-5f || u > 1f + 1e-5f) return false;

            hit = lineP + t * lineDir;
            return true;
        }

        private static bool TryGetLineRectIntersections(Vector2 p0, Vector2 p1, Rect rect, out Vector2 i0, out Vector2 i1)
        {
            i0 = default;
            i1 = default;

            Vector2 d = p1 - p0;
            if (d.sqrMagnitude < 1e-8f) return false;

            Vector2 tl = new Vector2(rect.xMin, rect.yMax);
            Vector2 tr = new Vector2(rect.xMax, rect.yMax);
            Vector2 br = new Vector2(rect.xMax, rect.yMin);
            Vector2 bl = new Vector2(rect.xMin, rect.yMin);

            var hits = new List<Vector2>(4);
            TryAddHit(tl, tr);
            TryAddHit(tr, br);
            TryAddHit(br, bl);
            TryAddHit(bl, tl);

            if (hits.Count < 2) return false;

            i0 = hits[0];
            i1 = hits[1];
            return true;

            void TryAddHit(Vector2 a, Vector2 b)
            {
                if (!TryLineSegmentIntersection(p0, d, a, b, out var hit)) return;
                for (int k = 0; k < hits.Count; k++)
                {
                    if ((hits[k] - hit).sqrMagnitude < 1e-6f) return;
                }
                hits.Add(hit);
            }
        }

        private static List<Vector2> BuildRectPerimeterWithIntersections(Rect rect, Vector2 i0, Vector2 i1)
        {
            Vector2 tl = new Vector2(rect.xMin, rect.yMax);
            Vector2 tr = new Vector2(rect.xMax, rect.yMax);
            Vector2 br = new Vector2(rect.xMax, rect.yMin);
            Vector2 bl = new Vector2(rect.xMin, rect.yMin);

            var corners = new[] { tl, tr, br, bl }; // clockwise
            var pointsOnEdge = new List<Vector2>[4] { new(), new(), new(), new() };

            TryAssign(i0);
            TryAssign(i1);

            var perim = new List<Vector2>(6);
            for (int edge = 0; edge < 4; edge++)
            {
                Vector2 a = corners[edge];
                Vector2 b = corners[(edge + 1) % 4];
                perim.Add(a);

                if (pointsOnEdge[edge].Count > 0)
                {
                    pointsOnEdge[edge].Sort((p, q) => EdgeParam(a, b, p).CompareTo(EdgeParam(a, b, q)));
                    for (int k = 0; k < pointsOnEdge[edge].Count; k++)
                    {
                        var p = pointsOnEdge[edge][k];
                        if ((p - a).sqrMagnitude < 1e-6f) continue;
                        if ((p - b).sqrMagnitude < 1e-6f) continue;
                        perim.Add(p);
                    }
                }
            }

            return perim;

            void TryAssign(Vector2 p)
            {
                for (int c = 0; c < 4; c++)
                {
                    if ((corners[c] - p).sqrMagnitude < 1e-6f) return;
                }

                for (int edge = 0; edge < 4; edge++)
                {
                    Vector2 a = corners[edge];
                    Vector2 b = corners[(edge + 1) % 4];
                    if (IsPointOnSegment(a, b, p))
                    {
                        pointsOnEdge[edge].Add(p);
                        return;
                    }
                }
            }
        }

        private static bool IsPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            Vector2 ap = p - a;
            float cross = Cross(ab, ap);
            if (Mathf.Abs(cross) > 1e-3f) return false;
            float dot = Vector2.Dot(ap, ab);
            if (dot < -1e-3f) return false;
            if (dot > ab.sqrMagnitude + 1e-3f) return false;
            return true;
        }

        private static float EdgeParam(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            float len2 = ab.sqrMagnitude;
            if (len2 < 1e-8f) return 0f;
            return Vector2.Dot(p - a, ab) / len2;
        }

        private static int FindPointIndex(List<Vector2> points, Vector2 p)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if ((points[i] - p).sqrMagnitude < 1e-6f) return i;
            }
            return -1;
        }

        private static List<Vector2> BuildPath(IReadOnlyList<Vector2> ordered, int startIndex, int endIndex, int step)
        {
            int n = ordered.Count;
            var result = new List<Vector2>(n + 2);

            int i = startIndex;
            for (int guard = 0; guard < n + 1; guard++)
            {
                result.Add(ordered[i]);
                if (i == endIndex) break;
                i = (i + step + n) % n;
            }

            return result;
        }

        private static Vector2 ComputePolygonCentroid(List<Vector2> poly)
        {
            if (poly == null || poly.Count < 3)
            {
                return poly == null || poly.Count == 0 ? Vector2.zero : Average(poly);
            }

            float twiceArea = 0f;
            float cx = 0f;
            float cy = 0f;

            for (int i = 0; i < poly.Count; i++)
            {
                Vector2 p0 = poly[i];
                Vector2 p1 = poly[(i + 1) % poly.Count];
                float cross = p0.x * p1.y - p1.x * p0.y;
                twiceArea += cross;
                cx += (p0.x + p1.x) * cross;
                cy += (p0.y + p1.y) * cross;
            }

            if (Mathf.Abs(twiceArea) < 1e-5f)
            {
                return Average(poly);
            }

            float inv = 1f / (3f * twiceArea);
            return new Vector2(cx * inv, cy * inv);
        }

        private static Vector2 Average(List<Vector2> poly)
        {
            float x = 0f;
            float y = 0f;
            for (int i = 0; i < poly.Count; i++)
            {
                x += poly[i].x;
                y += poly[i].y;
            }
            return new Vector2(x / poly.Count, y / poly.Count);
        }
    }
}
