using Fibonacci.Event;

namespace Fibonacci.InGame.Core
{
    public enum GamePhase
    {
        Drawing, 
        Playing 
    }

    /// <summary>
    /// ゲーム全体の進行フェーズを管理するマネジャークラス。
    /// Singleton基底クラスを継承することで、どこからでも GameManager.Instance でアクセス可能です。
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        protected override bool UseDontDestroyOnLoad => true;
        public GamePhase CurrentPhase { get; private set; } = GamePhase.Drawing;


        protected override void Awake()
        {
            base.Awake(); 

            if (AbilityManager.Instance != null)
            {
                AbilityManager.Instance.ResetAbilities();
            }
        }
        public void StartGame()
        {
            CurrentPhase = GamePhase.Playing;
            GameEvents.TriggerPhaseChanged(GamePhase.Playing);
        }

        /// <summary>
        /// ゲーム状態を初期状態（Drawing）にリセットします。
        /// </summary>
        public void ResetPhase()
        {
            CurrentPhase = GamePhase.Drawing;
            GameEvents.TriggerPhaseChanged(GamePhase.Drawing);
        }
    }
}