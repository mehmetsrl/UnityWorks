using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScene : TempScene
{
    protected override void BindEvents()
    {
    }

    protected override void Init()
    {
    }
    
    protected override void Run()
    {
    }

    protected override void SelectContiunousHandlers(out List<InputManager.ContiunousHandler> contiunousHandlersNeeded)
    {
        contiunousHandlersNeeded=new List<InputManager.ContiunousHandler>();
    }

    protected override void SelectDiscreateHandlers(out List<InputManager.Handler> handlersNeeded)
    {
        handlersNeeded=new List<InputManager.Handler>();
    }

    protected override void UnBindEvents()
    {
    }
}
