using UnityEngine;
using Fibonacci.InGame.Player;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class MoveLockAbility
    {
        /// <summary>
        /// 移動ロック状態を適用します。
        /// </summary>
        public void Apply(PlayerController controller, bool isLocked)
        {
            if (isLocked)
            {
                // 移動ロック：演出ON
            }
            else
            {
                // 移動ロック：演出OFF
            }
        }
    }
}