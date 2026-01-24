using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class MoveLockAbility
    {
        public void Apply(bool isLocked, UnityEngine.SpriteRenderer displayRenderer)
        {
            AbilityManager.Instance.SetGimmicksActive(isLocked);


            if (isLocked)
            {
                Sprite targetSprite = AbilityManager.Instance.GetAbilitySprite(AbilityType.MoveLock);

                displayRenderer.sprite = targetSprite;
                displayRenderer.enabled = true; 
            }
            else
            {
                displayRenderer.enabled = false;
            }
        }
    }
}