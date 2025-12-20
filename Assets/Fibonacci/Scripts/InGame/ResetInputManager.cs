using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Scene;

namespace Fibonacci
{
    /// <summary>
    /// ステージリセット機能を管理するクラス
    /// Rキーでのステージ再開処理を担当します
    /// </summary>
    public class ResetInputManager : MonoBehaviour
    {
        [Header("入力設定")]
        [SerializeField] private InputActionReference restartAction;
        
        [Header("デバッグ設定")]
        [SerializeField] private bool enableDebugLog = true;

        private void OnEnable()
        {
            // Restart入力アクションを有効化
            if (restartAction != null)
            {
                restartAction.action.Enable();
                restartAction.action.performed += OnRestartPerformed;
                
                if (enableDebugLog)
                {
                    Debug.Log("ResetInputManager: Restart input enabled");
                }
            }
            else
            {
                Debug.LogWarning("ResetInputManager: Restart action reference is not assigned!");
            }
        }

        private void OnDisable()
        {
            // Restart入力アクションを無効化
            if (restartAction != null)
            {
                restartAction.action.performed -= OnRestartPerformed;
                restartAction.action.Disable();
                
                if (enableDebugLog)
                {
                    Debug.Log("ResetInputManager: Restart input disabled");
                }
            }
        }

        /// <summary>
        /// Rキーが押された時の処理
        /// </summary>
        /// <param name="context">入力コンテキスト</param>
        private void OnRestartPerformed(InputAction.CallbackContext context)
        {
            if (enableDebugLog)
            {
                Debug.Log("ResetInputManager: Restart key pressed");
            }
            
            RestartCurrentStage();
        }

        /// <summary>
        /// 現在のステージを再開
        /// </summary>
        private void RestartCurrentStage()
        {
            // SceneControllerのインスタンスが存在するかチェック
            if (SceneController.Instance == null)
            {
                Debug.LogError("ResetInputManager: SceneController instance not found!");
                return;
            }

            // ゲームステージでのみリスタート可能
            if (SceneController.Instance.IsGameStage())
            {
                if (enableDebugLog)
                {
                    Debug.Log($"ResetInputManager: Restarting stage - {SceneController.Instance.CurrentStage}");
                }
                
                SceneController.Instance.RestartCurrentStage();
            }
            else
            {
                if (enableDebugLog)
                {
                    Debug.LogWarning($"ResetInputManager: Cannot restart - Current scene is not a game stage ({SceneController.Instance.CurrentStage})");
                }
            }
        }

        /// <summary>
        /// 手動でリスタートを実行（デバッグ用）
        /// </summary>
        [ContextMenu("Manual Restart")]
        public void ManualRestart()
        {
            RestartCurrentStage();
        }
    }
}
