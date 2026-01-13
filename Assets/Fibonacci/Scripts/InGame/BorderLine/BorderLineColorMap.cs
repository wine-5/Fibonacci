using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 境界線によって分割された領域を、視覚的に青と緑で塗り分けるためのテクスチャ描画クラス。
    /// BorderLineData から渡される座標情報に基づき、SpriteRenderer に適用するスプライトを
    /// 動的に生成・更新する「表示」の専門家です。判定ロジックは持ちません。
    /// </summary>
    public class BorderLineColorMap
    {
        private readonly SpriteRenderer displayRenderer;
        private readonly float worldZ;
        private readonly int resolution;
        private Texture2D currentTexture;

        private const float PADDING = 2.0f;

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

            displayRenderer.transform.position = new Vector3(visualRect.center.x, visualRect.center.y, worldZ + 0.5f);
            float spriteOriginalWorldWidth = width / 100f;
            float spriteOriginalWorldHeight = height / 100f;
            displayRenderer.transform.localScale = new Vector3(visualRect.width / spriteOriginalWorldWidth, visualRect.height / spriteOriginalWorldHeight, 1f);
            displayRenderer.sortingOrder = -1;
        }

        /// <summary>
        /// 描画されている色（テクスチャ）を消去し、表示をクリアします。
        /// </summary>
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