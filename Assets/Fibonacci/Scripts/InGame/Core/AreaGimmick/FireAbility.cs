using UnityEngine;
using Fibonacci.Event; 

namespace Fibonacci.InGame.Core.AreaGimmick
{
    public class FireAbility
    {
        private float timer = 0f;
        private const float LimitTime = 5f;

        public void Apply(bool isActive, int areaIndex, SpriteRenderer displayRenderer)
        {

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

        public void Tick(int currentAreaIndex)
        {
            if (currentAreaIndex == -1)
            {
                ResetTimer();
                return;
            }

            AbilityType currentAbility = AbilityManager.Instance.GetAbilityAt(currentAreaIndex);
            bool isFiring = currentAbility == AbilityType.Fire;

            if (isFiring)
            {
                timer += Time.fixedDeltaTime;
                
                if (timer >= LimitTime)
                {
                    timer = 0f;
                    
                    GameEvents.TriggerRestart();
                }
            }
            else
            {
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            timer = 0f;
        }
    }
}