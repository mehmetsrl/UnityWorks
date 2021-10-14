using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NoteChanger : MonoBehaviour
{
    [System.Serializable]
    public class Fret
    {
        public Transform[] wires = new Transform[6];
        public Transform handPlace,handDirectionPlace;
    }

    [System.Serializable]
    public class Note{
        public ushort fret;
        public ushort wire;
        public override string ToString()
        {
            return " | fret: " + fret + " wire: " + wire;
        }
    }

    private NoteChanger instance;
    public NoteChanger Instance
    {
        get { return instance; }
        private set { 
            if(instance!=null)
                instance = value;
            else
                Destroy(value);
        }
    }

    //public RigBuilder rig;
    //public TwoBoneIKConstraint leftHandPositionCons;
    //public MultiAimConstraint leftHandLookCons;
    //public ChainIKConstraint leftIndexFingerIKCons;
    //public TwoBoneIKConstraint leftIndexFinger2BoneIKCons;

    public Transform notePointTarget,handPosTarget;

    public ushort rigUpdateTime = 10;
    public float speed;
    public List<Note> notes;
    public List<Note> testNotes;
    public List<Fret> frets = new List<Fret>();


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Queue<Note> song=new Queue<Note>(notes);
            StartCoroutine(StartPlay(song,speed));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Queue<Note> song = new Queue<Note>(testNotes);
            StartCoroutine(StartPlay(song, speed));
        }
    }


    IEnumerator StartPlay(Queue<Note> song, float speed)
    {
        if (song.Count == 0)
        {
            //rig.layers[0].rig.weight = 1;
            yield return null;
        }
        else
        {
            //rig.layers[0].rig.weight = 0;
            AnimateToNote(song.Dequeue());
            yield return new WaitForSeconds(speed);
            StartCoroutine(StartPlay(song, speed));
        }
    }

    Transform parentOld;
    void TranslateTo(ref Transform to, ref Transform from)
    {
        parentOld = to.parent;
        to.parent = from;
        to.transform.localPosition = Vector3.zero;
        to.transform.localRotation = Quaternion.identity;
        to.transform.parent = parentOld;
    }

    void AnimateToNote(Note note)
    {
        TranslateTo(ref handPosTarget, ref frets[note.fret].handPlace);
        TranslateTo(ref notePointTarget, ref frets[note.fret].wires[note.wire]);


        ////Debug.Log("note pressed: "+note);

        ////Debug.Log("frets["+ note.fret + "].wires["+ note.wire+"]");
        //leftHandPositionCons.data.target = frets[note.fret].handPlace;
        ////leftHandLookCons.data.sourceObjects.SetTransform(0, frets[note.fret].handDirectionPlace);
        //leftIndexFingerIKCons.data.target = frets[note.fret].wires[note.wire];
        //leftIndexFinger2BoneIKCons.data.target = frets[note.fret].wires[note.wire];




        //StartCoroutine(UpdateRig(rigUpdateTime));
    }

    //IEnumerator UpdateRig(ushort updateTime)
    //{
    //    if (updateTime > 0)
    //    {
    //        rig.Build();

    //        yield return new WaitForEndOfFrame();
    //        StartCoroutine(UpdateRig(--updateTime));
    //    }
    //    else
    //    {
    //        //rig.layers[0].rig.weight = 1;
    //        yield return null;
    //    }

    //}

}
