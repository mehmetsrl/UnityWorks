using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Abstract class to create sealed method.
    /// </summary>
    public abstract class BaseHandler
    {
        public abstract IEventInvoker[] GetInput();
    }

    /// <summary>
    /// Main Handler class for all input methods. It implements chain of responsibility to get inputs.
    /// It has sealed class that handles inputs. If the handler cannot detect any input, it tries to get input from next handler.
    /// Handler definition method is HandleInput which is an abstract method ready to fill.
    /// </summary>
    public abstract class InputHandler : BaseHandler,IDisposable
    {
        private InputHandler nextHandler;

        /// <summary>
        /// Try to get input from handlers. If could not detect any input, returns null
        /// </summary>
        /// <returns>Returns ICommand interface to execute results.</returns>
        public sealed override IEventInvoker[] GetInput()
        {
            IEventInvoker[] result = HandleInput();
            if ((result==null || result.Length==0) && nextHandler != null)
                result = nextHandler.GetInput();
            return result;
        }

        /// <summary>
        /// Links handlers each others, set null if it is a tail.
        /// </summary>
        /// <param name="nextHandler">Next chain to handle inputs.</param>
        public InputHandler(InputHandler nextHandler)
        {
            this.nextHandler = nextHandler;
        }

        /// <summary>
        /// Adds new handler to end of the chain after initialization.
        /// </summary>
        /// <param name="nextHandler"></param>
        public void Add(InputHandler nextHandler)
        {
            if (this.nextHandler == null)
                this.nextHandler = nextHandler;
            else
                this.nextHandler.Add(nextHandler);
        }

        protected abstract IEventInvoker[] HandleInput();

        public void Dispose()
        {
            nextHandler?.Dispose();
            nextHandler = null;
        }
    }

    /// <summary>
    /// Handles shortcut inputs from keyboard.
    /// </summary>
    public class ShortcutHandler : InputHandler
    {
        public ShortcutHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        protected override IEventInvoker[] HandleInput()
        {
            if (Input.anyKeyDown)
            {
                List<KeyCode> keys = InputManager.UnityInputs.KeysPressed.ToList();
                List<IEventInvoker> result = new List<IEventInvoker>();
                //CTRL
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                {
                    keys.RemoveAll(x => (x == KeyCode.LeftControl || x == KeyCode.LeftControl || x == KeyCode.None));
                    keys = new List<KeyCode>(keys.Distinct());

                    result.Add(new CTRL_CombinationCommand(new EventRegistery.KeyboardInputData(keys, this)));
                }

                //ALT..
                if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                {
                    keys.RemoveAll(x => (x == KeyCode.LeftControl || x == KeyCode.LeftControl || x == KeyCode.None));
                    keys = new List<KeyCode>(keys.Distinct());

                    result.Add(new ALT_CombinationCommand(new EventRegistery.KeyboardInputData(keys, this)));
                }

                return result.ToArray();
            }

            return null;
        }

    }

    /// <summary>
    /// Handles look direction inputs
    /// </summary>
    public class LookDirectionDiscreateHandler : InputHandler
    {
        private LayerMask layer = default(LayerMask);
        private string tag = "";
        public LookDirectionDiscreateHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        public LookDirectionDiscreateHandler(InputHandler nextHandler, LayerMask layer = default(LayerMask),
            string tag = "") : base(nextHandler)
        {
            this.layer = layer;
            this.tag = tag;
        }

        public LookDirectionDiscreateHandler(InputHandler nextHandler,LayerMask layer = default(LayerMask)) : base(nextHandler)
        {
            this.layer = layer;
            this.tag = "";
        }
        public LookDirectionDiscreateHandler(InputHandler nextHandler,string tag="") : base(nextHandler)
        {
            this.layer = default(LayerMask);
            this.tag = tag;
        }


        protected override IEventInvoker[] HandleInput()
        {
            //List<Tuple<GazeInteractable, bool>> focusActions = InputManager.GazeInputs.FocusActions.ToList();

            //if (focusActions != null && focusActions.Count>0)

            IEventInvoker eyeFocusCmd = null;
            if (InputManager.GazeInputs.FocusAction != null)
            {
                if (layer != default(LayerMask) && InputManager.GazeInputs.FocusAction.Item1.gameObject.layer != layer) return null;
                if (tag != "" && InputManager.GazeInputs.FocusAction.Item1.gameObject.tag != tag) return null;

                if (InputManager.GazeInputs.FocusAction.Item2)
                {
                    if (InputManager.GazeInputs.EyeHit.point != Vector3.zero)
                        eyeFocusCmd = new EyeFocusCommand(new EventRegistery.EyeData(InputManager.GazeInputs.EyeHit,
                            InputManager.GazeInputs.FocusAction.Item1));
                }
                else
                {
                    if (InputManager.GazeInputs.LastEyeHit.point != Vector3.zero)
                        eyeFocusCmd = new EyeFocusEndCommand(new EventRegistery.EyeData(InputManager.GazeInputs.LastEyeHit,
                            InputManager.GazeInputs.FocusAction.Item1));
                }

                InputManager.GazeInputs.ClearFocusActions();
            }

            return new IEventInvoker[] {eyeFocusCmd};
        }

    }    
    
    /// <summary>
    /// Handles look direction inputs
    /// </summary>
    public class LookDirectionContiunousHandler : InputHandler
    {
        public LookDirectionContiunousHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        private GameObject lastFocusedObj = null;
        protected override IEventInvoker[] HandleInput()
        {
            IEventInvoker eyeHoverCmd = null;
            if (InputManager.GazeInputs.EyeHit.transform!=null)
            {
                lastFocusedObj = InputManager.GazeInputs.EyeHit.transform.gameObject;
                eyeHoverCmd = new EyeHoverCommand(new EventRegistery.EyeData(InputManager.GazeInputs.EyeHit));
            }
            else if(lastFocusedObj!=null)
            {
                lastFocusedObj = null;
                eyeHoverCmd = new EyeHoverEndCommand(new EventRegistery.EyeData(default(RaycastHit)));
            }

            return new IEventInvoker[]{ eyeHoverCmd };
        }

    }

    /// <summary>
    /// Uses SteamVR SDK and handles Vive inputs.
    /// </summary>
    public class ViveDiscreateHandler : InputHandler
    {
        public ViveDiscreateHandler( InputHandler nextHandler) : base( nextHandler){}

        protected override IEventInvoker[] HandleInput()
        {
            //Handle controller events
            foreach (Hand h in InputManager.ViveInputs.hands)
            {
                if (h != null)
                    if (h.controller != null)
                    {
                        List<IEventInvoker> viveCmd = new List<IEventInvoker>();
                        if (h.controller.GetPressUp(EVRButtonId.k_EButton_Grip))
                        {
                            viveCmd.Add(new ViveButtonUpCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_Grip, h.controller.GetAxis(EVRButtonId.k_EButton_Grip))));
                        }

                        if (h.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
                        {
                            viveCmd.Add(new ViveButtonDownCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_Grip, h.controller.GetAxis(EVRButtonId.k_EButton_Grip))));
                        }

                        if (h.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                        {
                            viveCmd.Add(new ViveButtonUpCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_SteamVR_Trigger,
                                h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger))));
                        }

                        if (h.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
                        {
                            viveCmd.Add( new ViveButtonDownCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_SteamVR_Trigger,
                                h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger))));
                        }

                        if (h.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad))
                        {
                            viveCmd.Add( new ViveButtonUpCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_SteamVR_Touchpad,
                                h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad))));
                        }

                        if (h.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
                        {
                            viveCmd.Add( new ViveButtonDownCommand(new EventRegistery.ViveInputData(h.controller,
                                EVRButtonId.k_EButton_SteamVR_Touchpad,
                                h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad))));
                        }

                        return viveCmd.ToArray();
                    }
            }

            return null;
        }
    }

    /// <summary>
    /// Aggressive Inputs for Vive
    /// </summary>
    public class ViveContiunousHandler : InputHandler
    {
        public ViveContiunousHandler( InputHandler nextHandler) : base( nextHandler) { }

        protected override IEventInvoker[] HandleInput()
        {
            //Handle controller events
            foreach (Hand h in InputManager.ViveInputs.hands)
            {
                if (h != null && h.controller != null)
                {
                    List<IEventInvoker> viveCmd = new List<IEventInvoker>();
                    if (h.controller.GetPress(EVRButtonId.k_EButton_Grip))
                    {
                        viveCmd.Add( new ViveButtonCommand(new EventRegistery.ViveInputData(h.controller,
                            EVRButtonId.k_EButton_Grip, h.controller.GetAxis(EVRButtonId.k_EButton_Grip))));
                    }

                    if (h.controller.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        viveCmd.Add(new ViveButtonCommand(new EventRegistery.ViveInputData(h.controller,
                            EVRButtonId.k_EButton_SteamVR_Trigger,
                            h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger))));
                    }

                    if (h.controller.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad))
                    {
                        viveCmd.Add( new ViveButtonCommand(new EventRegistery.ViveInputData(h.controller,
                            EVRButtonId.k_EButton_SteamVR_Touchpad,
                            h.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad))));
                    }

                    return viveCmd.ToArray();
                }
            }

            return null;
        }
    }

    public class LeapComponentHandler : InputHandler
    {
        public LeapComponentHandler( InputHandler nextHandler) : base( nextHandler)
        {
        }

        protected override IEventInvoker[] HandleInput()
        {
            //if (LeapInputs.Instance.HorizontalSlideList?.Count > 0)
            //    return new LeapComponentTriggerCommand(new EventRegistery.LeapInputSliderComponentData(
            //        LeapInputs.Instance.HorizontalSlideList.Last(), this,
            //        EventRegistery.LeapInputSliderComponentData.LeapSliderState.HorizontalSwipe));


            //if (LeapInputs.Instance.VerticalSlideList?.Count > 0)
            //    return new LeapComponentTriggerCommand(new EventRegistery.LeapInputSliderComponentData(
            //        LeapInputs.Instance.VerticalSlideList.Last(), this,
            //        EventRegistery.LeapInputSliderComponentData.LeapSliderState.VerticalSwipe));

            //if (LeapInputs.Instance.PressList?.Count > 0)
            //    return new LeapComponentTriggerCommand(new EventRegistery.LeapInputButtonComponentData(
            //        LeapInputs.Instance.PressList.Last(), this,
            //        EventRegistery.LeapInputButtonComponentData.LeapButtonState.Press));


            //if (LeapInputs.Instance.UnpressList?.Count > 0)
            //    return new LeapComponentTriggerCommand(new EventRegistery.LeapInputSliderComponentData(
            //        LeapInputs.Instance.UnpressList.Last(), this,
            //        EventRegistery.LeapInputSliderComponentData.LeapSliderState.VerticalSwipe));
            return null;
        }
    }


    public class LeapContactHandler : InputHandler
    {
        public LeapContactHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        protected override IEventInvoker[] HandleInput()
        {
            //if (LeapInputs.Instance.ContactBeginList?.Count > 0)
            //    return new LeapContactCommand(new EventRegistery.LeapInputContactData(
            //        LeapInputs.Instance.ContactBeginList.Last(), this,
            //        EventRegistery.LeapInputContactData.LeapContactType.Begin));

            //if (LeapInputs.Instance.ContactEndList?.Count > 0)
            //    return new LeapContactCommand(new EventRegistery.LeapInputContactData(LeapInputs.Instance.ContactEndList.Last(), this,
            //        EventRegistery.LeapInputContactData.LeapContactType.End));

            //if (LeapInputs.Instance.ContactStayList?.Count > 0)
            //    return new LeapContactCommand(new EventRegistery.LeapInputContactData(LeapInputs.Instance.ContactStayList.Last(),this,
            //        EventRegistery.LeapInputContactData.LeapContactType.Stay));

            return null;
        }
    }



    public class LeapGestureHandler : InputHandler
    {
        public LeapGestureHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }
        protected override IEventInvoker[] HandleInput()
        {
            if (LeapInputs.Instance.BeginFacingCamera)
                return new IEventInvoker[]{new LeapGestureCommand(new EventRegistery.LeapInputGestureData(null, this,
                    EventRegistery.LeapInputGestureData.LeapGestureType.BeginFacingCamera))};

            if (LeapInputs.Instance.EndFacingCamera)
                return new IEventInvoker[]{new LeapGestureCommand(new EventRegistery.LeapInputGestureData(null, this,
                    EventRegistery.LeapInputGestureData.LeapGestureType.EndFacingCamera))};
            
            return null;
        }
    }



    public class JoystickHandler : InputHandler
    {
        public JoystickHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        protected override IEventInvoker[] HandleInput()
        {
            if (UnityInputs.Instance.JoystickInputs != null && UnityInputs.Instance.JoystickInputs.Count > 0)
            {
                return new IEventInvoker[] {new JoystickCommand(new EventRegistery.JoystickData(UnityInputs.Instance.JoystickInputs.ToList(),this))};
            }
            return null;
        }
    }
    public class JoystickAxisHandler : InputHandler
    {
        public JoystickAxisHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        protected override IEventInvoker[] HandleInput()
        {
            List<IEventInvoker> axisInputComd = new List<IEventInvoker>();
            if (UnityInputs.Instance.ThrottleInputs != null && UnityInputs.Instance.ThrottleInputs.Count > 0)
            {
                axisInputComd.Add(new JoystickAxisCommand(
                    new EventRegistery.JoystickAxisThrottleData(UnityInputs.Instance.ThrottleInputs.ToList(), this)));
            }

            if (UnityInputs.Instance.StickInputs != null && UnityInputs.Instance.StickInputs.Count > 0)
            {
                axisInputComd.Add(new JoystickAxisCommand(
                    new EventRegistery.JoystickAxisStickData(UnityInputs.Instance.StickInputs.ToList(), this)));
            }

            return axisInputComd.ToArray();
        }
    }






    public class CollisionHandler : InputHandler
    {
        private string tag="", collidedObjTag="";
        public CollisionHandler(InputHandler nextHandler) : base(nextHandler)
        {
        }

        public CollisionHandler(InputHandler nextHandler, string tag="", string collidedObjTag="") : base(nextHandler)
        {
            this.tag = tag;
            this.collidedObjTag = collidedObjTag;
        }

        protected override IEventInvoker[] HandleInput()
        {


            if (UnityInputs.Instance.CollisionListUpdated)
            {
                //if (tag != "" || collidedObjTag != "")
                //{

                //    List<Tuple<GameObject,GameObject>> interestedPairs=new List<Tuple<GameObject, GameObject>>();
                //    foreach (var pair in UnityInputs.Instance.CollidedObjects)
                //    {
                //        if ((tag == "" ? true : pair.Item1.tag == tag) &&
                //            (collidedObjTag == "" ? true : pair.Item2.tag == collidedObjTag))
                //            interestedPairs.Add(pair);
                //    }

                //    return new CollisionCommand(new EventRegistery.CollisionData(interestedPairs, this));
                //}



                return new IEventInvoker[]{new CollisionCommand(new EventRegistery.CollisionData(UnityInputs.Instance.CollidedObjects, this))};
            }

            return null;
            
        }
    }

}