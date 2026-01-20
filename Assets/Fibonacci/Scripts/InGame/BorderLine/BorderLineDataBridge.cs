using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 現在の境界線の幾何学的な位置情報のみを保持するクラス。
    /// </summary>
    public class BorderLineDataBridge
    {
        public Vector2 P0 { get; private set; }
        public Vector2 P1 { get; private set; }
        public bool IsActive { get; private set; }

        public void SetLine(Vector2 p0, Vector2 p1)
        {
            P0 = p0;
            P1 = p1;
            IsActive = true;
        }

        public void Clear()
        {
            IsActive = false;
        }
    }
}