using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの移動慣性を制御し、滑りやすさを変更するクラス。
    /// 特定のエリアにおいて Rigidbody2D の空気抵抗（linearDamping）を調整し、
    /// 氷の上を滑るような操作感を提供します。
    /// </summary>
    public class MoveLockAbility
    {
        /// <summary>滑りやすい状態の空気抵抗値</summary>
        private const float SlipperyDamping = 0.05f;

        /// <summary>
        /// 移動慣性の状態を Rigidbody2D に適用します。
        /// </summary>
        /// <param name="rb">制御対象の Rigidbody2D</param>
        /// <param name="isLocked">慣性移動が有効（滑る状態）かどうか</param>
        public void Apply(Rigidbody2D rb, bool isLocked)
        {
            if (!isLocked) return;

            rb.linearDamping = SlipperyDamping;
        }
    }
}