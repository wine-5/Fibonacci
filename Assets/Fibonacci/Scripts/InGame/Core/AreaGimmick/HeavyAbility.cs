using UnityEngine;
using Fibonacci.InGame.Player;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class HeavyAbility
    {
        private const float NORMAL_MASS = 1.0f;
        private const float HEAVY_MASS = 5.0f;
        private const float NORMAL_SPEED = 5.0f; 
        private const float SLOW_SPEED = 2.0f;  

        public void Apply(Rigidbody2D rb, PlayerMove playerMove, bool isHeavy)
        {
            rb.mass = isHeavy ? HEAVY_MASS : NORMAL_MASS;

            float targetSpeed = isHeavy ? SLOW_SPEED : NORMAL_SPEED;
            playerMove.SetCurrentSpeed(targetSpeed);
        }
    }
}