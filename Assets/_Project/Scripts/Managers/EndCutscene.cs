using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutscene : MonoBehaviour
{
   public void End()
    {
        GameManager.Instance.MainMenu(true, 0);
    }

    public void EndFirstCutscene()
    {
        GameManager.Instance.StartGame();
    }
}
