using System.Collections.Generic;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    public class BorderLineFrameMap
    {
        private const float FRAME_WIDTH = 0.25f;

        private readonly SpriteRenderer displayRenderer;
        private readonly float worldZ;
        private readonly int resolution;
        private Texture2D currentTexture;

        public BorderLineFrameMap(SpriteRenderer displayRenderer, float worldZ, int resolution)
        {
            this.displayRenderer = displayRenderer;
            this.worldZ = worldZ;
            this.resolution = resolution;
        }

        public void UpdateFrame(BorderLineRegionSplitter.SplitResult split, Color color0, Color color1)
        {
            if (displayRenderer == null || split.Polygon1 == null || split.Polygon2 == null) return;

            Rect rect = split.Rect;
            int width = resolution;
            int height = Mathf.Max(1, Mathf.RoundToInt(resolution * (rect.height / rect.width)));

            if (currentTexture == null || currentTexture.width != width || currentTexture.height != height)
            {
                if (currentTexture != null) Object.Destroy(currentTexture);
                currentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                currentTexture.filterMode = FilterMode.Bilinear;
                currentTexture.wrapMode = TextureWrapMode.Clamp;
            }

            Color[] pixels = new Color[width * height];
            float sqrThreshold = FRAME_WIDTH * FRAME_WIDTH;

            Vector2 i0 = split.Intersection0;
            Vector2 i1 = split.Intersection1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float wx = rect.xMin + ((float)x / width) * rect.width;
                    float wy = rect.yMin + ((float)y / height) * rect.height;
                    Vector2 worldPos = new Vector2(wx, wy);

                    int index = y * width + x;

                    if (IsNearEdgeExcludingSplit(worldPos, split.Polygon1, sqrThreshold, i0, i1))
                    {
                        pixels[index] = color0;
                    }
                    else if (IsNearEdgeExcludingSplit(worldPos, split.Polygon2, sqrThreshold, i0, i1))
                    {
                        pixels[index] = color1;
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
            }

            currentTexture.SetPixels(pixels);
            currentTexture.Apply();

            ApplyToRenderer(width, height, rect);
        }

        private bool IsNearEdgeExcludingSplit(Vector2 point, List<Vector2> polygon, float sqrThreshold, Vector2 i0, Vector2 i1)
        {
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[(i + 1) % polygon.Count];

                bool isSplitEdge = (IsApprox(a, i0) && IsApprox(b, i1)) || (IsApprox(a, i1) && IsApprox(b, i0));

                if (isSplitEdge) continue;

                if (SqrDistanceToSegment(point, a, b) < sqrThreshold) return true;
            }
            return false;
        }

        private bool IsApprox(Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).sqrMagnitude < 0.0001f;
        }

        private float SqrDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            float l2 = (a - b).sqrMagnitude;
            if (l2 == 0.0f) return (p - a).sqrMagnitude;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, b - a) / l2);
            return (p - (a + t * (b - a))).sqrMagnitude;
        }

        private void ApplyToRenderer(int width, int height, Rect rect)
        {
            if (displayRenderer.sprite != null) Object.Destroy(displayRenderer.sprite);

            float dynamicPPU = width / rect.width;

            displayRenderer.sprite = Sprite.Create(
                currentTexture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                dynamicPPU
            );

            displayRenderer.transform.position = new Vector3(rect.center.x, rect.center.y, worldZ);
            displayRenderer.transform.localScale = Vector3.one;
        }

        public void ClearVisual()
        {
            if (displayRenderer != null) displayRenderer.sprite = null;
            if (currentTexture != null)
            {
                Object.Destroy(currentTexture);
                currentTexture = null;
            }
        }
    }
}