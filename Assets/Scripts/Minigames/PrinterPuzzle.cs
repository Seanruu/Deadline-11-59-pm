using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PrinterPuzzle : MonoBehaviour
{
    [Header("UI")]
    public GameObject keypadPanel;
    public TMP_Text displayText;
    public TMP_Text messageText;

    [Header("Puzzle")]
    public string correctCode = "4312";
    public int maxCodeLength = 4;

    [Header("Interaction")]
    public float interactDistance = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip printerBreakSound;

    private string enteredCode = "";

    private bool puzzleOpen = false;
    private bool printerBroken = false;

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;

        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        UpdateDisplay();
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            return;
        }

        // If keypad is open
        if (puzzleOpen)
        {
            HandlePuzzleInput();
            return;
        }

        // Printer already broken
        if (printerBroken)
            return;

        // Interact with printer
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteractWithPrinter();
        }
    }

    private void TryInteractWithPrinter()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactDistance))
        {
            PrinterPuzzle printer =
                hit.collider.GetComponentInParent<PrinterPuzzle>();

            if (printer == this)
            {
                OpenPuzzle();
            }
        }
    }

    private void OpenPuzzle()
    {
        if (printerBroken)
            return;

        puzzleOpen = true;

        enteredCode = "";

        if (keypadPanel != null)
            keypadPanel.SetActive(true);

        if (messageText != null)
            messageText.text = "ENTER SERVICE CODE";

        UpdateDisplay();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandlePuzzleInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePuzzle();
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            AddNumber("1");

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            AddNumber("2");

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            AddNumber("3");

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            AddNumber("4");

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            AddNumber("5");

        if (Keyboard.current.digit6Key.wasPressedThisFrame)
            AddNumber("6");

        if (Keyboard.current.digit7Key.wasPressedThisFrame)
            AddNumber("7");

        if (Keyboard.current.digit8Key.wasPressedThisFrame)
            AddNumber("8");

        if (Keyboard.current.digit9Key.wasPressedThisFrame)
            AddNumber("9");

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            AddNumber("0");

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
            ClearCode();

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            CheckCode();
    }

    public void AddNumber(string number)
    {
        if (!puzzleOpen)
            return;

        if (enteredCode.Length >= maxCodeLength)
            return;

        enteredCode += number;

        UpdateDisplay();

        PlaySound(buttonSound);
    }

    public void ClearCode()
    {
        if (!puzzleOpen)
            return;

        enteredCode = "";

        UpdateDisplay();

        PlaySound(buttonSound);
    }

    public void CheckCode()
    {
        if (!puzzleOpen)
            return;

        if (enteredCode == correctCode)
        {
            CorrectCode();
        }
        else
        {
            WrongCode();
        }
    }

    private void CorrectCode()
    {
        if (messageText != null)
            messageText.text = "CODE ACCEPTED";

        PlaySound(correctSound);

        Invoke(nameof(BreakPrinter), 1.5f);
    }

    private void WrongCode()
    {
        if (messageText != null)
            messageText.text = "INCORRECT CODE";

        PlaySound(wrongSound);

        Invoke(nameof(BreakPrinter), 1.5f);
    }

    private void BreakPrinter()
    {
        if (printerBroken)
            return;

        printerBroken = true;

        puzzleOpen = false;

        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlaySound(printerBreakSound);

        Debug.Log("Printer is broken.");

        PrinterFailureEvent();
    }

    private void PrinterFailureEvent()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError(
                "DialogueManager.Instance is missing!"
            );

            return;
        }

        string[] lines =
        {
            "Did this printer just break on me?"
        };

        // Start Julian's dialogue
        // When the dialogue ends,
        // start the phone notification.
        DialogueManager.Instance.StartDialogue(
            "JULIAN",
            lines,
            () =>
            {
                if (PhoneEvent.Instance != null)
                {
                    PhoneEvent.Instance.StartPhoneNotification();
                }
                else
                {
                    Debug.LogError(
                        "PhoneEvent.Instance is missing! " +
                        "Make sure a PhoneEvent exists in the scene."
                    );
                }
            }
        );
    }

    private void UpdateDisplay()
    {
        if (displayText == null)
            return;

        if (enteredCode.Length == 0)
        {
            displayText.text = "____";
            return;
        }

        string display = "";

        for (int i = 0; i < maxCodeLength; i++)
        {
            if (i < enteredCode.Length)
                display += enteredCode[i];
            else
                display += "_";
        }

        displayText.text = display;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void ClosePuzzle()
    {
        puzzleOpen = false;

        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}