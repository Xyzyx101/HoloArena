using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModuleSlotDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor = Color.yellow;
    public Color rejectColor = Color.grey;
    public int SlotID;
    public SlotType Type;
    public CreationController MyCreationController;

    public void OnEnable()
    {
        if (containerImage != null)
            normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data)
    {
        containerImage.color = normalColor;

        if (receivingImage == null)
            return;
        if (!MyCreationController.VerifySlot(SlotID, GetModuleName(data)))
        {
            return;
        }

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite == null)
        {
            return;
        }

        ModuleLayoutDefinition moduleDefinition = MyCreationController.Builder.GetModuleLayout(GetModuleName(data));
        if (MyCreationController.VerifySlot(SlotID, GetModuleName(data)))
        {
            receivingImage.overrideSprite = dropSprite;
            MyCreationController.SetModule(SlotID, GetModuleName(data));
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Show Module Info");
        if (containerImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
        {
            if (MyCreationController.VerifySlot(SlotID, GetModuleName(data)))
            {
                containerImage.color = highlightColor;
            }
            else
            {
                containerImage.color = rejectColor;
            }
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (containerImage == null)
            return;

        containerImage.color = normalColor;
    }

    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var moduleDrag = originalObj.GetComponent<ModuleDrag>();
        if (moduleDrag == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }

    private string GetModuleName(PointerEventData data)
    {
        var dragObj = data.pointerDrag;
        var moduleDrag = dragObj.GetComponent<ModuleDrag>();
        if (moduleDrag == null)
        {
            return "";
        }
        return moduleDrag.ModuleName;
    }
}
