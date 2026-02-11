using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private KeyCode saveKey = KeyCode.E;
    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private Transform promptParent;
    [SerializeField] private Vector3 promptLocalOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private TextMeshProUGUI savedText;
    [SerializeField] private float savedMessageDuration = 1.5f;
    [SerializeField] private string savePointId = "SavePoint";
    [SerializeField] private string savePointDisplayName = "Save Point";

    private Player playerInRange;
    private Coroutine savedMessageCo;
    private GameObject promptInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = collision.GetComponentInParent<Player>();
        if (playerInRange == null)
            return;

        if (promptPrefab != null && promptInstance == null)
        {
            var parent = promptParent != null ? promptParent : transform;
            promptInstance = Instantiate(promptPrefab, parent);
            promptInstance.transform.localPosition = promptLocalOffset;
        }

        if (promptInstance != null)
            promptInstance.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerInRange == null)
            return;

        if (collision.GetComponentInParent<Player>() != playerInRange)
            return;

        playerInRange = null;

        if (promptInstance != null)
            promptInstance.SetActive(false);
        if (savedText != null)
            savedText.gameObject.SetActive(false);

        if (savedMessageCo != null)
        {
            StopCoroutine(savedMessageCo);
            savedMessageCo = null;
        }
    }

    private void Update()
    {
        if (playerInRange == null)
            return;

        if (Input.GetKeyDown(saveKey))
            Save();
    }

    private void Save()
    {
        if (playerInRange == null)
            return;

        playerInRange.health?.SetHealthToPercent(1f);

        var manager = SaveManager.Instance ?? FindFirstObjectByType<SaveManager>();
        if (manager != null)
            manager.RegisterSavePoint(savePointId, transform.position, savePointDisplayName, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        manager?.Save(playerInRange);

        if (savedText != null)
        {
            if (savedMessageCo != null)
                StopCoroutine(savedMessageCo);
            savedMessageCo = StartCoroutine(ShowSavedMessage());
        }

        UI.Instance?.ShowSavePointMenu(playerInRange);
    }

    private System.Collections.IEnumerator ShowSavedMessage()
    {
        savedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(savedMessageDuration);
        savedText.gameObject.SetActive(false);
        savedMessageCo = null;
    }
}
