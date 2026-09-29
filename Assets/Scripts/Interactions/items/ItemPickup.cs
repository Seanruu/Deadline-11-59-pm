using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Information")]
    public string itemID = "BlueKey";

    [Header("Pickup Settings")]
    public float interactDistance = 3f;

    private Rigidbody rb;
    private Collider itemCollider;
    private bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (isHeld) return;

        Camera cam = Camera.main;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerItemHolder.Instance.PickupItem(this);
                }
            }
        }
    }

    public void PickUp(Transform holdPoint)
    {
        isHeld = true;

        rb.isKinematic = true;
        rb.useGravity = false;
        itemCollider.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        isHeld = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        itemCollider.enabled = true;

        rb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);
    }
}