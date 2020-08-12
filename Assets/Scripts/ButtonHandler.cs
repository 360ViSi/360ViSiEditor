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
        int buttonID = i;
        buttons[i].onClick.AddListener(delegate {whenClicked(buttonID);});
      }
    }

    // Update is called once per frame
    void Update()
    {
      setupButtons(simulationManager.getCurrentVideoPart());
    }

    public void whenClicked(int buttonID)
    {
      simulationManager.actionSelected(buttonID);
    }

    private void setupButtons(VideoPart currentVideoPart)
    {
      int actionCount =  currentVideoPart.getActionCount();
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
