using UnityEngine;

using Fibonacci.Event;


namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// 重力操作の「手順」のみを定義したピュアC#クラス。
    /// 自身は状態を持たず、渡されたコンポーネントに対して操作を実行します。　
    /// </summary>
    public class PlayerGravityLogic
    {
        public void Execute(Rigidbody2D rb, Transform trans, int areaIndex)
        {
            if (rb == null || trans == null) return;

            float targetScale = (areaIndex == 1) ? -1.0f : 1.0f;

            rb.gravityScale = targetScale;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            Vector3 scale = trans.localScale;
            scale.y = (targetScale < 0) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            trans.localScale = scale;

            

        }
        private void OnGravityRestart()
        {
        }

        
    }
}