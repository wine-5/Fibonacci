using UnityEngine;
using Fibonacci.InGame.Player;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの重量を増加させ、移動速度を低下させるギミックを制御するクラス。
    /// 特定のエリアにおいて物理的な重さと操作の鈍さを提供します。
    /// </summary>
    public class HeavyAbility
    {
        private const float HeavyMass = 5.0f;
        private const float SlowSpeed = 2.0f;

        /// <summary>
        /// 重重量状態を Rigidbody2D および移動制御クラスに適用します。
        /// </summary>
        public void Apply(Rigidbody2D rb, PlayerMove playerMove, bool isHeavy)
        {
            if (!isHeavy) return;

            rb.mass = HeavyMass;
            playerMove.SetCurrentSpeed(SlowSpeed);
        }
    }
}