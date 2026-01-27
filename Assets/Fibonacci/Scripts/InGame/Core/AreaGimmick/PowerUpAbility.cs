using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class PowerUpAbility
    {
        private const float NORMAL_MASS = 1.0f;
        private const float POWER_UP_MASS = 2.0f; 

        public void Apply(Rigidbody2D rb, bool isPowerUp)
        {
            rb.mass = isPowerUp ? POWER_UP_MASS : NORMAL_MASS;
        }
    }
}