using System.Collections.Generic;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 2点を通る直線でプレイ領域(矩形)を2分割するための純粋ロジック。
    /// - 入力・描画・GameObject生成は行わない
    /// - 分割結果として、交点2つ/2領域ポリゴン/重心を返す
    /// </summary>
    public static class BorderLineRegionSplitter
    {
        public readonly struct Region
        {
            public readonly int Id;
            public readonly List<Vector2> Polygon;
            public readonly Vector2 Centroid;

            public Region(int id, List<Vector2> polygon)
            {
                Id = id;
                Polygon = polygon;
                Centroid = ComputePolygonCentroid(polygon);
            }
        }

        public struct PartitionResult
        {
            public Rect Rect;
            public Vector2 Intersection0;
            public Vector2 Intersection1;
            public Vector2 LineP0;
            public Vector2 LineP1;
            public List<Region> Regions;
        }

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

        /// <summary>
        /// カメラのViewport(0..1)を、指定したZ平面(worldZ)上のワールド矩形(Rect)に変換します。
        /// </summary>
        public static bool TryGetCameraWorldRect(Camera cam, float worldZ, out Rect rect)
        {
            rect = default;
            if (cam == null) return false;

            float zDistance = Mathf.Abs(cam.transform.position.z - worldZ);
            Vector3 bl3 = cam.ViewportToWorldPoint(new Vector3(0f, 0f, zDistance));
            Vector3 tr3 = cam.ViewportToWorldPoint(new Vector3(1f, 1f, zDistance));

            float minX = Mathf.Min(bl3.x, tr3.x);
            float maxX = Mathf.Max(bl3.x, tr3.x);
            float minY = Mathf.Min(bl3.y, tr3.y);
            float maxY = Mathf.Max(bl3.y, tr3.y);

            if (!float.IsFinite(minX) || !float.IsFinite(maxX) || !float.IsFinite(minY) || !float.IsFinite(maxY))
            {
                return false;
            }

            rect = Rect.MinMaxRect(minX, minY, maxX, maxY);
            return rect.width > 0f && rect.height > 0f;
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

            if (!TrySplitRectByLine(rect, p0, p1, out PartitionResult partition))
            {
                return false;
            }

            if (partition.Regions == null || partition.Regions.Count != 2) return false;

            var r0 = partition.Regions[0];
            var r1 = partition.Regions[1];

            result = new SplitResult
            {
                Rect = partition.Rect,
                Intersection0 = partition.Intersection0,
                Intersection1 = partition.Intersection1,
                Polygon1 = r0.Polygon,
                Polygon2 = r1.Polygon,
                Centroid1 = r0.Centroid,
                Centroid2 = r1.Centroid
            };
            return true;
        }

        /// <summary>
        /// 矩形領域を、2点を通る直線で分割
        /// 将来の多分割に備え、結果は Regions(List) 
        /// </summary>
        public static bool TrySplitRectByLine(Rect rect, Vector2 p0, Vector2 p1, out PartitionResult result)
        {
            result = default;

            if (!TryGetLineRectIntersections(p0, p1, rect, out var i0, out var i1))
            {
                return false;
            }

            var rectPoly = BuildRectPolygon(rect);
            if (!TrySplitConvexPolygonByLine(rectPoly, p0, p1, out var polyA, out var polyB))
            {
                return false;
            }

            result = new PartitionResult
            {
                Rect = rect,
                Intersection0 = i0,
                Intersection1 = i1,
                LineP0 = p0,
                LineP1 = p1,
                Regions = new List<Region>(2)
                {
                    new Region(1, polyA),
                    new Region(2, polyB)
                }
            };
            return true;
        }

        /// <summary>
        /// 既存の分割結果(Regions)を、さらに1本の直線で分割して多領域化します。
        /// 直線が交差しない領域はそのまま保持します。
        /// </summary>
        public static bool TrySplitPartitionByLine(PartitionResult current, Vector2 p0, Vector2 p1, out PartitionResult next)
        {
            next = default;
            if (current.Regions == null || current.Regions.Count == 0) return false;

            var newRegions = new List<Region>(current.Regions.Count + 1);
            int nextId = 1;
            bool anySplit = false;

            for (int i = 0; i < current.Regions.Count; i++)
            {
                var region = current.Regions[i];
                var poly = region.Polygon;
                if (poly == null || poly.Count < 3) continue;

                if (TrySplitConvexPolygonByLine(poly, p0, p1, out var a, out var b))
                {
                    anySplit = true;
                    newRegions.Add(new Region(nextId++, a));
                    newRegions.Add(new Region(nextId++, b));
                }
                else
                {
                    newRegions.Add(new Region(nextId++, poly));
                }
            }

            if (!anySplit) return false;

            // 描画用に「画面外枠(Rect)との交点」は常に更新（延長線の端点として使う）
            if (!TryGetLineRectIntersections(p0, p1, current.Rect, out var i0, out var i1))
            {
                return false;
            }

            next = new PartitionResult
            {
                Rect = current.Rect,
                Intersection0 = i0,
                Intersection1 = i1,
                LineP0 = p0,
                LineP1 = p1,
                Regions = newRegions
            };
            return true;
        }

        private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        private static List<Vector2> BuildRectPolygon(Rect rect)
        {
            Vector2 tl = new Vector2(rect.xMin, rect.yMax);
            Vector2 tr = new Vector2(rect.xMax, rect.yMax);
            Vector2 br = new Vector2(rect.xMax, rect.yMin);
            Vector2 bl = new Vector2(rect.xMin, rect.yMin);
            return new List<Vector2>(4) { tl, tr, br, bl }; // clockwise
        }

        /// <summary>
        /// 凸ポリゴンを直線で二分割します。
        /// 戻り値trueのとき、両側のポリゴンが有効(各3点以上)です。
        /// </summary>
        private static bool TrySplitConvexPolygonByLine(List<Vector2> poly, Vector2 lineP0, Vector2 lineP1, out List<Vector2> left, out List<Vector2> right)
        {
            left = null;
            right = null;
            if (poly == null || poly.Count < 3) return false;

            Vector2 d = lineP1 - lineP0;
            if (d.sqrMagnitude < 1e-8f) return false;

            var l = new List<Vector2>(poly.Count + 2);
            var r = new List<Vector2>(poly.Count + 2);

            for (int i = 0; i < poly.Count; i++)
            {
                Vector2 a = poly[i];
                Vector2 b = poly[(i + 1) % poly.Count];

                float sa = Side(lineP0, d, a);
                float sb = Side(lineP0, d, b);

                bool aLeft = sa >= -1e-6f;
                bool aRight = sa <= 1e-6f;
                bool bLeft = sb >= -1e-6f;
                bool bRight = sb <= 1e-6f;

                if (aLeft) l.Add(a);
                if (aRight) r.Add(a);

                // 辺が直線を跨ぐなら交点を追加
                if ((sa > 1e-6f && sb < -1e-6f) || (sa < -1e-6f && sb > 1e-6f))
                {
                    if (TrySegmentLineIntersection(a, b, lineP0, d, out var hit))
                    {
                        l.Add(hit);
                        r.Add(hit);
                    }
                }
                else
                {
                    // bが境界上で、重複しやすいので追加は次ループで行う
                }
            }

            RemoveConsecutiveDuplicates(l);
            RemoveConsecutiveDuplicates(r);

            if (l.Count < 3 || r.Count < 3) return false;

            left = l;
            right = r;
            return true;
        }

        private static float Side(Vector2 lineP, Vector2 lineDir, Vector2 p)
        {
            return Cross(lineDir, p - lineP);
        }

        private static bool TrySegmentLineIntersection(Vector2 segA, Vector2 segB, Vector2 lineP, Vector2 lineDir, out Vector2 hit)
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

        private static void RemoveConsecutiveDuplicates(List<Vector2> pts)
        {
            if (pts == null || pts.Count <= 1) return;
            for (int i = pts.Count - 1; i >= 1; i--)
            {
                if ((pts[i] - pts[i - 1]).sqrMagnitude < 1e-8f)
                {
                    pts.RemoveAt(i);
                }
            }
            if (pts.Count >= 2 && (pts[0] - pts[^1]).sqrMagnitude < 1e-8f)
            {
                pts.RemoveAt(pts.Count - 1);
            }
        }

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
