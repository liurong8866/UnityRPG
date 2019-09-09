using UnityEngine;
using System.Collections;

public class XftArractDemo : MonoBehaviour 
{
    public XffectComponent Effect;
    public Transform Target;
    
	void Start () 
    {
        //set Attraction goal and Collision goal here.
        Effect.SetArractionAffectorGoal(Target);
        Effect.SetCollisionGoalPos(Target);
	}
    
    //callback function.
    void OnCollision(Xft.CollisionParam param)
    {
        Debug.Log("collision detect!, object:" + param.CollideObject.name + " collide pos:" + param.CollidePos);
    }
    
    void OnGUI()
    {
        GUI.Label(new Rect(200, 0, 400, 25), "this demo demonstrate how to use AttractionAffector and Collision API.");
    }
}
