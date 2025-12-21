using UnityEngine;

public class RuleUI : MonoBehaviour
{
    [SerializeField] GameObject rulePanel;

    public void ToggleRule()
    {
        // 今の表示状態を反転させる
        bool isActive = rulePanel.activeSelf;
        rulePanel.SetActive(!isActive);
    }
}