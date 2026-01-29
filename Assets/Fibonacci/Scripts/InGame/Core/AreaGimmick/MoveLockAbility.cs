namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの移動慣性を制御し、滑りやすさを変更するためのロジックを管理するクラス。
    /// 自身で値を適用せず、計算結果のみを返すことで疎結合な設計を維持します。
    /// </summary>
    public class MoveLockAbility
    {
        private const float SLIPPERY_DAMPING = 0.05f;

        /// <summary>
        /// 移動慣性の状態（滑る状態）が有効な場合、適用すべき空気抵抗値を返します。
        /// </summary>
        public float? GetAppliedDamping(bool isLocked)
        {
            if (!isLocked) return null;

            return SLIPPERY_DAMPING;
        }
    }
}