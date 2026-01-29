using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーの重力反転に関する計算ロジックを管理するクラス。
    /// 自身で物理挙動や座標更新を行わず、適用すべきスケール値のみを返します。
    /// </summary>
    public class GravityAbility
    {
        /// <summary>
        /// 重力反転状態に基づき、Rigidbodyの重力スケールと見た目のYスケール倍率を算出します。
        /// </summary>
        public (float gravityScale, float visualScaleY) GetAppliedScales(float currentGravityScale, bool isInverted)
        {
            float targetGravity = isInverted ? -Mathf.Abs(currentGravityScale) : Mathf.Abs(currentGravityScale);
            float targetVisualY = (targetGravity < 0) ? -1f : 1f;

            return (targetGravity, targetVisualY);
        }
    }
}