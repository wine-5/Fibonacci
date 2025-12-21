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
        /// リスタートイベントを発火
        /// </summary>
        public static void TriggerRestart()
        {
            OnRestart?.Invoke();
        }
    }
}
