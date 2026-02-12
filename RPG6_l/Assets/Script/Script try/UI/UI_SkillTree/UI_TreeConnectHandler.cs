using UnityEngine;
using System;
using UnityEngine.UI;


[Serializable]
public class UI_TreeConnectDetails
{

    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-25f, 25f)] public float rotation;
}

public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionImage;
    private Color originalColor;

    private void Awake()
    {
        if (connectionImage != null)
            originalColor = connectionImage.color;
    }

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
            return;

        if (connectionDetails.Length != connections.Length)
        {
            Debug.Log("长度不一样啊");
            return;
        }

        UpdateConnection();
    }


    public void UpdateConnection()
    {
        for(int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionImage = connection.GetConnectionImage();

            connection.DirectConnection(detail.direction, detail.length ,detail.rotation);

            if (detail.childNode == null)
                continue;

            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();
        }
    }

    public void UpdateAllConections()
    {
        UpdateConnection();

        foreach(var node in connectionDetails)
        {
            if(node.childNode == null)
                continue;

            node.childNode.UpdateConnection();
        }

    }


    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;

        connectionImage.color = unlocked ? Color.white : originalColor;
    }


    public void SetConnectionImage(Image image) => connectionImage = image;
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
