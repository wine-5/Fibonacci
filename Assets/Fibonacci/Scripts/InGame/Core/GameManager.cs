using Fibonacci.Event;
using UnityEngine;

namespace Fibonacci.InGame.Core
{
    public enum GamePhase
    {
        Drawing, // 線を引いている最中（これだけ発動）
        Playing  // ゲームプレイ中（プレイヤーが動き、判定が始まる）
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
            base.Awake(); // シングルトンの初期化

            // 新しいシーン（同じステージの再来含む）が始まったら、
            // 前回のエリア効果をクリアする
            if (AbilityManager.Instance != null)
            {
                AbilityManager.Instance.Reset();
            }
        }
        // 線を引き終わり、色塗りと効果選択が終わったらこれを呼ぶ
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