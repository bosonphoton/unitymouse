using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public GameObject object1,object2,object3,target,plane;
    public GameObject object1_T, object2_T, object3_T;
    float speed = 0.4f;
    Vector3 object1_p, object2_p, object3_p, target_p;

    void Start()
    {

        object1_p = object1.transform.position;
        object2_p = object2.transform.position;
        object3_p = object3.transform.position;
        target_p = target.transform.position;
       
        object1_T.transform.position = object1_p;
        object1_T.transform.position = object2_p;
        object1_T.transform.position = object3_p;

    }

    // Update is called once per frame
    void Update()
    {

        object1_T.transform.position = Vector3.MoveTowards(object1_p, target_p, speed * Time.deltaTime);
/*        Debug.Log("Speed" + speed);
        Debug.Log("object1_p" + object1_p);
        Debug.Log("target_p" + target_p);
        Debug.Log("transform" + object1.transform.position);*/



    }

}
