using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneEvent : MonoBehaviour
{
    public static PhoneEvent Instance;

    [Header("UI")]
    public GameObject phoneUI;
    public GameObject notification;
    public GameObject phoneScreen;

    [Header("Interaction")]
    public float notificationDuration = 3f;

    private bool notificationActive;
    private bool phoneOpen;
    private bool callActive;

    private void Awake()
    {
        Instance = this;

        if (phoneUI != null)
            phoneUI.SetActive(false);

        if (notification != null)
            notification.SetActive(false);

        if (phoneScreen != null)
            phoneScreen.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Notification is showing
        if (notificationActive)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenPhone();
            }

            return;
        }

        // Phone is open
        if (phoneOpen)
        {
            if (callActive &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                AnswerCall();
            }

            if (callActive &&
                Keyboard.current.qKey.wasPressedThisFrame)
            {
                DeclineCall();
            }
        }
    }

    public void StartPhoneNotification()
    {
        if (notificationActive || phoneOpen)
            return;

        notificationActive = true;

        if (phoneUI != null)
            phoneUI.SetActive(true);

        if (notification != null)
            notification.SetActive(true);

        if (phoneScreen != null)
            phoneScreen.SetActive(false);

        Debug.Log("Phone notification appeared.");
    }

    private void OpenPhone()
    {
        notificationActive = false;
        phoneOpen = true;
        callActive = true;

        if (notification != null)
            notification.SetActive(false);

        if (phoneScreen != null)
            phoneScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Phone opened.");
    }

    private void AnswerCall()
    {
        callActive = false;

        Debug.Log("Call answered.");

        // Jumpscare will be added here next.
    }

    private void DeclineCall()
    {
        callActive = false;
        phoneOpen = false;

        if (phoneUI != null)
            phoneUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Call declined.");
    }
}