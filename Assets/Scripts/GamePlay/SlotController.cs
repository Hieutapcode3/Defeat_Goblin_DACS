using DG.Tweening;
using TMPro;
using UnityEngine;


[System.Serializable]
public class SlotData
{
    public int index = 0;
    public EntityData entityData = null;
}
public class SlotController : MonoBehaviour
{
    public SlotData slotData;
    public BaseEntity currentEntity;
    public Transform Slot_Selected_Trans { get; private set; }
    public Transform Slot_Active_Trans { get; private set; }
    public Transform Flag_Trans { get; private set; }
    public Transform Level_Trans { get; private set; }
    public bool IsBuy { get; private set; } = false;
    private void Awake()
    {
        // Slot_Selected = this.transform.Find("Slot_Selected");
        // Slot_Active = this.transform.Find("Slot_Active");
        // Flag = Slot_Active.Find("Flag");
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == "Slot_Selected") Slot_Selected_Trans = child;
            if (child.name == "Slot_Active") Slot_Active_Trans = child;
            if (child.name == "Flag") Flag_Trans = child;
            if (child.name == "Level") Level_Trans = child;
        }
    }
    private void Start()
    {
        if (slotData.entityData?.type == EntityType.Enemy)
        {
            SpawnNewEntity();
            // GameObject newEnemy = Instantiate(slotData.entityData.prefab, transform);
            // newEnemy.GetComponent<BaseEntity>().slot = this;
            // Flag.gameObject.SetActive(false);
        }
    }
    public void Init(bool IsBuy, int index, EntityData entityData)
    {
        this.IsBuy = IsBuy;
        this.slotData.index = index;
        this.slotData.entityData = entityData;
    }

    public void SelectedSlot()
    {
        Slot_Selected_Trans.gameObject.SetActive(true);
    }

    public void UnSelectedSlot()
    {
        Slot_Selected_Trans.gameObject.SetActive(false);
    }
    public void Empty()
    {
        Slot_Active_Trans?.gameObject.SetActive(true);
        Slot_Active_Trans?.Find("Level").gameObject.SetActive(false);
        Flag_Trans?.gameObject.SetActive(true);
        Flag_Trans?.GetComponent<Animator>().SetTrigger("Spawn");
    }
    public void Occupied()
    {
        Slot_Active_Trans?.gameObject.SetActive(false);
        if (Flag_Trans)
        {
            Flag_Trans.gameObject.SetActive(false);
            Flag_Trans.Find("Sprite").transform.localScale = Vector2.zero;
            Flag_Trans.Find("Shadow").transform.localScale = Vector2.zero;
        }
    }
    public void SpawnNewEntity()
    {
        GameObject newChar = Instantiate(slotData.entityData.prefab, Vector3.zero, Quaternion.identity);
        newChar.transform.localScale = Vector3.zero;
        newChar.transform.position = transform.position;
        newChar.transform.parent = transform;
        currentEntity = newChar.GetComponent<BaseEntity>();
        currentEntity.entityData = slotData.entityData;
        currentEntity.slot = this;

        newChar.transform.DOScale(Vector3.one * (slotData.entityData.type == EntityType.Enemy ? 0.5f : 0.2f), 0.3f).SetEase(Ease.OutBack);
        Level_Trans?.gameObject.SetActive(true);
        GetComponentInChildren<TextMeshProUGUI>().text = ((int)slotData.entityData.level + 1).ToString();
        Flag_Trans.gameObject.SetActive(false);
        GoldManager.Instance.UpdateGoldPerSecondTxt();
    }
}
