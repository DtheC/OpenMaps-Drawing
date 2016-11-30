using UnityEngine;
using System.Collections;
using Gamelogic.Extensions;
using UnityEngine.UI;

public class DebugController : Singleton<DebugController> {

    public WayTracerEmitter Emitter;

    public bool DebugModeActive = false;
    public bool ShowingDead = true;
    
    Rect textArea = new Rect(0, 0, Screen.width, Screen.height);
    
    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            DebugModeActive = !DebugModeActive;
        }

        if (Input.GetKeyDown("l")){
            ShowingDead = !ShowingDead;
            if (ShowingDead)
            {
                Emitter.ShowTheDead();
            } else
            {
                Emitter.HideTheDead();
            }
        }
    }

    void OnGUI()
    {
        if (DebugModeActive)
        {
            if (Emitter)
            {
                GUI.Label(textArea, "Generation:  "+Emitter.GenerationCount.ToString() + "\nEntities Count: "+ Emitter.CurrentEntities.ToString()+"\nCamera Fade Disabled: "+CameraFade.Instance.DisableFade.ToString());
            }
           

        }
    }

}
