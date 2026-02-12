using UnityEngine;

public class Object_PickUpToAddMaxHealth : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("增加参数")]
    [SerializeField] private float healthIncreaseAmount = 20f;

    [Header("动态参数")]
    [SerializeField] private float floatSpeed = 1f;//浮动速度
    [SerializeField] private float floatRange = .1f;//浮动范围
    private Vector3 startPosition;//起始位置


    private void Awake()
    {
        startPosition = transform.position;
        sr = GetComponentInChildren<SpriteRenderer>();
    }


    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)//触发碰撞
    {
        // Try to find an Entity_Stats on the collider or its parents and apply increase there
        var stats = collision.GetComponentInParent<Entity_Stats>();
        if (stats == null)
            return;

        stats.IncreaseMaxHealth(healthIncreaseAmount);
        Destroy(gameObject);
    }
}
