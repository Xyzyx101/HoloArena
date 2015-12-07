using UnityEngine;
using System.Collections;

public class RobotBuilder : MonoBehaviour
{
    public RobotData[] RobotObjs;
    public RobotLayoutDefinition[] RobotLayouts;
    public ModuleLayoutDefinition[] ModuleLayouts;
    public InitialStatDefinition[] InitialStats;
    public InitialModuleDefinition[] DefaultModules;
    public int ActiveRobot;

    RobotData CreateRobotDataFromPlayerPrefs()
    {
        RobotData robotData = new RobotData();

        return robotData;
    }

    public RobotLayoutDefinition GetRobotLayout(int index)
    {
        if (index >= 0 && index < RobotLayouts.Length)
        {
            return RobotLayouts[index];
        }
        Debug.LogError("Unknown robot layout : " + index);
        return null;
    }

    public ModuleLayoutDefinition GetModuleLayout(string moduleName)
    {
        for (int i = 0; i < ModuleLayouts.Length; ++i)
        {
            if (ModuleLayouts[i].ModuleName == moduleName)
            {
                return ModuleLayouts[i];
            }
        }
        Debug.LogError("Unknown module definition : " + moduleName);
        return null;
    }

    public GameObject GetModulePrefab(string moduleName)
    {
        for (int i = 0; i < ModuleLayouts.Length; ++i)
        {
            if (ModuleLayouts[i].ModuleName == moduleName)
            {
                return ModuleLayouts[i].Prefab;
            }
        }
        Debug.LogError("Unknown module : " + moduleName);
        return null;
    }

    public void SetModule(int slotID, string moduleName)
    {
        if (moduleName == "")
        {
            RobotObjs[ActiveRobot].Modules[slotID] = null;
        }
        GameObject module = GetModulePrefab(moduleName);
        RobotObjs[ActiveRobot].Modules[slotID] = module;
    }

    public GameObject BuildRobot(Transform robotTransform)
    {
        RobotData robotDef = RobotObjs[ActiveRobot];
        robotDef.EnginePower = InitialStats[ActiveRobot].EnginePower;
        robotDef.Agility = InitialStats[ActiveRobot].Agility;
        robotDef.Weight = InitialStats[ActiveRobot].Weight;
        robotDef.Armour = InitialStats[ActiveRobot].Armour;

        GameObject newRobot = (GameObject)Instantiate(InitialStats[ActiveRobot].Prefab, robotTransform.position, robotTransform.rotation);
        ModuleBinder moduleBinder = newRobot.GetComponent<ModuleBinder>();

        if (robotDef.Modules.Length == 0)
        {
            robotDef.Modules = DefaultModules[ActiveRobot].Modules;
        }

        for (int i = 0; i < robotDef.Modules.Length; ++i)
        {
            GameObject moduleObj = robotDef.Modules[i];
            if (moduleObj == null || moduleObj.name == "Empty")
            {
                continue;
            }
            Module module = moduleObj.GetComponent<Module>();
            robotDef.EnginePower += module.EnginePower;
            robotDef.Agility += module.Agility;
            robotDef.Weight += module.Weight;
            robotDef.Armour += module.Armour;
            Debug.Log(moduleObj.name + " " + i);
            moduleBinder.Bind(i, robotDef.Modules[i]);
        }

        // Set Stats

        return newRobot;
    }
}

public enum RobotType
{
    Scout = 0,
    Assault = 1,
    Engineer = 2
}

public enum SlotType
{
    Weapon,
    Chassis,
    Mobility
}

[System.Serializable]
public class RobotData
{
    public RobotType Type;
    public Color PrimaryColor;
    public Color SecondaryColor;
    public float EnginePower;
    public float Agility;
    public float Weight;
    public float Armour;
    public GameObject[] Modules;
}

[System.Serializable]
public class RobotLayoutDefinition
{
    public SlotDefinition[] Slots;
}

[System.Serializable]
public class SlotDefinition
{
    public SlotType Type;
    public int Group;
}

[System.Serializable]
public class ModuleLayoutDefinition
{
    public string ModuleName;
    public SlotType ModuleType;
    public int ModuleSize;
    public GameObject Prefab;
    public Sprite Icon;
}

[System.Serializable]
public class InitialStatDefinition
{
    public GameObject Prefab;
    public float EnginePower;
    public float Agility;
    public float Weight;
    public float Armour;
}

[System.Serializable]
public class InitialModuleDefinition
{
    public GameObject[] Modules;
}
