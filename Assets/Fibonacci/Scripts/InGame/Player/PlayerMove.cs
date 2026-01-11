using UnityEngine;
using Fibonacci.InGame;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動・物理計算の純粋なロジックを担当
    /// </summary>
    public class PlayerMove
    {
        private const float RotationRight = 0f;
        private const float RotationLeft = 180f;

        private readonly Rigidbody2D _rb;
        private readonly Transform _transform;
        private readonly PlayerAnimationController _anim;

        private readonly float _moveSpeed;
        private readonly Vector3 _initialPosition;

        public Vector2 MoveInput { get; set; }

        public PlayerMove(Rigidbody2D rb, Transform transform, PlayerAnimationController anim, float speed)
        {
            _rb = rb;
            _transform = transform;
            _anim = anim;
            _moveSpeed = speed;
            _initialPosition = transform.position;
        }

        public void ExecutePhysicsUpdate()
        {
            Vector2 targetVelocity = new Vector2(MoveInput.x * _moveSpeed, _rb.linearVelocity.y);
            _rb.linearVelocity = targetVelocity;

            if (MoveInput.x != 0)
            {
                float yRotation = MoveInput.x > 0 ? RotationRight : RotationLeft;
                _transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
        /// <summary> アニメーションを更新します。</summary>
        public void UpdateAnimation()
        {
            if (_anim != null)
                _anim.UpdateMoveAnimation(MoveInput);
        }

        public void ResetPosition()
        {
            _transform.position = _initialPosition;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            MoveInput = Vector2.zero;
        }
    }
}