namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// 特定のエリア内でのみジャンプアクションを許可するためのフラグ管理クラス。
    /// プレイヤーの移動制限エリアや特殊アクションエリアの判定に使用されます。
    /// </summary>
    public class JumpAbility
    {
        /// <summary>
        /// 現在ジャンプが可能かどうかを示す値を取得します。
        /// </summary>
        public bool CanJump { get; private set; }

        /// <summary>
        /// ジャンプ許可状態を更新します。
        /// エリア外に出た場合はジャンプ不可状態にリセットします。
        /// </summary>
        public void Apply(bool isActive)
        {
            // ルールに従い、引数の状態をそのままプロパティに反映させる
            CanJump = isActive;
        }
    }
}