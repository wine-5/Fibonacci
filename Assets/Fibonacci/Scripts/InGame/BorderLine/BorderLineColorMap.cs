using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 境界線によって分割された領域を視覚的に塗り分けるためのテクスチャ描画クラス。
    /// 指定された解像度と計算式に基づき、動的にスプライトを生成します。
    /// </summary>
    public class BorderLineColorMap
    {
        private const float PADDING = 2.0f;
        private const float ANTI_ALIAS_RANGE = 0.05f;
        private const float PIXELS_PER_UNIT = 100f;
        private const int SORTING_ORDER = -1;
        private const float Z_OFFSET = 0.5f;

        private const float LERP_ADJUST_MULTIPLIER = 0.5f;
        private const float LERP_ADJUST_OFFSET = 0.5f;
        private const float SPRITE_PIVOT_CENTER = 0.5f;

        private readonly SpriteRenderer displayRenderer;
        private readonly float worldZ;
        private readonly int resolution;
        private Texture2D currentTexture;

        public BorderLineColorMap(SpriteRenderer displayRenderer, float worldZ, int resolution)
        {
            this.displayRenderer = displayRenderer;
            this.worldZ = worldZ;
            this.resolution = resolution;
        }

        public void UpdateVisual(BorderLineRegionSplitter.SplitResult split)
        {
            if (displayRenderer == null) return;

            Rect visualRect = split.Rect;
            visualRect.xMin -= PADDING;
            visualRect.yMin -= PADDING;
            visualRect.width += PADDING * 2;
            visualRect.height += PADDING * 2;

            int width = resolution;
            int height = Mathf.Max(1, Mathf.RoundToInt(resolution * (visualRect.height / visualRect.width)));

            if (currentTexture != null) Object.Destroy(currentTexture);
            currentTexture = new Texture2D(width, height);
            currentTexture.filterMode = FilterMode.Point;

            float lineLength = Vector2.Distance(split.Intersection0, split.Intersection1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float wx = visualRect.xMin + ((float)x / width) * visualRect.width;
                    float wy = visualRect.yMin + ((float)y / height) * visualRect.height;

                    float signedDistance = ((split.Intersection1.x - split.Intersection0.x) * (wy - split.Intersection0.y) -
                                            (split.Intersection1.y - split.Intersection0.y) * (wx - split.Intersection0.x)) / lineLength;

                    float t = Mathf.Clamp01((signedDistance / ANTI_ALIAS_RANGE) * LERP_ADJUST_MULTIPLIER + LERP_ADJUST_OFFSET);

                    Color finalColor = Color.Lerp(Color.blue, Color.green, t);
                    currentTexture.SetPixel(x, y, finalColor);
                }
            }

            currentTexture.filterMode = FilterMode.Bilinear;
            currentTexture.Apply();

            displayRenderer.sprite = Sprite.Create(
                currentTexture, 
                new Rect(0, 0, width, height), 
                new Vector2(SPRITE_PIVOT_CENTER, SPRITE_PIVOT_CENTER), 
                PIXELS_PER_UNIT
            );

            displayRenderer.transform.position = new Vector3(visualRect.center.x, visualRect.center.y, worldZ + Z_OFFSET);
            
            float spriteOriginalWorldWidth = width / PIXELS_PER_UNIT;
            float spriteOriginalWorldHeight = height / PIXELS_PER_UNIT;
            
            displayRenderer.transform.localScale = new Vector3(
                visualRect.width / spriteOriginalWorldWidth, 
                visualRect.height / spriteOriginalWorldHeight, 
                1f
            );
            
            displayRenderer.sortingOrder = SORTING_ORDER;
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
        }
    }
}