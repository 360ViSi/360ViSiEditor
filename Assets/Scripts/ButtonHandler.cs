using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private List<Button> buttons;

    [SerializeField]
    private SimulationManager simulationManager;

    void Start()
    {
      for (int i=0; i<buttons.Count; i++)
      {
        int actionID = i;
        buttons[i].onClick.AddListener(delegate {whenClicked(actionID);});
      }
    }

    // Update is called once per frame
    void Update()
    {
      var videoPart = simulationManager.getCurrentVideoPart();
      if(videoPart == null) 
        return;
      setupButtons(videoPart);
    }

    public void whenClicked(int actionID)
    {
      simulationManager.actionSelected(actionID);
    }

    private void setupButtons(VideoPart currentVideoPart)
    {
      int actionCount=0;
      if  (currentVideoPart!=null)
      {
        actionCount =  currentVideoPart.getActionCount();
      }
      var listOfEnabledActions = new List<Action>();

      for (int i=0; i<buttons.Count;i++)
      {
        if (i<actionCount)
        {
          buttons[i].gameObject.SetActive(true);
          buttons[i].gameObject.GetComponentInChildren<Text>().text=currentVideoPart.getActionText(i);
          continue;
        }
        buttons[i].gameObject.SetActive(false);
      }

    }

}
