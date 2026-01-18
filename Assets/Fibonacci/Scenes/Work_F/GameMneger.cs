using UnityEngine;

namespace Fibonacci.InGame
{
    public enum GamePhase
    {
        Drawing, // 線を引いている最中（これだけ発動）
        Playing  // ゲームプレイ中（プレイヤーが動き、判定が始まる）
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GamePhase CurrentPhase { get; private set; } = GamePhase.Drawing;

        void Awake()
        {
            Instance = this;
        }

        // 線を引き終わり、色塗りと効果選択が終わったらこれを呼ぶ
        public void StartGame()
        {
            CurrentPhase = GamePhase.Playing;
            Fibonacci.Event.GameEvents.TriggerPhaseChanged(GamePhase.Playing);
        }

        /// <summary>
        /// ゲーム状態を初期状態（Drawing）にリセットします。
        /// </summary>
        public void ResetPhase()
        {
            CurrentPhase = GamePhase.Drawing;
            Fibonacci.Event.GameEvents.TriggerPhaseChanged(GamePhase.Drawing);
        }
    }
}