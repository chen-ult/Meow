using UnityEngine;

// ����ű���Ҫ���ص���Rigidbody2D�������Ҷ�����
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("�ƶ��ٶ�")]
    [SerializeField] private float moveSpeed = 5f; // ���л��ֶΣ�������Inspector������

    private Rigidbody2D rb; // ��ɫ��2D�������
    private float horizontalInput; // ˮƽ����ֵ��-1��1֮�䣩

    // ��Ϸ��ʼʱ����
    private void Start()
    {
        // ��ȡ�������������������ظ�����GetComponent��������
        rb = GetComponent<Rigidbody2D>();
    }

    // ÿ֡���ã��������룩
    private void Update()
    {
        // ��ȡˮƽ���룺����A/D����������һ᷵��-1(��)��0(��)��1(��)
        horizontalInput = Input.GetAxis("Horizontal");

        // ��ѡ���ý�ɫ�����ƶ�����
        FlipCharacter();
    }

    // �̶�ʱ�䲽���ã�����������
    private void FixedUpdate()
    {
        // �����ƶ��ٶȣ�ֻ�ı�x���ٶȣ�y�ᱣ��ԭ���ٶȣ���Ӱ����Ծ�ȣ�
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Ӧ���ƶ��ٶȵ�����
        rb.linearVelocity = movement;
    }

    // ��ɫ���ҷ�ת
    private void FlipCharacter()
    {
        // �����ˮƽ���������뷽���뵱ǰ���ŷ���һ��ʱ��ת
        if (horizontalInput != 0)
        {
            // ͨ���޸ı������ŵ�xֵ����ת��ɫ
            transform.localScale = new Vector3(
                Mathf.Sign(horizontalInput), // 1Ϊ�ң�-1Ϊ��
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}