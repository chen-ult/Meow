using System.Collections;
using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private bool autoDestroy = true;//自动销毁
    [SerializeField] private float destroyDelay = 1;//销毁延迟时间
    [Space]
    [SerializeField] private bool randomOffset = true;//随机位置偏移
    [SerializeField] private bool randomRotation = true;//随机旋转

    [Header("Fade效果")]
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeSpeed = 1;

    [Header("随机旋转参数")]
    [SerializeField] private float minRotation = 0;//最小旋转角度
    [SerializeField] private float maxRotation = 360;//最大旋转角度

    [Header("随机位置参数")]
    [SerializeField] private float xMinOffset = -.3f;//最小X偏移
    [SerializeField] private float xMaxOffset = .3f;//最大X偏移
    [Space]
    [SerializeField] private float yMinOffset = -.3f;//最小Y偏移
    [SerializeField] private float yMaxOffset = .3f;//最大Y偏移

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }


    private void Start()
    {
        if (canFade)
            StartCoroutine(FadeCo());

        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);
    }

    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;

        while(targetColor.a > 0)
        {
            targetColor.a = targetColor.a - (fadeSpeed * Time.deltaTime);
            sr.color = targetColor;
            yield return null;
        }

        sr.color = targetColor;
    }

    private void ApplyRandomOffset()//应用随机位置偏移
    {
        if (!randomOffset)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position += new Vector3(xOffset, yOffset);
    }

    private void ApplyRandomRotation()//应用随机旋转
    {
        if (!randomRotation)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0,0, zRotation);
    }
}
