namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの重量増加と速度低下の計算ロジックを管理するクラス。
    /// 自身で値を適用せず、計算結果のみを返すことで他クラスとの依存関係を排除しています。
    /// </summary>
    public class HeavyAbility
    {
        private const float HEAVY_MASS = 5.0f;
        private const float SLOW_SPEED = 2.0f;

        /// <summary>
        /// 重重量状態が有効な場合、適用すべき物理パラメーターを返します。
        /// </summary>
        public (float mass, float speed)? GetAppliedValues(bool isHeavy)
        {
            if (!isHeavy) return null;

            return (HEAVY_MASS, SLOW_SPEED);
        }
    }
}