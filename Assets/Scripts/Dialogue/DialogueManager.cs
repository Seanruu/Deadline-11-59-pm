using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float textSpeed = 0.03f;

    private string currentText;
    private int currentLineIndex;

    private string[] dialogueLines;
    private string currentSpeaker;

    private bool dialogueActive;
    private bool isTyping;

    private float typingTimer;
    private int charactersShown;

    private Action dialogueEndAction;

    private void Awake()
    {
        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueActive)
            return;

        if (Keyboard.current == null)
            return;

        // Type the dialogue
        if (isTyping)
        {
            TypeText();
        }

        // Continue dialogue
        if (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ContinueDialogue();
        }
    }

    public void StartDialogue(
        string speaker,
        string[] lines,
        Action onComplete = null)
    {
        if (dialogueActive)
            return;

        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue Panel is not assigned!");
            return;
        }

        if (speakerText == null)
        {
            Debug.LogError("Speaker Text is not assigned!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("Dialogue Text is not assigned!");
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("No dialogue lines provided!");
            return;
        }

        currentSpeaker = speaker;
        dialogueLines = lines;
        currentLineIndex = 0;

        dialogueEndAction = onComplete;

        dialogueActive = true;

        dialoguePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        speakerText.text = currentSpeaker;

        currentText = dialogueLines[currentLineIndex];

        dialogueText.text = "";

        charactersShown = 0;
        typingTimer = 0f;
        isTyping = true;
    }

    private void TypeText()
    {
        if (!isTyping)
            return;

        typingTimer += Time.unscaledDeltaTime;

        if (typingTimer >= textSpeed)
        {
            typingTimer = 0f;

            charactersShown++;

            if (charactersShown >= currentText.Length)
            {
                charactersShown = currentText.Length;

                dialogueText.text = currentText;

                isTyping = false;
            }
            else
            {
                dialogueText.text =
                    currentText.Substring(
                        0,
                        charactersShown
                    );
            }
        }
    }

    private void ContinueDialogue()
    {
        // If text is still typing,
        // finish the current line first.
        if (isTyping)
        {
            dialogueText.text = currentText;

            charactersShown = currentText.Length;

            isTyping = false;

            return;
        }

        // Move to next line
        currentLineIndex++;

        // No more lines
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        isTyping = false;

        currentText = "";
        dialogueLines = null;
        currentLineIndex = 0;

        if (dialogueText != null)
            dialogueText.text = "";

        if (speakerText != null)
            speakerText.text = "";

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Save callback
        Action callback = dialogueEndAction;

        // Clear callback before executing it
        dialogueEndAction = null;

        // Run event after dialogue closes
        if (callback != null)
        {
            callback.Invoke();
        }

        Debug.Log("Dialogue ended.");
    }
}