using UnityEngine;

namespace BorderLine
{
    /// <summary>
    /// LineRendererを使って「2点間の1本の線分」を描画/非表示する責務だけを持つクラス。
    /// 入力・選択・領域計算などは担当しない。
    /// </summary>
    public sealed class BorderLineSegmentRenderer
    {
        private readonly LineRenderer lineRenderer;

        public BorderLineSegmentRenderer(LineRenderer lineRenderer)
        {
            this.lineRenderer = lineRenderer;
        }

        public bool IsValid => lineRenderer != null;

        public void Hide()
        {
            if (lineRenderer == null) return;
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        public void Draw(Vector3 startPos, Vector3 endPos)
        {
            if (lineRenderer == null) return;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}
