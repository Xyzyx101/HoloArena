using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreationController : MonoBehaviour
{
    public TurnTable MyTurnTable;
    public int SelectedRobot;
    public RobotBuilder Builder;
    public RobotSelectorType[] Robots;
    public RobotLayoutDefinition RobotDefinition;

    void Start()
    {
        for (int i = 0; i < Robots.Length; ++i)
        {
            Builder.ActiveRobot = i;
            GameObject robot = Builder.BuildRobot(Robots[i].Spawn);
            robot.transform.parent = Robots[i].Spawn;
            PlayerController player = robot.GetComponent<PlayerController>();
            player.LookAtTarget = MyTurnTable.transform;
            player.CanMove = false;
        }
        RobotDefinition = Builder.GetRobotLayout(0);
        Builder.ActiveRobot = 0;
        SetIconsFromBuilder();
    }

    public void NextRobot()
    {
        SelectedRobot = ++SelectedRobot % 3;
        MyTurnTable.TurnToSlot(SelectedRobot);
        RobotDefinition = Builder.GetRobotLayout(SelectedRobot);
        Builder.ActiveRobot = SelectedRobot;
        SetIconsFromBuilder();
    }

    public void PrevRobot()
    {
        --SelectedRobot;
        if (SelectedRobot == -1)
        {
            SelectedRobot = 2;
        }
        MyTurnTable.TurnToSlot(SelectedRobot);
        RobotDefinition = Builder.GetRobotLayout(SelectedRobot);
        Builder.ActiveRobot = SelectedRobot;
        SetIconsFromBuilder();
    }

    public bool VerifySlot(int slotID, string moduleName)
    {
        RobotLayoutDefinition robotDefinition = Builder.GetRobotLayout(SelectedRobot);
        ModuleLayoutDefinition moduleDefinition = Builder.GetModuleLayout(moduleName);

        // Check that type matches
        if (robotDefinition.Slots[slotID].Type != moduleDefinition.ModuleType)
        {
            return false;
        }

        // Check the group is big enough
        int groupSize = 0;
        for (int i = 0; i < robotDefinition.Slots.Length; ++i)
        {
            if (robotDefinition.Slots[i].Group == robotDefinition.Slots[slotID].Group)
            {
                ++groupSize;
            }
        }
        if (moduleDefinition.ModuleSize <= groupSize)
        {
            return true;
        }

        return false;
    }

    public void SetModule(int slotID, string moduleName)
    {
        RobotLayoutDefinition robotDefinition = Builder.GetRobotLayout(SelectedRobot);
        ModuleLayoutDefinition moduleDef = Builder.GetModuleLayout(moduleName);
        if (moduleDef.ModuleSize == 1)
        {
            Builder.SetModule(slotID, moduleName);
        }
        else
        {
            int usedSlots = 0;
            for (int i = 0; i < robotDefinition.Slots.Length; ++i)
            {
                if (robotDefinition.Slots[i].Group != robotDefinition.Slots[slotID].Group)
                {
                    continue;
                }
                if (usedSlots == 0)
                {
                    Builder.SetModule(i, moduleName);
                    SetIcon(i, moduleName);
                }
                else
                {
                    Builder.SetModule(i, "Empty");
                    SetIcon(i, "Empty");
                }
                ++usedSlots;
            }
        }
        Transform spawn = Robots[SelectedRobot].Spawn;
        
        Destroy(spawn.GetChild(0).gameObject);

        GameObject robot = Builder.BuildRobot(Robots[SelectedRobot].Spawn);
        robot.transform.parent = Robots[SelectedRobot].Spawn;
        PlayerController player = robot.GetComponent<PlayerController>();
        player.LookAtTarget = MyTurnTable.transform;
        player.CanMove = false;
        SetIconsFromBuilder();
    }

    private void SetIcon(int slotID, string moduleName)
    {
        Debug.Log("slotID:" + slotID + " moduleName:" + moduleName);
        GameObject x = Robots[SelectedRobot].Panel;
        Transform y = x.transform;
        string z = "Slot" + slotID.ToString();
        Transform q = y.Find(z);
        GameObject r = q.gameObject;
        GameObject slotImageObj = Robots[SelectedRobot].Panel.transform.Find("Slot" + slotID.ToString() + "/Image").gameObject;
        Image slotImage = null;
        if (slotImageObj != null)
        {
            slotImage = slotImageObj.GetComponent<Image>();
        }
        if (slotImage == null)
        {
            Debug.Log("No image found for slot " + slotID);
            return;
        }
        slotImage.overrideSprite = Builder.GetModuleLayout(moduleName).Icon;
    }

    void SetIconsFromBuilder()
    {
        GameObject[] modules = Builder.RobotObjs[SelectedRobot].Modules;
        for (int i = 0; i < modules.Length; ++i)
        {
            if (modules[i] == null)
            {
                continue;
            }
            Module module = modules[i].GetComponent<Module>();
            if (module != null)
            {
                SetIcon(i, module.ModuleName);
            }
        }
    }
}

[System.Serializable]
public class RobotSelectorType
{
    public string name;
    public Transform Spawn;
    public GameObject Panel;
}

[System.Serializable]
public class ModulePlacementData
{
    public string name;
    public SlotType slotType;
    public int size;
}