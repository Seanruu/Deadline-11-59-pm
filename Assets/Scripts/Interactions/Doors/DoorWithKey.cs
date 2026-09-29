using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorWithKey : MonoBehaviour
{
    [Header("Door Settings")]
    public string requiredKeyID = "BlueKey";
    public float interactDistance = 3f;

    [Header("Opening")]
    public float openAngle = 90f;
    public float openSpeed = 120f;

    [Header("Audio")]
    public AudioClip unlockSound;
    public AudioClip lockedSound;

    [Header("Key")]
    public bool consumeKey = true;

    private AudioSource audioSource;

    private bool isOpening;
    private bool isOpen;

    private Quaternion targetRotation;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                openSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isOpening = false;
            }

            return;
        }

        if (isOpen)
            return;

        Camera cam = Camera.main;

        if (cam == null)
            return;

        Ray ray = new Ray(
            cam.transform.position,
            cam.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactDistance))
        {
            DoorWithKey door =
                hit.collider.GetComponentInParent<DoorWithKey>();

            if (door == this &&
                Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }
    }

    private void TryInteract()
    {
        if (PlayerItemHolder.Instance == null)
        {
            Debug.LogError("PlayerItemHolder.Instance is missing!");
            return;
        }

        ItemPickup heldItem =
            PlayerItemHolder.Instance.GetHeldItem();

        // No item
        if (heldItem == null)
        {
            PlayLockedSound();
            return;
        }

        // Wrong item
        if (heldItem.itemID != requiredKeyID)
        {
            PlayLockedSound();
            return;
        }

        // Correct key
        if (LockSkillCheck.Instance == null)
        {
            Debug.LogError(
                "LockSkillCheck.Instance is missing! " +
                "Make sure LockSkillCheck exists in the scene."
            );

            return;
        }

        LockSkillCheck.Instance.StartSkillCheck(
            this,
            heldItem
        );
    }

    private void PlayLockedSound()
    {
        if (audioSource != null &&
            lockedSound != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }
    }

    public void UnlockDoor()
    {
        if (isOpen || isOpening)
            return;

        isOpen = true;
        isOpening = true;

        targetRotation =
            transform.rotation *
            Quaternion.Euler(0f, openAngle, 0f);

        if (audioSource != null &&
            unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        if (consumeKey)
        {
            if (PlayerItemHolder.Instance != null)
            {
                ItemPickup key =
                    PlayerItemHolder.Instance.GetHeldItem();

                if (key != null)
                {
                    PlayerItemHolder.Instance.RemoveHeldItem();

                    Destroy(key.gameObject);
                }
            }
        }
    }
}