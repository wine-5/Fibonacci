using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class PowerUpAbility
    {
        private const float POWER_UP_MASS = 2.0f;

        public void Apply(Rigidbody2D rb, bool isPowerUp)
        {
            if (isPowerUp)
            {
                rb.mass = POWER_UP_MASS;
            }
        }
    }
}