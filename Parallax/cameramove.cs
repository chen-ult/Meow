using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameramove : MonoBehaviour
{
    public Transform target;  //玩家位置
    public Transform farbackground, middlebackground;  //远景中景位置
    private Vector2 lastPos; //最后一次相机位置
    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //记录相机初始位置
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        Vector2 amountTOmove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

        farbackground.position += new Vector3(amountTOmove.x, amountTOmove.y, 0f);

        middlebackground.position += new Vector3(amountTOmove.x * 0.5f, amountTOmove.y * 0.5f, 0f);

        lastPos = transform.position;
    }
}
