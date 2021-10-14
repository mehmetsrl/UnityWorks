using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameExperimentComponent : ExperimentComponent
{
    [Header("Frame Variables")]
    public GameObject background;

    private Material backMaterial;

    protected new void Awake()
    {
        base.Awake();
        if (background != null && background.GetComponent<Renderer>() != null)
            backMaterial = background.GetComponent<Renderer>().material;
    }

    protected override void HandleRenderQueueLevelAssignment(RenderQueueLevel order)
    {
        if (backMaterial != null)
            backMaterial.renderQueue = (int)order;
    }
}
