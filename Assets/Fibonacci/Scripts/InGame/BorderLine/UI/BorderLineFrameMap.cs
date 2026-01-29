using System.Collections.Generic;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    /// <summary>
    /// 分割されたエリアの境界線（フレーム）をテクスチャとして生成し、表示を管理するクラス。
    /// 指定された解像度に基づき、各ポリゴンの外周を特定の色で描画します。
    /// </summary>
    public class BorderLineFrameMap
    {
        private const float FRAME_WIDTH = 0.25f;
        private const float APPROX_THRESHOLD = 0.0001f;
        private const float PIVOT_CENTER = 0.5f;
        private const float MIN_HEIGHT = 1.0f;

        private readonly SpriteRenderer displayRenderer;
        private readonly float worldZ;
        private readonly int resolution;
        
        private Texture2D currentTexture;
        private Color[] pixelCache;

        public BorderLineFrameMap(SpriteRenderer displayRenderer, float worldZ, int resolution)
        {
            this.displayRenderer = displayRenderer;
            this.worldZ = worldZ;
            this.resolution = resolution;
        }

        /// <summary>
        /// 分割結果と指定色に基づき、フレームテクスチャを更新してレンダラーに適用します。
        /// </summary>
        public void UpdateFrame(BorderLineRegionSplitter.SplitResult split, Color color0, Color color1)
        {
            if (displayRenderer == null || split.Polygon1 == null || split.Polygon2 == null) return;

            Rect rect = split.Rect;
            int width = resolution;
            int height = Mathf.Max((int)MIN_HEIGHT, Mathf.RoundToInt(resolution * (rect.height / rect.width)));

            if (currentTexture == null || currentTexture.width != width || currentTexture.height != height)
            {
                RecreateTexture(width, height);
            }

            float sqrThreshold = FRAME_WIDTH * FRAME_WIDTH;
            Vector2 i0 = split.Intersection0;
            Vector2 i1 = split.Intersection1;

            for (int y = 0; y < height; y++)
            {
                float normalizedY = (float)y / height;
                float wy = rect.yMin + normalizedY * rect.height;

                for (int x = 0; x < width; x++)
                {
                    float normalizedX = (float)x / width;
                    float wx = rect.xMin + normalizedX * rect.width;
                    
                    Vector2 worldPos = new Vector2(wx, wy);
                    int index = y * width + x;

                    if (IsNearEdgeExcludingSplit(worldPos, split.Polygon1, sqrThreshold, i0, i1))
                    {
                        pixelCache[index] = color0;
                    }
                    else if (IsNearEdgeExcludingSplit(worldPos, split.Polygon2, sqrThreshold, i0, i1))
                    {
                        pixelCache[index] = color1;
                    }
                    else
                    {
                        pixelCache[index] = Color.clear;
                    }
                }
            }

            currentTexture.SetPixels(pixelCache);
            currentTexture.Apply();

            ApplyToRenderer(width, height, rect);
        }

        private void RecreateTexture(int width, int height)
        {
            if (currentTexture != null) Object.Destroy(currentTexture);
            
            currentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            currentTexture.filterMode = FilterMode.Bilinear;
            currentTexture.wrapMode = TextureWrapMode.Clamp;
            
            pixelCache = new Color[width * height];
        }

        private bool IsNearEdgeExcludingSplit(Vector2 point, List<Vector2> polygon, float sqrThreshold, Vector2 i0, Vector2 i1)
        {
            int count = polygon.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[(i + 1) % count];

                bool isSplitEdge = (IsApprox(a, i0) && IsApprox(b, i1)) || (IsApprox(a, i1) && IsApprox(b, i0));
                if (isSplitEdge) continue;

                if (SqrDistanceToSegment(point, a, b) < sqrThreshold) return true;
            }
            return false;
        }

        private bool IsApprox(Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).sqrMagnitude < APPROX_THRESHOLD;
        }

        private float SqrDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float l2 = ab.sqrMagnitude;
            if (l2 == 0.0f) return (p - a).sqrMagnitude;
            
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / l2);
            return (p - (a + t * ab)).sqrMagnitude;
        }

        private void ApplyToRenderer(int width, int height, Rect rect)
        {
            if (displayRenderer.sprite != null) Object.Destroy(displayRenderer.sprite);

            float dynamicPPU = width / rect.width;
            Vector2 pivot = new Vector2(PIVOT_CENTER, PIVOT_CENTER);

            displayRenderer.sprite = Sprite.Create(
                currentTexture,
                new Rect(0, 0, width, height),
                pivot,
                dynamicPPU
            );

            displayRenderer.transform.position = new Vector3(rect.center.x, rect.center.y, worldZ);
            displayRenderer.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 生成されたテクスチャとスプライトを破棄し、描画をクリアします。
        /// </summary>
        public void ClearVisual()
        {
            if (displayRenderer != null) displayRenderer.sprite = null;
            if (currentTexture != null)
            {
                Object.Destroy(currentTexture);
                currentTexture = null;
            }
            pixelCache = null;
        }
    }
}