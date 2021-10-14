using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeCompImp : BaseGazeObject, ICompImp
{
    ExperimentComponent _linkedExperimentComponent;
    private RaycastHit lastHit = new RaycastHit();

    bool ignoreGrasping = true;

    public IComponent GetLinkedComponent()
    {
        return _linkedExperimentComponent;
    }

    public UnityEngine.Component GetComponent()
    {
        return this;
    }

    public void Initiate(IComponent linkedComponent,string tag)
    {
        Initiate(linkedComponent);
        gameObject.tag = tag;
    }

    public void Initiate(IComponent linkedComponent)
    {
        this._linkedExperimentComponent = linkedComponent as ExperimentComponent;

        if (linkedComponent == null) return;

        OnGazeFocusBegin += () =>
        {
            if (linkedComponent.CurrentState == ComponentBase.State.Active)
            {
                this._linkedExperimentComponent.OnContactBegin.Invoke();
            }
        };
        OnGazeFocusEnd += () =>
        {
            if (linkedComponent.CurrentState == ComponentBase.State.Active)
            {
                this._linkedExperimentComponent.OnContactEnd.Invoke();
            }
        };

        OnGazeHover += (RaycastHit hit) => { lastHit = hit; };

        if (!IsInitiated)
            Init();

    }

    public void NotifyButtonPress()
    {
        //We can send events after virtual (experimentComponent )implementation
        //That means we can use filtering by componentside
    }

    public void NotifyButtonUnPress()
    {
        //We can send events after virtual (experimentComponent )implementation
        //That means we can use filtering by componentside
    }

    public void SetState(ComponentBase.State compState)
    {
        //Informs experimentComponent's current status
    }

    public Vector3 GetClosestContactPoint()
    {
        return lastHit.point;
    }
    public Vector3 GetContactItemPosition()
    {
        return Camera.main.transform.position;
    }

    public GameObject GetContactItem()
    {
        return Camera.main.gameObject;
    }

    public bool IsValidAction()
    {
        return true;
    }

    public void IgnoreGrasping(bool ignore)
    {
        ignoreGrasping = ignore;
    }

    public bool IgnoreGrasping()
    {
        return ignoreGrasping;
    }

    public UIManager GetManager()
    {
        return GazeUIManager.Instance;
    }
}
