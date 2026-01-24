using UnityEngine;
 
namespace Fibonacci.UI
{
    [System.Serializable]
    public class FallFloorAnimation
    {
        [SerializeField] private float shakeFrequency = 8f;
        [SerializeField] private float rotationAmount = 0.5f;
 
        private const float TILT_FREQUENCY_MULTIPLIER = 0.5f;
        private const float TILT_AMOUNT_MULTIPLIER = 0.5f;
        private const float WOBBLE_FREQUENCY_MULTIPLIER = 2f;
        private const float WOBBLE_AMOUNT_MULTIPLIER = 0.3f;
 
        private Quaternion initialRotation;
        private float time;
        private bool isActive;
 
        public void Initialize(Transform target)
        {
            initialRotation = target.localRotation;
            time = 0f;
            isActive = true;
        }
 
        public void UpdateAnimation(Transform target)
        {
            if (!isActive) return;
 
            time += Time.deltaTime * shakeFrequency;
            float tilt = Mathf.PerlinNoise(time * TILT_FREQUENCY_MULTIPLIER, 0f) * rotationAmount - (rotationAmount * TILT_AMOUNT_MULTIPLIER);
            float wobble = Mathf.Sin(time * WOBBLE_FREQUENCY_MULTIPLIER) * (rotationAmount * WOBBLE_AMOUNT_MULTIPLIER);
            target.localRotation = initialRotation * Quaternion.Euler(0f, 0f, tilt + wobble);
        }
 
        public void Stop(Transform target)
        {
            isActive = false;
            target.localRotation = initialRotation;
        }
    }
}
 