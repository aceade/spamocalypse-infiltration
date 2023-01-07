using UnityEngine;

public class AssignPlayer : MonoBehaviour {

    PlayerControl control;
    PlayerMap map;
    Inventory inventory;
    Briefing briefing;
    DevConsole devConsole;

    public void Assign()
    {
        control = GetComponent<PlayerControl>();
        map = GetComponent<PlayerMap>();
        inventory = GetComponent<Inventory>();
        briefing = GetComponent<Briefing>();
        devConsole = GetComponentInChildren<DevConsole>();
        control.AssignPublicVariables();
        map.AssignPublicVariables();
        inventory.AssignPublicVariables();
        devConsole.AssignPublicVariables();
        briefing.CheckAssignments();
    }
}
