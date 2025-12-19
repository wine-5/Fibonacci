using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーがゴールに到達した時の判定とシーン遷移を管理するクラス
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [Header("ゴールの設定")]
    [SerializeField] private string targetSceneName = "Title";
    
    // TODO: Playerの当たり判定が2Dになったらここも2Dに変更する
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("A");
        // Playerタグのオブジェクトと接触したかチェック
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal reached! Loading Title scene...");
            LoadTargetScene();
        }
    }
    
    private void LoadTargetScene()
    {
        // Unity標準のSceneManagerを使用してシーン切り替え
        SceneManager.LoadScene(targetSceneName);
    }
}
