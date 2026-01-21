using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    /// <summary>
    /// プレイヤーに対する重力操作の具体的な計算と適用ロジックを定義するクラス。
    /// ステートレス（状態を持たない）な設計により、外部から渡されたRigidbody2DとTransformに対して
    /// 重力方向の変更、速度のリセット、および見た目の反転処理を直接実行します。
    /// </summary>

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Transform))]

    public class GravityAbility
    {
        public void Apply(Rigidbody2D rb, Transform trans, bool isInverted, float baseScale = 1.0f)
        {
            float currentAbsScale = Mathf.Abs(rb.gravityScale);
            rb.gravityScale = isInverted ? -currentAbsScale : currentAbsScale;

            Vector3 scale = trans.localScale;
            scale.y = (rb.gravityScale < 0) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            trans.localScale = scale;
        }
    }
}