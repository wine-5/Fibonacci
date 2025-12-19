using UnityEngine;

namespace BorderLine
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

        private GameObject regionMarker1;
        private GameObject regionMarker2;

        public BorderLineRegionDebugView(Transform parent, float worldZ, float markerTextSize, float debugDrawDuration)
        {
            this.parent = parent;
            this.worldZ = worldZ;
            this.markerTextSize = markerTextSize;
            this.debugDrawDuration = debugDrawDuration;
        }

        public void EnsureMarkers()
        {
            if (regionMarker1 == null)
            {
                regionMarker1 = CreateTextMarker("RegionMarker1", "1", Color.cyan);
            }
            if (regionMarker2 == null)
            {
                regionMarker2 = CreateTextMarker("RegionMarker2", "2", Color.magenta);
            }
        }

        public void SetMarkerPositions(Vector2 c1, Vector2 c2)
        {
            if (regionMarker1 != null) regionMarker1.transform.position = new Vector3(c1.x, c1.y, worldZ);
            if (regionMarker2 != null) regionMarker2.transform.position = new Vector3(c2.x, c2.y, worldZ);
        }

        public void DrawPolygons(BorderLineRegionSplitter.SplitResult split)
        {
            DrawPolygonDebug(split.Polygon1, Color.cyan);
            DrawPolygonDebug(split.Polygon2, Color.magenta);
            Debug.DrawLine(split.Intersection0, split.Intersection1, Color.white, debugDrawDuration);
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
