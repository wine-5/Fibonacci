using UnityEngine;
using System;

namespace Fibonacci.Event

{
    /// <summary>
    /// リスタート専用のシンプルなイベントシステム
    /// GameJam用の軽量実装
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// Rキーでリスタートが実行された時のイベント
        /// プレイヤー位置リセット、UI更新などに使用
        /// </summary>
        public static event Action OnRestart;

        /// <summary>
        /// 能力（エリア効果）が変更された時に通知するイベント
        /// AbilityManager.SetAreaAbility から発火される
        /// </summary>
        public static event Action OnAbilitiesUpdated;

        /// <summary>
        /// リスタートイベントを発火
        /// </summary>
        public static void TriggerRestart()
        {
            OnRestart?.Invoke();
        }

        public static void TriggerAbilitiesUpdated()
        {
            OnAbilitiesUpdated?.Invoke();
        }
    }
}
