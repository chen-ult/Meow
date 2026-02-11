    
using System.Collections;
using UnityEngine;

public class Firestorm : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 4f;
    public Vector2 moveDir = Vector2.right;
    public float lifetime = 6f;

    [Header("Animation")]
    public bool hasSpawnAnim = true;
    public float spawnAnimDuration = 0.25f;
    private Animator anim;
    private bool isFlying = false;
    private bool isDying = false;

    [Header("Vanish")]
    public bool hasVanishAnim = true;
    public float vanishAnimDuration = 0.35f;

    [Header("Damage Over Time")]
    public float damagePerTick = 2f;
    public float tickInterval = 0.5f;
    public ElementType element = ElementType.Fire;

    private float lifeTimer;
    private Coroutine currentDot;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        lifeTimer = 0f;
    }

    private void Start()
    {
        if (hasSpawnAnim && anim != null)
        {
            anim.SetTrigger("Spawn");
            Invoke(nameof(StartFly), spawnAnimDuration);
        }
        else
        {
            StartFly();
        }
    }

    private void StartFly()
    {
        isFlying = true;
    }

    private void Update()
    {
        if (!isFlying) return;

        transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            PlayDestroyAnim();
    }

    // allow spawner to set direction
    public void SetMoveDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
        moveDir = dir.normalized;
    }

    public void SetMoveDirection(int facingDir)
    {
        SetMoveDirection(new Vector2(facingDir, 0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Player"))
        {
            IDamagable dam = other.GetComponent<IDamagable>() ?? other.GetComponentInParent<IDamagable>();
            if (dam != null)
            {
                // start DOT on player while inside
                if (currentDot != null)
                    StopCoroutine(currentDot);
                currentDot = StartCoroutine(DoDamageOverTime(dam));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        if (other.CompareTag("Player"))
        {
            if (currentDot != null)
            {
                StopCoroutine(currentDot);
                currentDot = null;
            }
        }
    }

    private IEnumerator DoDamageOverTime(IDamagable target)
    {
        while (target != null)
        {
            target.TakeDamage(damagePerTick, transform, 0f, element);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    private void PlayDestroyAnim()
    {
        if (isDying) return;
        isDying = true;

        // stop movement
        isFlying = false;

        // prefer vanish animation if configured
        if (hasVanishAnim && anim != null)
        {
            anim.SetTrigger("Vanish");
            Destroy(gameObject, vanishAnimDuration);
            return;
        }

        // fallback to hit animation or immediate destroy
        if (anim != null && anim.HasState(0, Animator.StringToHash("Hit")))
        {
            anim.SetTrigger("Hit");
            Destroy(gameObject, 0.2f);
            return;
        }

        Destroy(gameObject);
    }
}
