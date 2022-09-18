#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class CharacterMakerWizard : ScriptableWizard
{
    [SerializeField] private CharacterAppearanceObject m_appearanceObject = null;
    [SerializeField] private CharacterBehaviorObject m_behaviorObject = null;

    [SerializeField] private ItemObject m_itemLeftHand = null;
    [SerializeField] private ItemObject m_itemRightHand = null;

    [SerializeField] private AccessoryObject[] m_accessories = null;

    [SerializeField] private bool m_keepItemPrefabConnection = true;
    [SerializeField] private bool m_keepCharacterPrefabConnection = false;

    [MenuItem("Supercyan/Character Maker")]
    private static void CreateWizard()
    {
        DisplayWizard<CharacterMakerWizard>("Character Maker");
    }

    public static void CreateCharacter(CharacterAppearanceObject appearance, CharacterBehaviorObject behaviour, ItemObject leftHandItem, ItemObject rightHandItem, AccessoryObject[] accessories, Vector3 offset, bool keepCharacterPrefabConnection = false, bool keepItemPrefabConnection = true)
    {
        if (EditorApplication.isPlaying) {
            if (keepCharacterPrefabConnection)
            {
                keepCharacterPrefabConnection = false;
                Debug.LogError("Character Maker: keepCharacterPrefabConnection is not supported in playmode");
            }

            if (keepItemPrefabConnection)
            {
                keepItemPrefabConnection = false;
                Debug.LogError("Character Maker: keepItemPrefabConnection is not supported in playmode");
            }
        }

        GameObject character = Instantiate(appearance.Model, offset, Quaternion.identity, keepCharacterPrefabConnection);

        CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
        collider.radius = 0.1f;
        collider.direction = 1;
        collider.center = new Vector3(0.0f, 0.5f, 0.0f);

        Rigidbody rigidbody = character.AddComponent<Rigidbody>();
        rigidbody.angularDrag = 5.0f;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (behaviour != null)
        {
            behaviour.InitializeBehaviour(character);
        }

        ItemHoldLogic itemHoldLogic = character.GetComponent<ItemHoldLogic>();
        AccessoryWearLogic accessoryWearLogic = character.GetComponent<AccessoryWearLogic>();
        bool hasItems = leftHandItem || rightHandItem;

        if ((itemHoldLogic == null) && (leftHandItem || rightHandItem))
        {
            itemHoldLogic = character.AddComponent<ItemHoldLogic>();
            hasItems = true;
        }

        if (itemHoldLogic != null)
        {
            itemHoldLogic.Initialize(character);
        }

        if (accessoryWearLogic == null)
        {
            if((leftHandItem != null && leftHandItem.Item != null && leftHandItem.Item.AccessoryLogic != null)
            || (rightHandItem != null && rightHandItem.Item != null && rightHandItem.Item.AccessoryLogic != null)
            || (accessories != null && accessories.Length > 0))
            {
                accessoryWearLogic = character.AddComponent<AccessoryWearLogic>();
            }
        }

        // Attach items
        if (hasItems)
        {
            CharacterItemAnimator itemAnimator;
            itemAnimator = itemHoldLogic.gameObject.AddComponent<CharacterItemAnimator>();
            itemAnimator.ThisHand = CharacterItemAnimator.Hand.Left;

            itemAnimator = itemHoldLogic.gameObject.AddComponent<CharacterItemAnimator>();
            itemAnimator.ThisHand = CharacterItemAnimator.Hand.Right;
        }

        AttachItem(character, accessoryWearLogic, leftHandItem, itemHoldLogic, CharacterItemAnimator.Hand.Left, keepItemPrefabConnection);
        AttachItem(character, accessoryWearLogic, rightHandItem, itemHoldLogic, CharacterItemAnimator.Hand.Right, keepItemPrefabConnection);

        if (accessories != null)
        {
            // Attach accessories
            foreach (AccessoryObject a in accessories)
            {
                if(a == null) { continue; }
                AccessoryLogic accessory = Instantiate(a.Accessory, keepItemPrefabConnection);
                accessory.transform.parent = character.transform;
                accessoryWearLogic.Attach(accessory);
            }
        }
    }

    private static void AttachItem(GameObject character, AccessoryWearLogic accessoryWearLogic, ItemObject itemObject, ItemHoldLogic itemHoldLogic, CharacterItemAnimator.Hand hand, bool keepPrefabConnection)
    {
        if (itemObject != null)
        {
            if(itemObject.Item == null)
            {
                Debug.LogError("Null prefab in " + itemObject);
                return;
            }

            ItemLogic item = Instantiate(itemObject.Item, keepPrefabConnection);
            if (hand == CharacterItemAnimator.Hand.Left)
            {
                itemHoldLogic.m_itemInHandL = item;
            }
            else if (hand == CharacterItemAnimator.Hand.Right)
            {
                itemHoldLogic.m_itemInHandR = item;
            }

            if (itemObject.Item.AccessoryLogic)
            {
                item.transform.parent = character.transform;
                accessoryWearLogic.Attach(item.AccessoryLogic);
            }
            else
            {
                if (hand == CharacterItemAnimator.Hand.Left)
                {
                    item.transform.parent = itemHoldLogic.HandBoneL;
                }
                else if (hand == CharacterItemAnimator.Hand.Right)
                {
                    item.transform.parent = itemHoldLogic.HandBoneR;
                }

                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private static T Instantiate<T>(T component, bool keepPrefabConnection) where T : Component
    {
        if(component == null) { return null; }
        GameObject instance = Instantiate(component.gameObject, keepPrefabConnection);
        return instance.GetComponent<T>();
    }

    private static GameObject Instantiate(GameObject original, bool keepPrefabConnection)
    {
        if(keepPrefabConnection)
        {
            return PrefabUtility.InstantiatePrefab(original) as GameObject;
        }
        else
        {
            return Instantiate(original);
        }
    }

    private static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, bool keepPrefabConnection)
    {
        if (keepPrefabConnection)
        {
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(original) as GameObject;
            prefabInstance.transform.SetPositionAndRotation(position, rotation);
            return prefabInstance;
        }
        else
        {
            return Instantiate(original, position, rotation);
        }
    }

    private void OnWizardUpdate()
    {
        isValid = m_appearanceObject != null && m_behaviorObject != null;
    }

    private void OnWizardCreate()
    {
        CreateCharacter(m_appearanceObject, m_behaviorObject, m_itemLeftHand, m_itemRightHand, m_accessories, Vector3.zero, m_keepCharacterPrefabConnection, m_keepItemPrefabConnection);
    }
}
#endif