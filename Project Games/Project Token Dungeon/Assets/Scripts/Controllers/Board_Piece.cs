using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Piece : MonoBehaviour
{
    [HideInInspector] public Game_Manager manager;
    public Location_Data data;

    [SerializeField] private List<Action_Controller> actionControllers = new List<Action_Controller>(); 
    public List<Action_ScrObj> insertedActions = new List<Action_ScrObj>();

    private void Start()
    {
        StartCoroutine(Play_Inserted_Actions_WithDelay());
    }

    public void Connect_Manager(Game_Manager manager)
    {
        this.manager = manager;
    }
    public void Update_Location_Data(Location_Data data)
    {
        this.data = data;
    }

    private IEnumerator Play_Inserted_Actions_WithDelay()
    {
        for (int i = 0; i < insertedActions.Count; i++)
        {
            for (int j = 0; j < actionControllers.Count; j++)
            {
                if (insertedActions[i] != actionControllers[j].action) continue;
                if (!actionControllers[j].gameObject.TryGetComponent(out IAction action)) continue;

                action.Action();
                break;
            }

            yield return new WaitForSeconds(insertedActions[i].animationTime);
        }
    }
}