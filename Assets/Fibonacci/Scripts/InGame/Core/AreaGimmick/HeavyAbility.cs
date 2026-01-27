using UnityEngine;
using Fibonacci.InGame.Player;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class HeavyAbility
    {
        private const float HEAVY_MASS = 5.0f;
        private const float SLOW_SPEED = 2.0f;

        public void Apply(Rigidbody2D rb, PlayerMove playerMove, bool isHeavy)
        {
            if (isHeavy)
            {
                rb.mass = HEAVY_MASS;
                playerMove.SetCurrentSpeed(SLOW_SPEED);
            }
        }
    }
}