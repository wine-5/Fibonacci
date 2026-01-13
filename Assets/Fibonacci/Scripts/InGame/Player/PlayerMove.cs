using UnityEngine;
using Fibonacci.InGame;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動・物理計算の純粋なロジックを担当
    /// </summary>
    public class PlayerMove
    {
        private const float ROTATION_RIGHT = 0f;
        private const float ROTATION_LEFT = 180f;

        private readonly Rigidbody2D rb;
        private readonly Transform transform;
        private readonly PlayerAnimationController anim;

        private readonly float moveSpeed;
        private readonly Vector3 initialPosition;

        public Vector2 MoveInput { get; set; }

        public PlayerMove(Rigidbody2D rb, Transform transform, PlayerAnimationController anim, float speed)
        {
            this.rb = rb;
            this.transform = transform;
            this.anim = anim;
            moveSpeed = speed;
            initialPosition = transform.position;
        }

        public void ExecutePhysicsUpdate()
        {
            Vector2 targetVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = targetVelocity;

            if (MoveInput.x != 0)
            {
                float yRotation = MoveInput.x > 0 ? ROTATION_RIGHT : ROTATION_LEFT;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
        /// <summary> アニメーションを更新します。</summary>
        public void UpdateAnimation()
        {
            if (anim != null)
                anim.UpdateMoveAnimation(MoveInput);
        }

        public void ResetPosition()
        {
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            MoveInput = Vector2.zero;
        }
    }
}