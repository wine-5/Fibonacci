using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class MoveLockAbility
    {
        private const float NormalDamping = 1.0f;
        private const float SlipperyDamping = 0.05f;

        public void Apply(Rigidbody2D rb, bool isLocked, int areaIndex, SpriteRenderer displayRenderer)
        {

            if (isLocked)
            {
                rb.linearDamping = SlipperyDamping;
            }
            else
            {
                rb.linearDamping = NormalDamping;
            }
        }
    }
}