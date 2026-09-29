using UnityEngine;

public class PlayerItemHolder : MonoBehaviour
{
    public static PlayerItemHolder Instance;

    [Header("Hold Position")]
    public Transform holdPoint;

    private ItemPickup currentItem;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentItem != null && Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    public void PickupItem(ItemPickup item)
    {
        if (currentItem != null) return;

        currentItem = item;
        currentItem.PickUp(holdPoint);
    }

    public void DropItem()
    {
        if (currentItem == null) return;

        ItemPickup itemToDrop = currentItem;
        currentItem = null;

        itemToDrop.Drop();
    }

    public ItemPickup GetHeldItem()
    {
        return currentItem;
    }

    public bool HoldingItem()
    {
        return currentItem != null;
    }

    public void RemoveHeldItem()
    {
        currentItem = null;
    }
}