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

        // 領域を広げる幅
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

                    // 線からの距離を計算 (外積の結果を線の長さで割ると距離が出る)
                    float lineLength = Vector2.Distance(split.Intersection0, split.Intersection1);
                    float signedDistance = ((split.Intersection1.x - split.Intersection0.x) * (wy - split.Intersection0.y) -
                                            (split.Intersection1.y - split.Intersection0.y) * (wx - split.Intersection0.x)) / lineLength;

                    // 境界付近（例：0.05ユニット以内）で色を混ぜる
                    float antiAliasRange = 0.05f;
                    float t = Mathf.Clamp01((signedDistance / antiAliasRange) * 0.5f + 0.5f);

                    // 0に近いほど青、1に近いほど緑
                    Color finalColor = Color.Lerp(Color.blue, Color.green, t);
                    currentTexture.SetPixel(x, y, finalColor);
                }
            }
            // フィルターモードをBilinearにして滑らかにする
            currentTexture.filterMode = FilterMode.Bilinear;
            currentTexture.Apply();
            currentTexture.Apply();

            displayRenderer.sprite = Sprite.Create(currentTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);

            // --- [修正箇所] Z座標を worldZ より大きくして後ろに下げる ---
            // ターゲット（玉）が worldZ(0) にあるなら、板は 0.5f くらいにすると確実に後ろへ行きます
            float backZ = worldZ + 0.5f;
            displayRenderer.transform.position = new Vector3(visualRect.center.x, visualRect.center.y, backZ);
            // --------------------------------------------------------

            float spriteOriginalWorldWidth = width / 100f;
            float spriteOriginalWorldHeight = height / 100f;
            displayRenderer.transform.localScale = new Vector3(visualRect.width / spriteOriginalWorldWidth, visualRect.height / spriteOriginalWorldHeight, 1f);

            // [追加アドバイス] さらに確実にするなら Order in Layer を下げる
            displayRenderer.sortingOrder = -1;
        }
    }
}