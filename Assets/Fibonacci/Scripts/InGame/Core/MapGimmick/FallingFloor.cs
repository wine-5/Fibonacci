using UnityEngine;
using System.Collections;
using Fibonacci.UI;
 
namespace Fibonacci.InGame.Core.MapGimmick
{
    public class FallingFloor : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 1.0f;  // 落ちるのに必要な重さ
        [SerializeField] private float fallDelay = 0.5f;    // 乗ってから落ちるまでの時間
        [SerializeField] private float destroyDelay = 3.0f; // 落ち始めてから消えるまでの時間
        [SerializeField] private float respawnDelay = 2.0f; // 消えてから復活するまでの時間
 
        [Header("アニメーション")]
        [SerializeField] private FallFloorAnimation floorAnimation = new FallFloorAnimation();
 
        private Rigidbody2D rb;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isFalling = false;
        private Collider2D col;
        private new SpriteRenderer renderer;
 
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            renderer = GetComponent<SpriteRenderer>();
 
            startPosition = transform.position;
            startRotation = transform.rotation;
 
            floorAnimation.Initialize(transform);
 
            SetupInitialState();
        }
 
        private void Update()
        {
            if (!isFalling)
                floorAnimation.UpdateAnimation(transform);
        }
 
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
 
        private IEnumerator FallSequence()
        {
            isFalling = true;
            floorAnimation.Stop(transform);
 
            yield return new WaitForSeconds(fallDelay);
            rb.bodyType = RigidbodyType2D.Dynamic;
 
            yield return new WaitForSeconds(destroyDelay);
            renderer.enabled = false;
            col.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
 
            yield return new WaitForSeconds(respawnDelay);
            SetupInitialState();
        }
    }
}