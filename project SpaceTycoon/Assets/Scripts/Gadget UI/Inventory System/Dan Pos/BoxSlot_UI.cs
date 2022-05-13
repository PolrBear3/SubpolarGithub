using UnityEngine;
using UnityEngine.UI;

public class BoxSlot_UI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private Text _itemCount;
    [SerializeField] private Box_Slot _assignedBoxSlot;

    private Button button;

    public Box_Slot assignedBoxSlot => _assignedBoxSlot;
    public Box_Display parentDisplay { get; private set; }

    private void Awake()
    {
        Clear_Slot();
        button = GetComponent<Button>();
        button?.onClick.AddListener(On_UISlot_Click);

        parentDisplay = transform.parent.GetComponent<Box_Display>();

        currentSlotType = GetComponent<Image>();
    }

    private void Start()
    {
        Slot_Status_Update();
    }

    public void Init(Box_Slot slot)
    {
        _assignedBoxSlot = slot;
        Update_UISlot(slot);
    }

    public void Update_UISlot(Box_Slot slot)
    {
        if (slot.itemInfo != null)
        {
            _itemSprite.sprite = slot.itemInfo.itemIcon;
            _itemSprite.color = Color.white;

            if (slot.currentAmount > 1) _itemCount.text = slot.currentAmount.ToString();
            else _itemCount.text = "";
        }
        else
        {
            Clear_Slot();
        }
    }

    public void Update_UISlot()
    {
        if (_assignedBoxSlot != null) Update_UISlot(_assignedBoxSlot);
    }

    public void Clear_Slot()
    {
        _assignedBoxSlot?.Clear_Slot();
        _itemSprite.sprite = null;
        _itemSprite.color = Color.clear;
        _itemCount.text = "";
    }

    public void On_UISlot_Click()
    {
        parentDisplay?.Slot_Clicked(this);
    }

    public static bool slotClicked = false;
    public void Slot_Clicked()
    {
        slotClicked = true;
        slotClicked = false;
    }

    [HideInInspector]
    public bool unlocked = false;
    public GameObject lockedImage;
    public Sprite[] slotTypes;
    [HideInInspector]
    public Image currentSlotType;
    public void Slot_Status_Update()
    {
        if (!unlocked)
        {
            lockedImage.SetActive(true);
            button.enabled = false;
        }
        if (unlocked)
        {
            lockedImage.SetActive(false);
            button.enabled = true;
        }
    }
}
