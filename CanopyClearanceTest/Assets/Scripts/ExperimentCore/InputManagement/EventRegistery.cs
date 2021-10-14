using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Valve.VR;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Events holder and serializer class to host events.
    /// </summary>
    [System.Serializable]
    public class EventRegistery
    {
        /// <summary>
        /// Input inputData containers
        /// </summary>
        #region InputData

        public class InputData
        {
            public enum InputType
            {
                Keyboard,
                HTCViveController,
                Leap,
                PupilLab,
                ViewportGaze,
                Joystick,
                Collision
            }

            private InputType _type;

            public InputType Type
            {
                get { return _type; }
            }

            public InputData(InputType type)
            {
                _type = type;
            }
        }

        [System.Serializable]
        public class GazeInputData : InputData
        {
            public GazeInputData(Vector2 Pos2D, Vector3 Pos3D, GameObject interactableObject, InputHandler handler) : base(
                InputType.PupilLab)
            {
            }
        }

        [System.Serializable]
        public class ViveInputData : InputData
        {
            public SteamVR_Controller.Device device;
            public EVRButtonId button;
            public Vector2 axis;
            public ViveInputData(SteamVR_Controller.Device device) : base(InputType.HTCViveController)
            {
                this.device = device;
            }
            public ViveInputData(SteamVR_Controller.Device device, EVRButtonId button,Vector2 axis) : base(InputType.HTCViveController)
            {
                this.device = device;
                this.button = button;
                this.axis = axis;
            }
        }

        [System.Serializable]
        public class KeyboardInputData : InputData
        {
            public List<KeyCode> keys;
            public InputHandler handler;

            public KeyboardInputData(List<KeyCode> keys, InputHandler handler) : base(InputType.Keyboard)
            {
                this.keys = keys;
                this.handler = handler;
            }
        }

        [System.Serializable]
        public class JoystickData : InputData
        {
            public enum JoystickKeyCode
            {
                None=0,
                Fire=1
            }
            public List<int> keys;
            public InputHandler handler;

            public JoystickData(List<int> keys, InputHandler handler) : base(InputType.Joystick)
            {
                this.keys = keys;
                this.handler = handler;
            }
        }
        

        [System.Serializable]
        public class JoystickAxisData : InputData
        {
            public enum AxisType
            {
                Unspecified_1D,
                Unspecified_2D,
                Throttle,
                Stick
            }

            public AxisType type = AxisType.Unspecified_1D;
            public InputHandler handler;
            
            public JoystickAxisData(AxisType type, InputHandler handler) : base(InputType.Joystick)
            {
                this.type = type;
                this.handler = handler;
            }
        }



        [System.Serializable]
        public class JoystickAxisThrottleData : JoystickAxisData
        {
            public List<float> vals;
            public JoystickAxisThrottleData(List<float> vals, InputHandler handler) : base(JoystickAxisData.AxisType.Throttle,handler)
            {
                this.vals = vals;
            }
        }


        [System.Serializable]
        public class JoystickAxisStickData : JoystickAxisData
        {
            public List<Vector2> vals;
            public JoystickAxisStickData(List<Vector2> vals, InputHandler handler) : base(JoystickAxisData.AxisType.Stick, handler)
            {
                this.vals = vals;
            }
        }

        [System.Serializable]
        public class JoystickAxis1DData : JoystickAxisData
        {
            public List<float> vals;
            public JoystickAxis1DData(List<float> vals, InputHandler handler) : base(JoystickAxisData.AxisType.Unspecified_1D, handler)
            {
                this.vals = vals;
            }
        }

        [System.Serializable]
        public class JoystickAxis2DData : JoystickAxisData
        {
            public List<Vector2> vals;
            public JoystickAxis2DData(List<Vector2> vals, InputHandler handler) : base(JoystickAxisData.AxisType.Unspecified_2D, handler)
            {
                this.vals = vals;
            }
        }

        public class CollisionAttribute
        {
            public GameObject reporterObject,collidedObject;

            public CollisionAttribute(GameObject reporterObject, GameObject collidedObject)
            {
                this.reporterObject = reporterObject;
                this.collidedObject = collidedObject;
            }
        }

        [System.Serializable]
        public class CollisionData : InputData
        {
            public List<CollisionAttribute> collisionPairs;
            public InputHandler handler;

            public CollisionData(Dictionary<GameObject,GameObject> collisionPairs, InputHandler handler) : base(InputType.Collision)
            {
                this.collisionPairs = new List<CollisionAttribute>();
                foreach (var col in collisionPairs)
                {
                    this.collisionPairs.Add(new CollisionAttribute(col.Key, col.Value));
                }
                
                this.handler = handler;
            }
        }

        [System.Serializable]
        public class LeapInputData : InputData
        {
            public BaseLeapObject @object;
            public enum LeapAction
            {
                BasicContact,
                Gesture,
                Component
            }

            public LeapAction actionType = LeapAction.BasicContact;

            public LeapInputData(BaseLeapObject @object,  InputHandler handler, LeapAction actionType) : base(InputType.Leap)
            {
                this.@object = @object;
                this.actionType = actionType;
            }
        }
        [System.Serializable]
        public class LeapInputContactData : LeapInputData
        {
            public enum LeapContactType
            {
                Begin,
                Stay,
                End
            }

            public LeapContactType contactType = LeapContactType.Begin;

            public LeapInputContactData(BaseLeapObject @object, InputHandler handler, LeapContactType contactType) : base(@object, handler, LeapAction.BasicContact)
            {
                this.contactType = contactType;
            }
        }

        [System.Serializable]
        public class LeapInputGestureData : LeapInputData
        {
            public enum LeapGestureType
            {
                BeginFacingCamera,
                EndFacingCamera
            }

            public LeapGestureType gestureType = LeapGestureType.BeginFacingCamera;

            public LeapInputGestureData(BaseLeapObject @object, InputHandler handler, LeapGestureType gestureType) : base(@object, handler,LeapAction.Gesture)
            {
                this.gestureType = gestureType;
            }
        }

        [System.Serializable]
        public class LeapInputComponentData : LeapInputData
        {
            public enum LeapComponentType
            {
                Button,
                Slider
            }

            public LeapComponentType componentType = LeapComponentType.Button;

            public LeapInputComponentData(BaseLeapObject @object, InputHandler handler, LeapComponentType componentType) : base(@object, handler, LeapAction.Component)
            {
                this.componentType = componentType;
            }
        }

        [System.Serializable]
        public class LeapInputButtonComponentData : LeapInputComponentData
        {
            public enum LeapButtonState
            {
                Press,
                Unpress
            }

            public LeapButtonState buttonState = LeapButtonState.Press;

            public LeapInputButtonComponentData(BaseLeapObject @object, InputHandler handler, LeapButtonState buttonState) : base(@object, handler, LeapComponentType.Button)
            {
                this.buttonState = buttonState;
            }
        }



        [System.Serializable]
        public class LeapInputSliderComponentData : LeapInputComponentData
        {
            public enum LeapSliderState
            {
                HorizontalSwipe,
                VerticalSwipe
            }

            public LeapSliderState sliderState = LeapSliderState.HorizontalSwipe;

            public LeapInputSliderComponentData(BaseLeapObject @object, InputHandler handler, LeapSliderState sliderState) : base(@object, handler, LeapComponentType.Slider)
            {
                this.sliderState = sliderState;
            }
        }


        [System.Serializable]
        public class EyeData : InputData
        {
            public RaycastHit hit;

            public EyeData(RaycastHit hit, GazeInteractable gazeInteractableObject =null) : base(InputType.ViewportGaze)
            {
                this.hit = hit;
            }
        }

        #endregion

        public class ActionData
        {
            public GameObject gameObject;
            public ActionData(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }
        }

        public class TransformData : ActionData
        {
            public bool isActive;
            public TransformData(GameObject gameObject, bool isActive) : base(gameObject)
            {
                this.isActive = isActive;
            }
        }

        public class LabelData: ActionData
        {
            public string label;
            public LabelData(GameObject gameObject,string label):base(gameObject)
            {
                this.label = label;
            }
        }

        /// <summary>
        /// Unity event that represents InputData class
        /// </summary>
        [System.Serializable]
        public class KeyboardEventData : UnityEvent<KeyboardInputData>
        {
        }
        [System.Serializable]
        public class LeapEventContactEventData : UnityEvent<LeapInputContactData>
        {
        }
        [System.Serializable]
        public class LeapInputGestureEventData : UnityEvent<LeapInputGestureData>
        {
        }
        [System.Serializable]
        public class LeapInputComponentEventData : UnityEvent<LeapInputComponentData>
        {
        }
        [System.Serializable]
        public class ViveEventData : UnityEvent<ViveInputData>
        {
        }
        [System.Serializable]
        public class EyeEventData : UnityEvent<EyeData>
        {
        }
        [System.Serializable]
        public class JoystickEventData : UnityEvent<JoystickData>
        {
        }
        [System.Serializable]
        public class JoystickAxisEventData : UnityEvent<JoystickAxisData>
        {
        }

        [System.Serializable]
        public class CollisionEventData : UnityEvent<CollisionData>
        {
        }

        /// <summary>
        /// RootEvents shown in inspector
        /// </summary>
        #region EventsInIspector
        //Keyboard
        [FormerlySerializedAs("CTRL_CombinationCommand")]
        public KeyboardEventData ctrlCombinationEvent = new KeyboardEventData();
        [FormerlySerializedAs("ALT_CombinationCommand")]
        public KeyboardEventData altCombinationEvent = new KeyboardEventData();

        //Vive
        [FormerlySerializedAs("ViveButtonDownCommand")]
        public ViveEventData onViveButtonDown = new ViveEventData();
        [FormerlySerializedAs("ViveButtonCommand")]
        public ViveEventData onViveButton = new ViveEventData();
        [FormerlySerializedAs("ViveButtonUpCommand")]
        public ViveEventData onViveButtonUp = new ViveEventData();

        //Leap
        [FormerlySerializedAs("LeapContactCommand")]
        public LeapEventContactEventData onLeapContact = new LeapEventContactEventData();
        [FormerlySerializedAs("OnLeapGestureEvent")]
        public LeapInputGestureEventData onLeapGesture = new LeapInputGestureEventData();
        [FormerlySerializedAs("OnLeapComponentTriggered")]
        public LeapInputComponentEventData onLeapComponentTriggered = new LeapInputComponentEventData();

        //Eye
        [FormerlySerializedAs("OnEyeHover")]
        public EyeEventData onEyeHover = new EyeEventData();
        [FormerlySerializedAs("OnEyeHoverEnd")]
        public EyeEventData onEyeHoverEnd = new EyeEventData();
        [FormerlySerializedAs("OnEyeFocused")]
        public EyeEventData onEyeFocused = new EyeEventData();
        [FormerlySerializedAs("OnEyeFocusEnd")]
        public EyeEventData onEyeFocusEnd = new EyeEventData();


        [FormerlySerializedAs("OnJoystickDown")]
        public JoystickEventData onJoystickDown = new JoystickEventData();
        [FormerlySerializedAs("OnJoystickAxisChange")]
        public JoystickAxisEventData onJoystickAxisChange = new JoystickAxisEventData();


        [FormerlySerializedAs("OnCollisionOccour")]
        public CollisionEventData onCollisionOccour = new CollisionEventData();

        #endregion

        /// <summary>
        /// Events accessors for other classes
        /// </summary>
        #region Accesors

        public KeyboardEventData CTRL_CombinationEvent
        {
            get { return ctrlCombinationEvent; }
        }

        public KeyboardEventData ALT_CombinationEvent
        {
            get { return altCombinationEvent; }
        }

        //Vive
        public ViveEventData OnViveButton
        {
            get { return onViveButton; }
        }
        public ViveEventData OnViveButtonUp
        {
            get { return onViveButtonUp; }
        }
        public ViveEventData OnViveButtonDown
        {
            get { return onViveButtonDown; }
        }

        //Leap
        public LeapEventContactEventData OnLeapContact
        {
            get { return onLeapContact; }
        }
        public LeapInputComponentEventData OnLeapComponentTriggered
        {
            get { return onLeapComponentTriggered; }
        }
        public LeapInputGestureEventData OnLeapGesture
        {
            get { return onLeapGesture; }
        }

        //Eye
        public EyeEventData OnEyeHover
        {
            get { return onEyeHover; }
        }
        public EyeEventData OnEyeHoverEnd
        {
            get { return onEyeHoverEnd; }
        }
        public EyeEventData OnEyeFocused
        {
            get { return onEyeFocused; }
        }
        public EyeEventData OnEyeFocusEnd
        {
            get { return onEyeFocusEnd; }
        }
        public JoystickEventData OnJoystickDown
        {
            get { return onJoystickDown; }
        }
        public JoystickAxisEventData OnJoystickAxisChange
        {
            get { return onJoystickAxisChange; }
        }

        public CollisionEventData OnCollisionOccour
        {
            get { return onCollisionOccour; }
        }
        

        #endregion


    }
}