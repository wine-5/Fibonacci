using Fibonacci.Scene;
using UnityEngine;

/// <summary>
/// プレイヤーがゴールに到達した時の判定とシーン遷移を管理するクラス
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    /// <summary>
    /// プレイヤーがゴールに到達した時の判定とシーン遷移を管理するクラス
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            LoadResultScene();
    }

    private void LoadResultScene()
    {
        // SceneControllerのインスタンスを経由してリザルト画面に移動
        if (SceneController.Instance != null)
            SceneController.Instance.LoadScene(SceneName.Clear);
    }
}
