using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// オブジェクトを指定された速度で Z 軸中心に回転させるシンプルなコンポーネント。
    /// 回転速度は度/秒で指定します。
    /// </summary>
    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 100f;

        private static readonly Vector3 ROTATION_AXIS = Vector3.forward;

        /// <summary>
        /// キャッシュされた回転軸を使用してオブジェクトを回転させます。
        /// </summary>
        private void Update()
        {
            transform.Rotate(ROTATION_AXIS * (rotationSpeed * Time.deltaTime));
        }
    }
}