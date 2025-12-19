using UnityEngine;

namespace Fibonacci.InGame.BorderLine
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

        public void Hide()
        {
            if (lineRenderer == null) return;
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        private void Draw(Vector3 startPos, Vector3 endPos)
        {
            if (lineRenderer == null) return;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }

        public void DrawSplitOrSegment(
            Vector3 segmentStart,
            Vector3 segmentEnd,
            Vector2 intersection0,
            Vector2 intersection1,
            bool extendToBounds,
            float z = 0f)
        {
            if (extendToBounds)
            {
                Draw(new Vector3(intersection0.x, intersection0.y, z), new Vector3(intersection1.x, intersection1.y, z));
            }
            else
            {
                Draw(segmentStart, segmentEnd);
            }
        }
    }
}
