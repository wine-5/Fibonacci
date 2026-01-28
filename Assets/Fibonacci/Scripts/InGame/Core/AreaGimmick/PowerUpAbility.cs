using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの物理的な干渉力を強化するロジックを管理するクラス。
    /// 質量を一時的に増加させることで、重量オブジェクトを押し出す能力を提供します。
    /// </summary>
    public class PowerUpAbility
    {
        private const float POWERUPMASS = 2.0f;

        /// <summary>
        /// パワーアップ状態に応じた質量を Rigidbody2D に適用します。
        /// </summary>
        public void Apply(Rigidbody2D rb, bool isPowerUp)
        {
            if (!isPowerUp) return;

            rb.mass = POWERUPMASS;
        }
    }
}