namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// 低重力エリアにおける空気抵抗（Damping）の計算ロジックを管理するクラス。
    /// 自身で値を適用せず、計算結果のみを返すことで疎結合な設計を維持します。
    /// </summary>
    public class LowGravityAbility
    {
        private const float LOW_GRAVITY_DRAG = 5.0f;

        /// <summary>
        /// 低重力状態が有効な場合、適用すべき空気抵抗値を返します。
        /// </summary>
        public float? GetAppliedDrag(bool isLow)
        {
            if (!isLow) return null;

            return LOW_GRAVITY_DRAG;
        }
    }
}