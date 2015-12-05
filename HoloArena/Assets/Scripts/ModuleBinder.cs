using UnityEngine;
using System.Collections;

public class ModuleBinder : MonoBehaviour {

    public Transform[] Slots;

	public void Bind(int slot, GameObject module)
    {
        GameObject newModule = (GameObject)Instantiate(module, Slots[slot].position, Slots[slot].rotation);
        newModule.transform.parent = Slots[slot];
    }
}
