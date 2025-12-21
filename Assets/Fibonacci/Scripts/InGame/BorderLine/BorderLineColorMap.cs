using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    public class BorderLineColorMap
    {
        private readonly SpriteRenderer displayRenderer;
        private readonly float worldZ;
        private readonly int resolution;
        private Texture2D currentTexture;
        private Rect currentRect;

        private const float PADDING = 2.0f;

        public BorderLineColorMap(SpriteRenderer displayRenderer, float worldZ, int resolution)
        {
            this.displayRenderer = displayRenderer;
            this.worldZ = worldZ;
            this.resolution = resolution;
        }

        // --- [ここを追加] プレイヤーの座標からエリア番号を返す ---
        public int GetAreaIndex(Vector2 worldPos)
        {
            if (currentTexture == null || !currentRect.Contains(worldPos)) return -1;

            float tx = (worldPos.x - currentRect.xMin) / currentRect.width;
            float ty = (worldPos.y - currentRect.yMin) / currentRect.height;
            Color c = currentTexture.GetPixelBilinear(tx, ty);

            if (c.a < 0.1f) return -1;
            return (c.g > c.b) ? 1 : 0; // 緑(G)が多ければ1、青(B)が多ければ0
        }
        // -----------------------------------------------------

        public void UpdateVisual(BorderLineRegionSplitter.SplitResult split)
        {
            if (displayRenderer == null) return;

            Rect visualRect = split.Rect;
            visualRect.xMin -= PADDING;
            visualRect.yMin -= PADDING;
            visualRect.width += PADDING * 2;
            visualRect.height += PADDING * 2;

            currentRect = visualRect;

            int width = resolution;
            int height = Mathf.RoundToInt(resolution * (visualRect.height / visualRect.width));
            if (height < 1) height = 1;

            if (currentTexture != null) Object.Destroy(currentTexture);
            currentTexture = new Texture2D(width, height);
            currentTexture.filterMode = FilterMode.Point;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float wx = visualRect.xMin + ((float)x / width) * visualRect.width;
                    float wy = visualRect.yMin + ((float)y / height) * visualRect.height;

                    float lineLength = Vector2.Distance(split.Intersection0, split.Intersection1);
                    float signedDistance = ((split.Intersection1.x - split.Intersection0.x) * (wy - split.Intersection0.y) -
                                            (split.Intersection1.y - split.Intersection0.y) * (wx - split.Intersection0.x)) / lineLength;

                    float antiAliasRange = 0.05f;
                    float t = Mathf.Clamp01((signedDistance / antiAliasRange) * 0.5f + 0.5f);

                    Color finalColor = Color.Lerp(Color.blue, Color.green, t);
                    currentTexture.SetPixel(x, y, finalColor);
                }
            }
            currentTexture.filterMode = FilterMode.Bilinear;
            currentTexture.Apply();

            displayRenderer.sprite = Sprite.Create(currentTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);

            float backZ = worldZ + 0.5f;
            displayRenderer.transform.position = new Vector3(visualRect.center.x, visualRect.center.y, backZ);

            float spriteOriginalWorldWidth = width / 100f;
            float spriteOriginalWorldHeight = height / 100f;
            displayRenderer.transform.localScale = new Vector3(visualRect.width / spriteOriginalWorldWidth, visualRect.height / spriteOriginalWorldHeight, 1f);

            displayRenderer.sortingOrder = -1;
        }

        public void UpdateVisual(BorderLineRegionSplitter.PartitionResult partition)
        {
            var split = new BorderLineRegionSplitter.SplitResult
            {
                Rect = partition.Rect,
                Intersection0 = partition.Intersection0,
                Intersection1 = partition.Intersection1
            };

            if (partition.Regions != null && partition.Regions.Count >= 2)
            {
                split.Polygon1 = partition.Regions[0].Polygon;
                split.Polygon2 = partition.Regions[1].Polygon;
                split.Centroid1 = partition.Regions[0].Centroid;
                split.Centroid2 = partition.Regions[1].Centroid;
            }
            else
            {
                split.Polygon1 = new System.Collections.Generic.List<Vector2>();
                split.Polygon2 = new System.Collections.Generic.List<Vector2>();
                split.Centroid1 = partition.Rect.center;
                split.Centroid2 = partition.Rect.center;
            }

            UpdateVisual(split);
        }

        public void ClearVisual()
        {
            if (displayRenderer != null)
            {
                displayRenderer.sprite = null;
            }

            if (currentTexture != null)
            {
                Object.Destroy(currentTexture);
                currentTexture = null;
            }

            currentRect = default;
        }
    }
}