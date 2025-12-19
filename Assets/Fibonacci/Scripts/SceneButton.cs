using UnityEngine;
using Fibonacci.Scene;

/// <summary>
/// シーン遷移ボタンのラッパークラス
/// SceneControllerが他のシーンにない場合でも、enumを使ってシーン遷移が可能
/// </summary>
public class SceneButton : MonoBehaviour
{
    [Header("シーン遷移設定")]
    [SerializeField] private SceneName targetScene = SceneName.Title;
    
    /// <summary>
    /// ボタンクリック時に呼び出される
    /// インスペクターで指定されたシーンに遷移
    /// </summary>
    public void OnClick()
    {
        if (SceneController.Instance != null)
        {
            Debug.Log($"Scene transition button clicked: {targetScene}");
            SceneController.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("SceneController instance not found! Make sure it's attached in Title scene.");
        }
    }
    
    /// <summary>
    /// プログラムからシーンを指定して遷移
    /// </summary>
    /// <param name="sceneName">遷移先シーン</param>
    public void LoadScene(SceneName sceneName)
    {
        if (SceneController.Instance != null)
        {
            Debug.Log($"Programmatic scene transition: {sceneName}");
            SceneController.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneController instance not found! Make sure it's attached in Title scene.");
        }
    }
}
