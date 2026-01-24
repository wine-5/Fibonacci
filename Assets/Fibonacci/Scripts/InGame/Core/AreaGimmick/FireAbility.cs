using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class FireAbility
    {
        public void Apply(bool isActive, int areaIndex, SpriteRenderer displayRenderer)
        {
            AbilityManager.Instance.SetGimmicksHidden(areaIndex, isActive);

            if (isActive)
            {
                displayRenderer.sprite = AbilityManager.Instance.GetAbilitySprite(AbilityType.Fire);
                displayRenderer.enabled = true;
            }
            else
            {
                displayRenderer.enabled = false;
            }
        }
    }
}