using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class PowerUpAbility
    {
        private const float NORMAL_MASS = 1.0f;
        private const float POWER_UP_MASS = 2.0f; // PushBlock（2.0）を押せる重さに設定

        public void Apply(Rigidbody2D rb, bool isPowerUp)
        {
            // HeavyAbilityと同様に三項演算子でマスの切り替えのみを行う
            rb.mass = isPowerUp ? POWER_UP_MASS : NORMAL_MASS;
        }
    }
}