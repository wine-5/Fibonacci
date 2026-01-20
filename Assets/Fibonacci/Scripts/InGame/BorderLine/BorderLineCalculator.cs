using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 境界線に関する数学的な計算（幾何計算）を専門に行うクラス。
    /// </summary>
    public static class BorderLineCalculator
    {
        /// <summary>
        /// 点が線のどちら側にあるかを判定する（外積を利用）
        /// </summary>
        /// <returns>1: 左/上側, 0: 右/下側</returns>
        public static int DetermineAreaIndex(Vector2 lineP0, Vector2 lineP1, Vector2 targetPos)
        {
            float crossProduct = (lineP1.x - lineP0.x) * (targetPos.y - lineP0.y) -
                                 (lineP1.y - lineP0.y) * (targetPos.x - lineP0.x);
            
            return (crossProduct >= 0) ? 1 : 0;
        }
    }
}