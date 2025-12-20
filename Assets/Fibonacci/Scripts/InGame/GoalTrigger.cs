using Fibonacci.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーがゴールに到達した時の判定とシーン遷移を管理するクラス
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [Header("ゴールの設定")]
    [SerializeField] private string targetSceneName = "Title";

    /// <summary>
    /// プレイヤーがゴールに到達した時の判定とシーン遷移を管理するクラス
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Goal Trigger 2D activated");
        // Playerタグのオブジェクトと接触したかチェック
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal reached! Loading Result scene...");
            LoadResultScene();
        }
    }

    private void LoadResultScene()
    {
        // SceneControllerのインスタンスを経由してリザルト画面に移動
        if (SceneController.Instance != null)
            SceneController.Instance.LoadScene(SceneName.Result);
    }
}
