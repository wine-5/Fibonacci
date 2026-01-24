using UnityEngine;

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class MoveLockAbility
    {
        private const float NormalDamping = 1.0f;
        private const float SlipperyDamping = 0.05f;

        public void Apply(Rigidbody2D rb, bool isLocked, int areaIndex, SpriteRenderer displayRenderer)
        {
            AbilityManager.Instance.SetGimmicksActive(areaIndex, isLocked);

            if (isLocked)
            {
                rb.linearDamping = SlipperyDamping;

                displayRenderer.sprite = AbilityManager.Instance.GetAbilitySprite(AbilityType.MoveLock);
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