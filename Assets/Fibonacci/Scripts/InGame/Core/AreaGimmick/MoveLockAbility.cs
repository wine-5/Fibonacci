using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class MoveLockAbility
    {
        private const float NormalDamping = 1.0f;
        private const float SlipperyDamping = 0f;

        public void Apply(Rigidbody2D rb, bool isLocked, int areaIndex, SpriteRenderer displayRenderer)
        {
            AbilityManager.Instance.SetGimmicksActive(areaIndex, isLocked);

            if (isLocked)
            {
                rb.linearDamping = SlipperyDamping;

                Sprite targetSprite = AbilityManager.Instance.GetAbilitySprite(AbilityType.MoveLock);
                displayRenderer.sprite = targetSprite;
                displayRenderer.enabled = true;
            }
            else
            {
                rb.linearDamping = NormalDamping;
                displayRenderer.enabled = false;
            }
        }
    }
}