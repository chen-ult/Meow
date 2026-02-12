using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private bool onlyAffectPlayer = true;
    [SerializeField] private string targetTag = "Player";

    private readonly Dictionary<GameObject, Coroutine> activeTargets = new Dictionary<GameObject, Coroutine>();

    private void Reset()
    {
        // Keep collider configuration to designer; do not force trigger to allow standing on trap
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsValidTarget(collision))
            return;

        // find the GameObject that owns the IDamagable component if available
        var damComp = collision.GetComponentInParent<IDamagable>();
        GameObject go;
        if (damComp != null)
            go = ((Component)damComp).gameObject;
        else if (collision.attachedRigidbody != null)
            go = collision.attachedRigidbody.gameObject;
        else
            go = collision.gameObject;

        if (activeTargets.ContainsKey(go))
            return;

        var routine = StartCoroutine(DoDamageOverTime(go));
        activeTargets.Add(go, routine);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var damComp = collision.GetComponentInParent<IDamagable>();
        GameObject go;
        if (damComp != null)
            go = ((Component)damComp).gameObject;
        else if (collision.attachedRigidbody != null)
            go = collision.attachedRigidbody.gameObject;
        else
            go = collision.gameObject;

        if (activeTargets.TryGetValue(go, out var routine))
        {
            StopCoroutine(routine);
            activeTargets.Remove(go);
        }
    }

    private bool IsValidTarget(Collider2D collision)
    {
        if (onlyAffectPlayer && !collision.CompareTag(targetTag))
            return false;

        var dam = collision.GetComponentInParent<IDamagable>();
        return dam != null;
    }

    private IEnumerator DoDamageOverTime(GameObject targetObj)
    {
        var damagable = targetObj.GetComponentInParent<IDamagable>();
        if (damagable == null)
            yield break;

        while (true)
        {
            // Call TakeDamage so mitigation/knockback triggers
            damagable.TakeDamage(damagePerTick, transform, 0f, ElementType.None);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<Collider2D>();
        if (col == null)
            return;

        Gizmos.color = Color.red;
        if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius * Mathf.Max(transform.localScale.x, transform.localScale.y));
        }
        else if (col is BoxCollider2D box)
        {
            Vector3 size = new Vector3(box.size.x * transform.localScale.x, box.size.y * transform.localScale.y, 0f);
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, size);
        }
        else
        {
            // fallback: draw bounds
            var bounds = col.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
