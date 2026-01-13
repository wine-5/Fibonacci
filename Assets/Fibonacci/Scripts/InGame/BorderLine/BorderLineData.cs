using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 境界線の幾何学的な座標データと、それに基づく領域判定ロジックを保持するデータクラス（ブリッジ）。
    /// Unityの描画（Texture2Dなど）からは完全に独立しており、純粋な数学的計算によって
    /// プレイヤーが「線に対してどちら側にいるか」を高速に判定する責任を持ちます。
    /// </summary>
    public class BorderLineData
    {
        public Vector2 Intersection0 { get; private set; }
        public Vector2 Intersection1 { get; private set; }
        public bool IsActive { get; private set; }


        /// <summary>
        /// 線のデータを更新します。
        /// </summary>
        public void UpdateData(Vector2 p0, Vector2 p1)
        {
            Intersection0 = p0;
            Intersection1 = p1;
            IsActive = true;
        }

        /// <summary>
        /// 線のデータをクリアします。
        /// </summary>
        public void Clear()
        {
            IsActive = false;
        }

        /// <summary>
        /// 指定した座標がどちらのエリアにあるかを判定します。
        /// 外積（Cross Product）を利用した数学的判定により、テクスチャの色を確認するより高速かつ正確です。
        /// </summary>
        /// <returns>エリア番号 (0 or 1)。無効な場合は -1</returns>
        public int GetAreaIndex(Vector2 worldPos)
        {
            if (!IsActive) return -1;

            float crossProduct = (Intersection1.x - Intersection0.x) * (worldPos.y - Intersection0.y) -
                                 (Intersection1.y - Intersection0.y) * (worldPos.x - Intersection0.x);
            return (crossProduct >= 0) ? 1 : 0;
        }
    }
}