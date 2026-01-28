using UnityEngine;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// 火属性エリアにおける特殊効果と時間制限によるペナルティを管理するクラス。
    /// エリア滞在時間を計測し、一定時間を超えた場合にリスタートイベントを発火させます。
    /// </summary>
    public class FireAbility
    {
        private float timer = 0f;
        
        private const float LimitTime = 5.0f;
        
        private const int InvalidAreaIndex = -1;

        /// <summary>
        /// 火属性アビリティの見た目と有効状態を切り替えます。
        /// </summary>
        /// <param name="isActive">アビリティが有効かどうか</param>
        /// <param name="displayRenderer">アビリティアイコンを表示するレンダラー</param>
        public void Apply(bool isActive, SpriteRenderer displayRenderer)
        {
            if (isActive)
            {
                displayRenderer.sprite = AbilityManager.Instance.GetAbilitySprite(AbilityType.Fire);
                displayRenderer.enabled = true;
                return;
            }

            displayRenderer.enabled = false;
        }

        /// <summary>
        /// 毎フレームの滞在時間計測処理を実行します。
        /// </summary>
        /// <param name="currentAreaIndex">現在プレイヤーが滞在しているエリアのインデックス</param>
        public void Tick(int currentAreaIndex)
        {
            if (currentAreaIndex == InvalidAreaIndex)
            {
                ResetTimer();
                return;
            }

            AbilityType currentAbility = AbilityManager.Instance.GetAbilityAt(currentAreaIndex);
            bool isFiring = currentAbility == AbilityType.Fire;

            if (!isFiring)
            {
                ResetTimer();
                return;
            }

            timer += Time.fixedDeltaTime;

            if (timer >= LimitTime)
            {
                timer = 0f;
                GameEvents.TriggerRestart();
            }
        }

        private void ResetTimer()
        {
            timer = 0f;
        }
    }
}