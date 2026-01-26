using UnityEngine;
using Fibonacci.InGame.BorderLine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    public class AbilityFrameHighlighter : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;

        [Header("Ability Colors")]
        [SerializeField] private Color fireColor = Color.red;
        [SerializeField] private Color MoveLockColor = Color.blue;
        [SerializeField] private Color defaultColor = Color.gray;

        private AbilityType lastType0 = (AbilityType)(-1);
        private AbilityType lastType1 = (AbilityType)(-1);
        private bool isFirstUpdate = true;

        void Update()
        {
            if (AbilityManager.Instance == null || drawBorderLine == null) return;

            AbilityType type0 = AbilityManager.Instance.GetAbilityAt(0);
            AbilityType type1 = AbilityManager.Instance.GetAbilityAt(1);

            bool isPartitionDirty = false;
            if (drawBorderLine.TryGetCurrentPartition(out _))
            {
                isPartitionDirty = isFirstUpdate;
            }

            if (type0 == lastType0 && type1 == lastType1 && !isPartitionDirty) return;

            lastType0 = type0;
            lastType1 = type1;
            isFirstUpdate = false;

            Color color0 = GetColorForType(type0);
            Color color1 = GetColorForType(type1);

            drawBorderLine.UpdateFrameColors(color0, color1);
        }

        private Color GetColorForType(AbilityType type)
        {
            return type switch
            {
                AbilityType.Fire => fireColor,
                AbilityType.MoveLock => MoveLockColor,
                _ => defaultColor
            };
        }
    }
}