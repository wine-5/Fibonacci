using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// 低重力エリアにおける浮遊感や空気抵抗の変化を管理するクラス。
    /// 落下速度を抑制するために Rigidbody2D の抵抗値を直接操作します。
    /// </summary>
    public class LowGravityAbility
    {
        private const float LowGravityDrag = 5.0f;

        /// <summary>
        /// 低重力状態に応じた空気抵抗を Rigidbody2D に適用します。
        /// </summary>
        public void Apply(Rigidbody2D rb, bool isLow)
        {
            if (!isLow) return;

            rb.linearDamping = LowGravityDrag;
        }
    }
}