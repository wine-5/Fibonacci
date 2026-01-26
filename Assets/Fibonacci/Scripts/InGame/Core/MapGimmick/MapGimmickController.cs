using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmickGimmick
{
    public class MapGimmickController : MonoBehaviour
    {
        [Header("Area Settings")]
        [Tooltip("このギミックが所属するエリア番号")]
        public int areaIndex;

        [Header("Visibility Settings")]
        [SerializeField] private AbilityType targetAbility;
        [SerializeField] private bool activeWhenMatched = true;

        private void Update()
        {
            if (AbilityManager.Instance == null) return;

            AbilityType currentAbility = AbilityManager.Instance.GetAbilityAt(areaIndex);

            bool isMatched = (currentAbility == targetAbility);

            bool shouldBeActive = activeWhenMatched ? isMatched : !isMatched;

            SetChildrenActive(shouldBeActive);
        }

        private void SetChildrenActive(bool active)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player")) continue;
                
                if (child.gameObject.activeSelf != active)
                {
                    child.gameObject.SetActive(active);
                }
            }
        }
    }
}