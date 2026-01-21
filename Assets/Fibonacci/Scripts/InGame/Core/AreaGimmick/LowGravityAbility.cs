using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class LowGravityAbility
    {
        private const float NORMAL_GRAVITY_SCALE = 1.0f;
        private const float LOW_GRAVITY_SCALE = 0.2f;   
        
        private const float NORMAL_DRAG = 0f;
        private const float LOW_GRAVITY_DRAG = 2.0f;    

        public void Apply(Rigidbody2D rb, bool isLow)
        {
            rb.gravityScale = isLow ? LOW_GRAVITY_SCALE : NORMAL_GRAVITY_SCALE;

            rb.linearDamping = isLow ? LOW_GRAVITY_DRAG : NORMAL_DRAG;
        }
    }
}