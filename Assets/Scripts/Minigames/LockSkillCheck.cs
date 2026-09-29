using UnityEngine;
using TMPro;

public class LockSkillCheck : MonoBehaviour
{
    public static LockSkillCheck Instance;

    [Header("UI")]
    public GameObject panel;
    public RectTransform bar;
    public RectTransform marker;
    public RectTransform successZone;
    public TMP_Text progressText;

    [Header("Settings")]
    public float markerSpeed = 450f;
    public float successZoneWidth = 80f;
    public int requiredHits = 3;

    private float leftLimit;
    private float rightLimit;
    private float markerPosition;

    private bool movingRight;
    private bool isPlaying;

    private int currentHits;

    private DoorWithKey currentDoor;
    private ItemPickup currentKey;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        MoveMarker();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSkill();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelMinigame();
        }
    }

    public void StartSkillCheck(DoorWithKey door, ItemPickup key)
    {
        if (isPlaying)
            return;

        if (panel == null)
        {
            Debug.LogError("LockSkillCheck: Panel is not assigned!");
            return;
        }

        if (bar == null)
        {
            Debug.LogError("LockSkillCheck: Bar is not assigned!");
            return;
        }

        if (marker == null)
        {
            Debug.LogError("LockSkillCheck: Marker is not assigned!");
            return;
        }

        if (successZone == null)
        {
            Debug.LogError("LockSkillCheck: Success Zone is not assigned!");
            return;
        }

        if (progressText == null)
        {
            Debug.LogError("LockSkillCheck: Progress Text is not assigned!");
            return;
        }

        currentDoor = door;
        currentKey = key;

        panel.SetActive(true);

        Canvas.ForceUpdateCanvases();

        leftLimit = -(bar.rect.width / 2f);
        rightLimit = bar.rect.width / 2f;

        if (rightLimit <= leftLimit)
        {
            Debug.LogError("LockSkillCheck: Bar width is 0 or invalid.");
            panel.SetActive(false);
            return;
        }

        currentHits = 0;
        UpdateProgress();

        isPlaying = true;

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetupRound();
    }

    private void MoveMarker()
    {
        float movement = markerSpeed * Time.unscaledDeltaTime;

        if (movingRight)
            markerPosition += movement;
        else
            markerPosition -= movement;

        if (markerPosition >= rightLimit)
        {
            markerPosition = rightLimit;
            movingRight = false;
        }

        if (markerPosition <= leftLimit)
        {
            markerPosition = leftLimit;
            movingRight = true;
        }

        marker.anchoredPosition = new Vector2(markerPosition, 0f);
    }

    private void SetupRound()
    {
        float halfZone = successZoneWidth / 2f;

        float minimum = leftLimit + halfZone;
        float maximum = rightLimit - halfZone;

        float randomPosition = Random.Range(minimum, maximum);

        successZone.sizeDelta = new Vector2(
            successZoneWidth,
            successZone.sizeDelta.y
        );

        successZone.anchoredPosition = new Vector2(
            randomPosition,
            0f
        );

        markerPosition = leftLimit;
        movingRight = true;

        marker.anchoredPosition = new Vector2(
            markerPosition,
            0f
        );
    }

    private void CheckSkill()
    {
        float zoneCenter = successZone.anchoredPosition.x;
        float zoneHalf = successZoneWidth / 2f;

        bool hit =
            markerPosition >= zoneCenter - zoneHalf &&
            markerPosition <= zoneCenter + zoneHalf;

        if (hit)
        {
            currentHits++;

            UpdateProgress();

            if (currentHits >= requiredHits)
            {
                CompleteMinigame();
            }
            else
            {
                SetupRound();
            }
        }
        else
        {
            currentHits = 0;

            UpdateProgress();

            SetupRound();
        }
    }

    private void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text =
                "UNLOCKING  " +
                currentHits +
                " / " +
                requiredHits;
        }
    }

    private void CompleteMinigame()
    {
        isPlaying = false;

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (panel != null)
            panel.SetActive(false);

        DoorWithKey door = currentDoor;

        currentDoor = null;
        currentKey = null;

        if (door != null)
        {
            door.UnlockDoor();
        }
    }

    private void CancelMinigame()
    {
        isPlaying = false;

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (panel != null)
            panel.SetActive(false);

        currentDoor = null;
        currentKey = null;
    }
}