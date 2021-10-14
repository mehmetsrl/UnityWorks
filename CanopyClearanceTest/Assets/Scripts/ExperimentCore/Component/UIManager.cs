using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIManager : MonoBehaviour
{
    public GameObject UIrootGO;
    public float DistanceFromCamera = .25f;

    //Leaf Components
    [SerializeField]
    protected ButtonExperimentComponent buttonExperimentPrefab;
    [SerializeField]
    protected SliderExperimentComponent sliderExperimentPrefab;
    [SerializeField]
    protected FrameExperimentComponent frameExperimentPrefab;
    [SerializeField]
    protected FrameExperimentComponent backFrameExperimentPrefab;
    [SerializeField]
    protected VideoPlayerExperimentComponent videoPlayerExperimentPrefab;
    [SerializeField]
    protected TextExperimentComponent textExperimentPrefab;
    [SerializeField]
    protected ImageExperimentComponent imageExperimentPrefab;
    [SerializeField]
    protected HolographicViewExperimentComponent hologramPrefab;
    [SerializeField]
    protected FrameExperimentComponent pagePrefab;

    protected abstract void HandleCreatedUIItem(ExperimentComponent experimentComponent);

    public enum Position
    {
        Up,
        Down,
        Right,
        Left,
        Middle
    }
    public enum PositionType
    {
        SameDept,
        Backside,
        Frontside
    }

    private float deptUnitDistance = 0.01f;

    public ExperimentComponent CreateComponent(ExperimentComponent experimentComponentToCreate, ExperimentComponent parentExperimentComponent,
        bool isInnerPosition = true, Position position = Position.Middle, float distance = 0f, PositionType positionType = PositionType.Frontside, Vector2 offset = default(Vector2))
    {
        ExperimentComponent createdItem = GameObject.Instantiate(experimentComponentToCreate);



        if (parentExperimentComponent != null)
        {
            Vector3 initialParentScale = parentExperimentComponent.transform.localScale;
            Vector3 initialParentLossyScale = parentExperimentComponent.transform.lossyScale;
            Transform lastParentOfParent = parentExperimentComponent.transform.parent;
            parentExperimentComponent.transform.parent = null;

            parentExperimentComponent.transform.localScale = Vector3.one;
            Vector3 parentHalfSize = parentExperimentComponent.GetComponentInChildren<Renderer>().bounds.extents;
            parentHalfSize=new Vector3(
                parentHalfSize.x* initialParentLossyScale.x,
                parentHalfSize.y * initialParentLossyScale.y,
                parentHalfSize.z * initialParentLossyScale.z
                );

            Vector3 itemHalfSize = createdItem.GetComponentInChildren<Renderer>().bounds.extents;
            createdItem.transform.parent = parentExperimentComponent.transform;

            parentExperimentComponent.AttachChildren(createdItem);


            switch (position)
            {
                case Position.Middle:
                    createdItem.transform.localPosition = Vector3.zero;
                    createdItem.transform.localRotation = Quaternion.identity;
                    break;
                case Position.Down:
                    if (isInnerPosition)
                        createdItem.transform.localPosition =
                            Vector3.up * (Vector3.Dot(Vector3.up, parentHalfSize) -
                                          (Vector3.Dot(Vector3.up, itemHalfSize) + distance));
                    else
                        createdItem.transform.localPosition =
                            Vector3.up * (Vector3.Dot(Vector3.up, parentHalfSize) +
                                          Vector3.Dot(Vector3.up, itemHalfSize) + distance);

                    createdItem.transform.localRotation = Quaternion.identity;
                    break;
                case Position.Up:
                    if (isInnerPosition)
                        createdItem.transform.localPosition =
                            Vector3.down * (Vector3.Dot(Vector3.up, parentHalfSize) -
                                            (Vector3.Dot(Vector3.up, itemHalfSize) + distance));
                    else
                        createdItem.transform.localPosition =
                            Vector3.down * (Vector3.Dot(Vector3.up, parentHalfSize) +
                                            Vector3.Dot(Vector3.up, itemHalfSize) + distance);

                    createdItem.transform.localRotation = Quaternion.identity;
                    break;
                case Position.Left:
                    if (isInnerPosition)
                        createdItem.transform.localPosition =
                            Vector3.right * (Vector3.Dot(Vector3.right, parentHalfSize) -
                                             (Vector3.Dot(Vector3.right, itemHalfSize) + distance));
                    else
                        createdItem.transform.localPosition =
                            Vector3.right * (Vector3.Dot(Vector3.right, parentHalfSize) +
                                             Vector3.Dot(Vector3.right, itemHalfSize) + distance);

                    createdItem.transform.localRotation = Quaternion.identity;
                    break;
                case Position.Right:
                    if (isInnerPosition)
                        createdItem.transform.localPosition =
                            Vector3.left * (Vector3.Dot(Vector3.right, parentHalfSize) -
                                            (Vector3.Dot(Vector3.right, itemHalfSize) + distance));
                    else
                        createdItem.transform.localPosition =
                            Vector3.left * (Vector3.Dot(Vector3.right, parentHalfSize) +
                                            Vector3.Dot(Vector3.right, itemHalfSize) + distance);

                    createdItem.transform.localRotation = Quaternion.identity;
                    break;
            }

            createdItem.transform.parent = null;

            parentExperimentComponent.transform.parent = lastParentOfParent;
            parentExperimentComponent.transform.localScale = initialParentScale;


            switch (positionType)
            {
                case PositionType.Frontside:
                    createdItem.transform.localPosition += Vector3.forward * deptUnitDistance;
                    break;
                case PositionType.Backside:
                    createdItem.transform.localPosition += Vector3.back * deptUnitDistance;
                    break;
            }

            createdItem.transform.parent = parentExperimentComponent.transform;

            createdItem.transform.localPosition -= new Vector3(offset.x, offset.y, 0);
            StartCoroutine(HandleComponent(createdItem));
        }
;

        return createdItem;
    }

    IEnumerator HandleComponent(ExperimentComponent experimentComponent)
    {
        yield return new WaitUntil(()=>experimentComponent.Initiated);

        HandleCreatedUIItem(experimentComponent);
    }

    //To Generate Root Components
    public ExperimentComponent CreateComponent(ExperimentComponent experimentComponentToCreate, Vector2 offset, PositionType positionType = PositionType.Frontside)
    {
        ExperimentComponent createdItem = GameObject.Instantiate(experimentComponentToCreate);

        if (UIrootGO != null)
        {
            createdItem.transform.parent = UIrootGO.transform;

            createdItem.transform.localPosition = -offset;
            createdItem.transform.localRotation = Quaternion.identity;

            switch (positionType)
            {
                case PositionType.Frontside:
                    createdItem.transform.localPosition += Vector3.back * deptUnitDistance;
                    break;
                case PositionType.Backside:
                    createdItem.transform.localPosition += Vector3.forward * deptUnitDistance;
                    break;
            }
        }

        StartCoroutine(HandleComponent(createdItem));
        return createdItem;
    }
    
}
