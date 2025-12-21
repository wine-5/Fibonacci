using UnityEngine;

namespace Fibonacci.Player
{
    /// <summary>
    /// プレイヤーのアニメーション制御を管理するクラス
    /// アニメーションの切り替えや状態管理を担当
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {  
        //TODO:Animationの制御はまだ書いていない
        private Animator animator;      
        void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }
    }
}
