namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの物理的な干渉力を強化するロジックを管理するクラス。
    /// 自身で値を適用せず、計算結果としての質量のみを返すことで疎結合な設計を維持します。
    /// </summary>
    public class PowerUpAbility
    {
        private const float POWER_UP_MASS = 2.0f;

        /// <summary>
        /// パワーアップ状態が有効な場合、適用すべき質量を返します。
        /// </summary>
        public float? GetAppliedMass(bool isPowerUp)
        {
            if (!isPowerUp) return null;

            return POWER_UP_MASS;
        }
    }
}