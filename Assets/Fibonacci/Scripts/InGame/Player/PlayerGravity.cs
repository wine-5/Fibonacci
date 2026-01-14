// using UnityEngine;

// namespace Fibonacci.InGame.Player
// {
//     /// <summary>
//     /// プレイヤーの重力制御と、それに伴う反転処理を管理するクラス。
//     /// Rigidbody2Dの重力スケール変更と同時に、プレイヤーの上下の向き(localScale.y)を
//     /// 適切に同期させ、物理的な落下方向とビジュアルを一致させる役割を担います。
//     /// </summary>
//     [RequireComponent(typeof(Rigidbody2D))]
//     public class PlayerGravity : MonoBehaviour
//     {
//         private Rigidbody2D rb;

//         void Awake()
//         {
//             rb = GetComponent<Rigidbody2D>();
//         }

//         public float GetGravity() => rb != null ? rb.gravityScale : 1f;

//         public void ReverseGravity()
//         {
//             if (rb != null)
//             {
//                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

//                 Vector3 scale = transform.localScale;
//                 scale.y *= -1;
//                 transform.localScale = scale;
//             }
//         }
//         public void SetGravityScale(float scale)
//         {
//             if (rb == null) return;

//             rb.gravityScale = scale;
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

//             Vector3 localScale = transform.localScale;
//             localScale.y = (scale < 0) ? -Mathf.Abs(localScale.y) : Mathf.Abs(localScale.y);
//             transform.localScale = localScale;
//         }

//         public void SetNormalGravity()
//         {
//             SetGravityScale(1f);
//         }
//     }
// }