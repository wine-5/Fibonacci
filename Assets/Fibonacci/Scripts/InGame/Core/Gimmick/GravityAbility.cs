using UnityEngine;

namespace Fibonacci.InGame.Core.Gimmick
{
    /// <summary>
    /// プレイヤーに対する重力操作の具体的な計算と適用ロジックを定義するクラス。
    /// ステートレス（状態を持たない）な設計により、外部から渡されたRigidbody2DとTransformに対して
    /// 重力方向の変更、速度のリセット、および見た目の反転処理を直接実行します。
    /// </summary>
    public class GravityAbility
    {
        public void Apply(Rigidbody2D rb, Transform trans, int areaIndex)
        {
            if (rb == null || trans == null) return;

            float targetScale = (areaIndex == 1) ? -1.0f : 1.0f;

            rb.gravityScale = targetScale;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            Vector3 scale = trans.localScale;
            scale.y = (targetScale < 0) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            trans.localScale = scale;
        }
    }
}