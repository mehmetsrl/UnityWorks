using System;
using Boo.Lang;
using Leap.Unity;
using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    public class LeapInputs : MonoBehaviour {
        static LeapInputs _instance;

        public static LeapInputs Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        public Action<ExperimentComponent> OnContactBegin;
        public Action<ExperimentComponent> OnContactStay;
        public Action<ExperimentComponent> OnContactEnd;
        public Action<ExperimentComponent> OnPress;
        public Action<ExperimentComponent> OnUnpress;
        public Action<ExperimentComponent, float> OnHorizontalSlideEvent;
        public Action<ExperimentComponent, float> OnVerticalSlideEvent;
        public Action OnBeginFacingCamera, OnEndFacingCamera;

        [SerializeField]
        public List<ExperimentComponent> 
            ContactBeginList,
            ContactStayList,
            ContactEndList,
            PressList,
            UnpressList,
            HorizontalSlideList,
            VerticalSlideList;

        public bool BeginFacingCamera , EndFacingCamera ;
        //public bool BeginFacingCamera = false, EndFacingCamera=false;

        public HandModelManager ModelManager;
        void Awake()
        {
            Instance = this;

            //TODO
            OnContactBegin += (obj) => { ContactBeginList.Add(obj); };
            OnContactStay += (obj) => { ContactStayList.Add(obj); };
            OnContactEnd += (obj) => { ContactEndList.Add(obj); };

            OnPress += (obj) => { PressList.Add(obj); };
            OnUnpress += (obj) => { UnpressList.Add(obj); };

            OnHorizontalSlideEvent += (obj, amount) =>
            {
                HorizontalSlideList.Add(obj);
            };
            OnVerticalSlideEvent += (obj, amount) => { VerticalSlideList.Add(obj); };

            OnBeginFacingCamera += () => { BeginFacingCamera = true; };
            OnEndFacingCamera += () => { EndFacingCamera= true; };

        }

        void OnDestroy()
        {
            OnContactBegin = null;
            OnContactStay = null;
            OnContactEnd = null;
            OnPress = null;
            OnUnpress = null;

            OnHorizontalSlideEvent = null;
            OnVerticalSlideEvent = null;

            OnBeginFacingCamera = null;
            OnEndFacingCamera = null;
        }

        void LateUpdate()
        {

            ContactBeginList?.Clear();
            ContactStayList?.Clear();
            ContactEndList?.Clear();
            PressList?.Clear();
            UnpressList?.Clear();
            HorizontalSlideList?.Clear();
            VerticalSlideList?.Clear();


            ContactBeginList = new List<ExperimentComponent>();
            ContactStayList = new List<ExperimentComponent>();
            ContactEndList = new List<ExperimentComponent>();
            PressList = new List<ExperimentComponent>();
            UnpressList = new List<ExperimentComponent>();
            HorizontalSlideList = new List<ExperimentComponent> ();
            VerticalSlideList = new List<ExperimentComponent>();


            BeginFacingCamera = false;
            EndFacingCamera = false;
        }
    }
}
