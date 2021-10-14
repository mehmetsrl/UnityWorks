using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.InputManagement;
using UnityEngine;



namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Common interface fo all executable events.
    /// </summary>
    public interface IEventInvoker
    {
        void Execute(EventRegistery registery);
    }

    /// <summary>
    /// Abstract class to create sealed method.
    /// </summary>
    public abstract class ExperimentEventCommandBase : IEventInvoker
    {
        public abstract void Execute(EventRegistery registery);
    }

    /// <summary>
    /// Main executable class for Experiment Editor
    /// </summary>
    public abstract class ExperimentEventCommand : ExperimentEventCommandBase
    {
        protected EventRegistery.InputData inputData;

        protected ExperimentEventCommand(EventRegistery.InputData inputData)
        {
            this.inputData = inputData;
        }

        public sealed override void Execute(EventRegistery registery)
        {
            if (inputData == null) return;
            ExecuteCommand(registery);
        }

        protected abstract void ExecuteCommand(EventRegistery registery);
    }

    public class CTRL_CombinationCommand : ExperimentEventCommand
    {
        public CTRL_CombinationCommand(EventRegistery.KeyboardInputData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.CTRL_CombinationEvent.Invoke(inputData as EventRegistery.KeyboardInputData);
        }
    }
    public class ALT_CombinationCommand : ExperimentEventCommand
    {
        public ALT_CombinationCommand(EventRegistery.KeyboardInputData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.ALT_CombinationEvent.Invoke(inputData as EventRegistery.KeyboardInputData);
        }
    }

    public class ViveButtonCommand : ExperimentEventCommand
    {
        public ViveButtonCommand(EventRegistery.ViveInputData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnViveButton.Invoke(inputData as EventRegistery.ViveInputData);
        }
    }

    public class ViveButtonUpCommand : ExperimentEventCommand
    {
        public ViveButtonUpCommand(EventRegistery.ViveInputData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnViveButtonUp.Invoke(inputData as EventRegistery.ViveInputData);
        }
    }

    public class ViveButtonDownCommand : ExperimentEventCommand
    {
        public ViveButtonDownCommand(EventRegistery.ViveInputData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnViveButtonDown.Invoke(inputData as EventRegistery.ViveInputData);
        }
    }

    public class LeapContactCommand : ExperimentEventCommand
    {
        public LeapContactCommand(EventRegistery.LeapInputContactData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnLeapContact.Invoke(inputData as EventRegistery.LeapInputContactData);
        }
    }
    public class LeapGestureCommand : ExperimentEventCommand
    {
        public LeapGestureCommand(EventRegistery.LeapInputGestureData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnLeapGesture.Invoke(inputData as EventRegistery.LeapInputGestureData);
        }
    }
    public class LeapComponentTriggerCommand : ExperimentEventCommand
    {
        public LeapComponentTriggerCommand(EventRegistery.LeapInputComponentData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnLeapComponentTriggered.Invoke(inputData as EventRegistery.LeapInputComponentData);
        }
    }

    public class EyeHoverCommand : ExperimentEventCommand
    {
        public EyeHoverCommand(EventRegistery.EyeData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnEyeHover.Invoke(inputData as EventRegistery.EyeData);
        }
    }

    public class EyeHoverEndCommand : ExperimentEventCommand
    {
        public EyeHoverEndCommand(EventRegistery.EyeData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnEyeHoverEnd.Invoke(inputData as EventRegistery.EyeData);
        }
    }

    public class EyeFocusCommand : ExperimentEventCommand
    {
        public EyeFocusCommand(EventRegistery.EyeData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnEyeFocused.Invoke(inputData as EventRegistery.EyeData);
        }
    }
    public class EyeFocusEndCommand : ExperimentEventCommand
    {
        public EyeFocusEndCommand(EventRegistery.EyeData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnEyeFocusEnd.Invoke(inputData as EventRegistery.EyeData);
        }
    }

    public class JoystickCommand : ExperimentEventCommand
    {
        public JoystickCommand(EventRegistery.JoystickData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnJoystickDown.Invoke(inputData as EventRegistery.JoystickData);
        }
    }
    public class JoystickAxisCommand : ExperimentEventCommand
    {
        public JoystickAxisCommand(EventRegistery.JoystickAxisData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnJoystickAxisChange.Invoke(inputData as EventRegistery.JoystickAxisData);
        }
    }


    public class CollisionCommand : ExperimentEventCommand
    {
        public CollisionCommand(EventRegistery.CollisionData inputData) : base(inputData)
        {
        }

        protected override void ExecuteCommand(EventRegistery registery)
        {
            registery.OnCollisionOccour.Invoke(inputData as EventRegistery.CollisionData);
        }
    }
}