using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Common interface fo all executable objects.
    /// </summary>
    public interface ICommand
    {
        void Execute(EventRegistery.InputData inputData, EventRegistery.ActionData actionData);
    }

    /// <summary>
    /// Abstract class to create sealed method.
    /// </summary>
    public abstract class ExperimentEditorCommandBase : ICommand
    {
        public abstract void Execute(EventRegistery.InputData inputData, EventRegistery.ActionData actionData);
    }

    /// <summary>
    /// Main executable class for Experiment Editor
    /// </summary>
    public abstract class ExperimentCommand : ExperimentEditorCommandBase
    {

        public sealed override void Execute(EventRegistery.InputData inputData, EventRegistery.ActionData actionData)
        {
            if (inputData == null) return;
            ExecuteCommand(inputData, actionData);
            inputData = null;
        }
        protected abstract void ExecuteCommand(EventRegistery.InputData inputData, EventRegistery.ActionData actionData);
    }
    
    public class OpenCalibrationUI: ExperimentCommand {
        protected override void ExecuteCommand(EventRegistery.InputData inputData, EventRegistery.ActionData actionData)
        {
                CalibrationManger.Instance.ShowConfigurator(!CalibrationManger.Instance
                    .CalibrationConfiguratorWindowOpen);
        }
    }
    public class ActiveDeactiveObject : ExperimentCommand
    {
        protected override void ExecuteCommand(EventRegistery.InputData inputData, EventRegistery.ActionData actionData)
        {
            actionData.gameObject.SetActive(((EventRegistery.TransformData) actionData).isActive);
        }
    }

    public class SetInfo : ExperimentCommand
    {
        private TextMesh textMesh;
        protected override void ExecuteCommand(EventRegistery.InputData inputData, EventRegistery.ActionData actionData)
        {
            if (actionData.gameObject != null)
            {
                textMesh = actionData.gameObject.GetComponent<TextMesh>();
                if (textMesh == null)
                    textMesh = actionData.gameObject.AddComponent<TextMesh>();

                textMesh.alignment = TextAlignment.Center;
                textMesh.anchor = TextAnchor.LowerCenter;
                textMesh.fontSize = 45;
                textMesh.characterSize = 0.01f;
                textMesh.text = ((EventRegistery.LabelData) actionData).label;
            }
        }
    }

    public class TestAction : ExperimentCommand
    {
        protected override void ExecuteCommand(EventRegistery.InputData inputData, EventRegistery.ActionData actionData)
        {
            Debug.Log("This is a test action");
        }
    }

}