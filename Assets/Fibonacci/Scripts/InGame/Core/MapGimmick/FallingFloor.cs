using UnityEngine;
using System.Collections;
using Fibonacci.UI;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// 一定以上の質量を持つオブジェクトが接触した際、時間差で落下し、その後復活する床。
    /// 落下シーケンス、透明化、および再配置のサイクルを管理します。
    /// </summary>
    public class FallingFloor : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 1.0f;
        [SerializeField] private float fallDelay = 0.5f;
        [SerializeField] private float destroyDelay = 3.0f;
        [SerializeField] private float respawnDelay = 2.0f;

        [Header("アニメーション")]
        [SerializeField] private FallFloorAnimation floorAnimation = new FallFloorAnimation();

        private Rigidbody2D rb;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isFalling = false;
        private Collider2D col;
        private new SpriteRenderer renderer;

        private WaitForSeconds cachedFallDelay;
        private WaitForSeconds cachedDestroyDelay;
        private WaitForSeconds cachedRespawnDelay;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            renderer = GetComponent<SpriteRenderer>();

            startPosition = transform.position;
            startRotation = transform.rotation;

            cachedFallDelay = new WaitForSeconds(fallDelay);
            cachedDestroyDelay = new WaitForSeconds(destroyDelay);
            cachedRespawnDelay = new WaitForSeconds(respawnDelay);

            floorAnimation.Initialize(transform);

            SetupInitialState();
        }

        private void Update()
        {
            if (isFalling) return;
            
            floorAnimation.UpdateAnimation(transform);
        }

        /// <summary>
        /// 床を初期状態にリセットし、物理挙動と視覚情報を有効化します。
        /// </summary>
        private void SetupInitialState()
        {
            isFalling = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = startPosition;
            transform.rotation = startRotation;

            renderer.enabled = true;
            col.enabled = true;

            floorAnimation.Initialize(transform);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isFalling) return;

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.mass >= requiredMass)
            {
                StartCoroutine(FallSequence());
            }
        }

        /// <summary>
        /// 落下から消滅、そして復活までのフローを制御します。
        /// </summary>
        private IEnumerator FallSequence()
        {
            isFalling = true;
            floorAnimation.Stop(transform);

            yield return cachedFallDelay;
            rb.bodyType = RigidbodyType2D.Dynamic;

            yield return cachedDestroyDelay;
            renderer.enabled = false;
            col.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;

            yield return cachedRespawnDelay;
            SetupInitialState();
        }
    }
}