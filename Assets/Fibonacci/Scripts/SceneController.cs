using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fibonacci.Scene
{
    /// <summary>
    /// ゲーム内のシーン名を定義するenum
    /// </summary>
    public enum SceneName
    {
        Title,
        StageSelect,
        InGame,
        Result
    }

    /// <summary>
    /// シーン遷移を管理するSingletonクラス
    /// Titleシーンで一度生成されれば、他のシーンでも利用可能
    /// </summary>
    public class SceneController : Singleton<SceneController>
    {
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>
        /// enumで指定されたシーンに切り替え
        /// </summary>
        /// <param name="sceneName">遷移先のシーン</param>
        public void LoadScene(SceneName sceneName)
        {
            string sceneNameStr = sceneName.ToString();
            SceneManager.LoadScene(sceneNameStr);
        }

        /// <summary>
        /// 現在のシーンをリロード
        /// </summary>
        public void ReloadCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
