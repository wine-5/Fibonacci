using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 領域分割のデバッグ可視化専用。
    /// - TextMeshの"1/2"マーカー表示
    /// - Debug.DrawLineによる領域アウトライン表示
    /// ゲーム本体の判定ロジックは持たない。
    /// </summary>
    public sealed class BorderLineRegionDebugView
    {
        private readonly Transform parent;
        private readonly float worldZ;

        private readonly float markerTextSize;
        private readonly float debugDrawDuration;

        private readonly System.Collections.Generic.List<GameObject> regionMarkers = new();

        public BorderLineRegionDebugView(Transform parent, float worldZ, float markerTextSize, float debugDrawDuration)
        {
            this.parent = parent;
            this.worldZ = worldZ;
            this.markerTextSize = markerTextSize;
            this.debugDrawDuration = debugDrawDuration;
        }

        private void EnsureMarkerCount(int count)
        {
            while (regionMarkers.Count < count)
            {
                int idx = regionMarkers.Count + 1;
                var go = CreateTextMarker($"RegionMarker{idx}", idx.ToString(), GetRegionColor(idx));
                regionMarkers.Add(go);
            }
        }

        private void SetMarkerPositions(System.Collections.Generic.IReadOnlyList<BorderLineRegionSplitter.Region> regions)
        {
            for (int i = 0; i < regions.Count; i++)
            {
                var go = regionMarkers[i];
                if (go == null) continue;
                Vector2 c = regions[i].Centroid;
                go.transform.position = new Vector3(c.x, c.y, worldZ);
            }
        }

        private void DrawPolygons(BorderLineRegionSplitter.PartitionResult partition)
        {
            if (partition.Regions != null)
            {
                for (int i = 0; i < partition.Regions.Count; i++)
                {
                    var poly = partition.Regions[i].Polygon;
                    DrawPolygonDebug(poly, GetRegionColor(i + 1));
                }
            }

            Debug.DrawLine(partition.Intersection0, partition.Intersection1, Color.white, debugDrawDuration);
        }

        public void Render(BorderLineRegionSplitter.PartitionResult partition, bool showMarkers, bool drawOutlines)
        {
            if (showMarkers)
            {
                int count = partition.Regions != null ? partition.Regions.Count : 0;
                EnsureMarkerCount(count);

                for (int i = 0; i < regionMarkers.Count; i++)
                {
                    if (regionMarkers[i] != null) regionMarkers[i].SetActive(i < count);
                }

                if (partition.Regions != null && partition.Regions.Count > 0)
                {
                    SetMarkerPositions(partition.Regions);
                }
            }
            else
            {
                for (int i = 0; i < regionMarkers.Count; i++)
                {
                    if (regionMarkers[i] != null) regionMarkers[i].SetActive(false);
                }
            }

            if (drawOutlines)
            {
                DrawPolygons(partition);
            }
        }

        private GameObject CreateTextMarker(string name, string text, Color color)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, worldPositionStays: true);

            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.color = color;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.characterSize = markerTextSize;
            tm.fontSize = 64;

            return go;
        }

        private static Color GetRegionColor(int index)
        {
            // 1..N を想定。HSVで見分けやすい色を生成。
            float hue = Mathf.Repeat((index - 1) * 0.18f, 1f);
            return Color.HSVToRGB(hue, 0.85f, 1f);
        }

        private void DrawPolygonDebug(System.Collections.Generic.List<Vector2> poly, Color color)
        {
            if (poly == null || poly.Count < 2) return;
            for (int i = 0; i < poly.Count; i++)
            {
                Vector2 a = poly[i];
                Vector2 b = poly[(i + 1) % poly.Count];
                Debug.DrawLine(a, b, color, debugDrawDuration);
            }
        }
    }
}
