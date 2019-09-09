using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is the PocketRPG Animation Controller... It is necessary to run PocketRPG trails
// THIS REPLACES METHODS LIKE ANIMATION.PLAY AND ANIMATION.CROSSFADE... YOU CANNOT USE THOSE IN CONJUNCTION WITH THESE TRAILS UNLESS YOU ARE HAPPY WITH FRAMERATE DEPENDANT ANIMATION
// PocketRPG trails run faster than the framerate... (default is 300 frames a second)... that is how they are so smooth (30fps trails are rather jerky)
// This code was written by Evan Greenwood (of Free Lives) and used in the game PocketRPG by Tasty Poison Games.
// But you can use this how you please... Make great games... that's what this is about.
// This code is provided as is. Please discuss this on the Unity Forums if you need help.

[RequireComponent(typeof(Animation))]
[AddComponentMenu("PocketRPG/Animation Controller")]
public class AnimationController : MonoBehaviour
{

    private AnimationState currentState;
    private float currentStateTime = 0;
    private List<AnimationState> fadingStates;
    private float animationFadeTime = 0.15f;
    protected List<WeaponTrail> trails;
    protected float t = 0.033f;
    protected float m = 0;
    protected Vector3 lastEulerAngles = Vector3.zero;
    protected Vector3 lastPosition = Vector3.zero;
    protected Vector3 eulerAngles = Vector3.zero;
    protected Vector3 position = Vector3.zero;
    private float tempT = 0;
    public bool gatherDeltaTimeAutomatically = true;
    // ** You may want to set the deltaTime yourself for personal slow motion effects
    //
    protected float animationIncrement = 0.003f;
    // ** This sets the number of time the controller samples the animation for the weapon trails
    //
    //
    void Awake()
    {
        trails = new List<WeaponTrail>();
        fadingStates = new List<AnimationState>();
        currentState = null;
        GetComponent<Animation>().Stop();
        lastPosition = transform.position;
        lastEulerAngles = transform.eulerAngles;
        //
        GetComponent<Animation>().playAutomatically = false; // ** The Animation component must not be conflicting with the AnimationController
        GetComponent<Animation>().Stop(); 
        SetAnimationSampleRate(30);
    }

    void Start()
    {
        //animation.Stop(); // ** The Animation component must not be conflicting with the AnimationController

    }
    //
    public void SetDeltaTime(float deltaTime)
    {
        t = deltaTime; // ** This is for forcing the deltaTime of the Animation Controller for personal slow motion effects
    }

    public void SetAnimationSampleRate(int samplesPerSecond)
    {
        animationIncrement = 1f / samplesPerSecond;
    }
    //
    protected virtual void LateUpdate()
    {
        SampleTrail();
    }

    public void AddTrail(WeaponTrail trail)
    {
        trails.Add(trail);
    }

    private string curAniName = null;

    public void PlayAnimation(AnimationState state)
    {
        var aniName = state.name;
        /*
        if(state.weight != 0) {
            animation.PlayQueued(aniName);
        }else {
            curAniName = aniName;
        }
        */
        GetComponent<Animation>().CrossFade(state.name);
    }

    public void CrossfadeAnimation(AnimationState state, float fadeTime)
    {
        Log.Ani("currentState " + " " + state.name);
        /*
        var aniName = state.name;
        if(state.weight != 0){
            animation.PlayQueued(aniName);
        }else {
            curAniName = aniName;
            //animation.CrossFade (state.name);
        }
        */
        GetComponent<Animation>().CrossFade(state.name, fadeTime);
        return;
    }


    void SampleTrail()
    {

        t = Time.deltaTime;
        //首先计算当前位置
        //接着插值 三个点给trail position scale 和 rotation
        //WeaponTrail加入一个借口用于插入相关的采样点
        for (int j = 0; j < trails.Count; j++)
        {
            if (trails [j].time > 0)
            {
                //trails[j].Itterate (Time.time - t + tempT);
                trails [j].AddRefPoint(Time.time - t);
            } else
            {
                trails [j].ClearTrail();
            }
        }

        for (int j = 0; j < trails.Count; j++)
        {
            if (trails [j].time > 0)
            {
                trails [j].UpdateTrail(Time.time, t);
            }
        }

    }

	
}
