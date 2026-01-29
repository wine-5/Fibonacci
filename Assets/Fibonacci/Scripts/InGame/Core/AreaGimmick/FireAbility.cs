namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// 火属性エリアにおける滞在時間制限のロジックを管理するクラス。
    /// 自身でイベントを発火させず、時間切れの判定結果を返すことで疎結合な設計を維持します。
    /// </summary>
    public class FireAbility
    {
        private const float LIMIT_TIME = 4.0f;
        
        private float timer = 0f;

        /// <summary>
        /// 現在のアビリティ状態に基づき、タイマーを更新して「リスタートが必要か」を返します。
        /// </summary>
        public bool Tick(bool isFireActive, float deltaTime)
        {
            if (!isFireActive)
            {
                timer = 0f;
                return false;
            }

            timer += deltaTime;

            if (timer >= LIMIT_TIME)
            {
                timer = 0f;
                return true;
            }

            return false;
        }

        /// <summary>
        /// タイマーを強制的にリセットします。
        /// </summary>
        public void Reset()
        {
            timer = 0f;
        }
    }
}