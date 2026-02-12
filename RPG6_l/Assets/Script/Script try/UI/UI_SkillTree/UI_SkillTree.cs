using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoint;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    public Player_SkillManager skillManager {  get; private set; }


    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }


    private void Start()
    {
        UpdateAllConnections();
    }

    [ContextMenu("重置技能树")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            node.Refund();
        }
    }

    public bool EnoughSkillPoints(int cost) => skillPoint >= cost;
    public void RemoveSkillPoints(int cost) => skillPoint -= cost;
    public void AddSkillPoints(int points) => skillPoint += points;

    [ContextMenu("更新所有关系")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConections();
        }
    }
}
