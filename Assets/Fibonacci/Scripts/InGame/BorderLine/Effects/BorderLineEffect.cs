using UnityEngine;
using Fibonacci.Player;

namespace Fibonacci.InGame.BorderLine.Effects
{
    public abstract class BorderLineEffect : ScriptableObject
    {
        public abstract void Apply(PlayerController player, float value);

        public virtual void Clear(PlayerController player)
        {
        }
    }
}
