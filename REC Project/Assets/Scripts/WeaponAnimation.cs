using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this method is not currently used.
//It involved some exploratory work to include a weapon item with my Mixamo animation
//Ultimately proved a step too far at this stage, but I'd like to return to it

public class WeaponAnimation : MonoBehaviour
{


    Animator anim;

    public float lhweight = 1;

    public Transform lhtarget;



    //public float rhweight = 1;

    //public Transform rhtarget;



    public Transform weapon;

    public Vector3 lookpos;



    void Start()

    {

        anim = GetComponent<Animator>();
        weapon.transform.Rotate(-95, 250, -90);
        Vector3 temp = new Vector3(0.088f,0.219f,-0.183f);
        temp= weapon.transform.position;
        temp.x=temp.x +1.0f;

        weapon.transform.position = temp;
    }


    // Update is called once per frame

    void Update()

    {



    }

    void OnAnimatorIk()

    {

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, lhweight);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, lhtarget.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, lhweight);

        anim.SetIKRotation(AvatarIKGoal.LeftHand, lhtarget.rotation);



        //anim.SetIKPositionWeight (AvatarIKGoal.RightHand, rhweight);

        //anim.SetIKPosition (AvatarIKGoal.RightHand, rhtarget.position);

        //anim.SetIKRotationWeight (AvatarIKGoal.RightHand, rhweight);

        //anim.SetIKRotation(AvatarIKGoal.RightHand, rhtarget.rotation);





    }

}
