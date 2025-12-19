using UnityEngine;

namespace Fibonacci
{
    /// <summary>
    /// アプリケーション全体の管理を行うSingletonクラス
    /// アプリケーションの終了処理などを担当
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>
        /// アプリケーションを終了
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
