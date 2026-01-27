using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class JumpAbility
    {
        public bool CanJump { get; private set; }

        public void Apply(bool isActive)
        {
            CanJump = isActive;
        }
    }
}