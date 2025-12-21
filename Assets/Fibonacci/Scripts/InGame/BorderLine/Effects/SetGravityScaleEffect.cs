using UnityEngine;
using Fibonacci.Player;

namespace Fibonacci.InGame.BorderLine.Effects
{
    [CreateAssetMenu(menuName = "Fibonacci/BorderLine/Effects/Set Gravity Scale", fileName = "SetGravityScaleEffect")]
    public sealed class SetGravityScaleEffect : BorderLineEffect
    {
        public override void Apply(PlayerController player, float value)
        {
            if (player == null) return;

            var gravity = player.GetComponent<PlayerGravity>();
            if (gravity == null) return;

            gravity.SetGravityScale(value);
        }

        public override void Clear(PlayerController player)
        {
            if (player == null) return;

            var gravity = player.GetComponent<PlayerGravity>();
            if (gravity == null) return;

            gravity.SetNormalGravity();
        }
    }
}
