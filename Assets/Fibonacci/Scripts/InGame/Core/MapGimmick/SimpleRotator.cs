using UnityEngine;
namespace Fibonacci.InGame.Core.MapGimmick
{
    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 100f;

        void Update()
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}