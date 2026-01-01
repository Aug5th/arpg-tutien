using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    private InventorySlot _slot;
    private System.Action<InventorySlot> _onSelected;

    public void Setup(InventorySlot slot, System.Action<InventorySlot> onSelected)
    {
        _slot = slot;
        _onSelected = onSelected;
        
        iconImage.sprite = slot.itemDefinition.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        iconImage.enabled = true;
    }

    public void OnClick() => _onSelected?.Invoke(_slot);
}
