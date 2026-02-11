using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;//±³¾°±ä»»
    [SerializeField] private float parallaxMultiplier;//ÊÓ²î±¶ÔöÆ÷
    [SerializeField] private float imageWidthOffset = 10;//Í¼Ïñ¿í¶ÈÆ«ÒÆÁ¿

    private float imageFullWidth;//Í¼ÏñÍêÕû¿í¶È
    private float imageHalfWidth;//Í¼Ïñ°ë¿í¶È

    public void CalculateImageWidth()//¼ÆËãÍ¼Ïñ¿í¶È
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float distanceToMove)//ÒÆ¶¯±³¾°
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    public void LoopBackground(float cameraLeftEdge,float cameraRightEdge)//Ñ­»·±³¾°
    {
        float imageRightEdge = (background.position.x + imageHalfWidth) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - imageHalfWidth) + imageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * imageFullWidth;
        else if (imageLeftEdge > cameraRightEdge)
            background.position += Vector3.right * -imageFullWidth;
    }
}
