using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class LowGravityAbility
    {
        private const float NORMAL_DRAG = 0f;
        private const float LOW_GRAVITY_DRAG = 5.0f;

        public void Apply(Rigidbody2D rb, bool isLow)
        {
            rb.linearDamping = isLow ? LOW_GRAVITY_DRAG : NORMAL_DRAG;
        }
    }
}