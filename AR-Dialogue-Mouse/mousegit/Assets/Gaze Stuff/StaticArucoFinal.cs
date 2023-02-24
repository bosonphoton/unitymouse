using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class StaticArucoFinal : MonoBehaviour
{
    private string publication_id;

    //public GameObject image1; //aruco marker initialization
    //public GameObject eyes;

    public GameObject red1, blue1, red2, blue2, table1, table2, target;
    public GameObject projectedred1, projectedblue1, projectedred2, projectedblue2;

    private GameObject initializedred1, initializedred2, initializedblue1, initializedblue2, initializedtable1, initializedtable2, initializedtarget;
    private GameObject initializedprojectedred1, initializedprojectedred2, initializedprojectedblue1, initializedprojectedblue2;
    private GameObject first_object;
    //AR representation objects


    private List<GameObject> allCubes = new List<GameObject>();
    private List<GameObject> objects_at_target = new List<GameObject>();
    private List<GameObject> objects_not_at_target = new List<GameObject>();

    public Material table1mat,table2mat, targetmat;
    public Material detectedMaterial, lockedMaterial;

    public List<Material> allMaterials = new List<Material>();

    public float startTime = 0f;
    public float timetoLock = 3f; // 5 seconds to lock in gaze

    bool execute2 = true; //for moveobject2
    bool execute3 = true; //moveobject3

    bool getaruco = true;
    bool desiredObjectHighlighted = false;
    bool desiredTargetHighlighted = false;
    bool targetLocked = false;
    bool objectLocked = false;
    bool moveObject1 = false;
    bool moveObject2 = false;
    bool moveObject3 = false;
    bool targetplaced1 = false; //if first object placed
    bool targetplaced2 = false; //if second object placed
    bool targetplaced3 = false; //if third object placed

    bool projectred1 = false; //change when you get policy 
    bool projectblue1 = false;
    bool projectred2 = false;
    bool projectblue2 = false;

    string lastObject = "";
    string current_object = "";
    string current_target = "";
    string last_target = "";
    float lockGazeNow = 3f;
    float lockTargetNow = 3f;
    float speed = 0.4f;
    float count = 0; // stores the number of objects moved
    int current_object_to_move = -1;

    Vector3 image_position1;
    Vector3 position_red1, position_red2, position_blue1, position_blue2, position_table1, position_table2, position_target;
    Vector3 drop_pos2, drop_pos3; //positions to drop from
    Vector3 newpos1, newpos2, newpos3; //positions after dropping


    private Rigidbody first_rb, second_rb, third_rb;


    // Raycast variables
    Camera cam;
    public LayerMask mask;

    Vector3 eyes_pos;



    // Start is called before the first frame update
    void Start()
    {

        initializedred1 = Instantiate(red1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedred1.gameObject.name = "red1";
        initializedred2 = Instantiate(red2, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedred2.gameObject.name = "red2";
        initializedblue1 = Instantiate(blue1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedblue1.gameObject.name = "blue1";
        initializedblue2 = Instantiate(blue2, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedblue2.gameObject.name = "blue2";
        initializedtable1 = Instantiate(table1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedtable1.gameObject.name = "table1";
        initializedtable2 = Instantiate(table2, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedtable2.gameObject.name = "table2";
        initializedtarget = Instantiate(target, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedtarget.gameObject.name = "target";


        /*        allCubes.Add(initializedred1);
                allCubes.Add(initializedred2);
                allCubes.Add(initializedblue1);
                allCubes.Add(initializedblue2);*/
        allCubes.Add(initializedtable1);
        allCubes.Add(initializedtable2);
        allCubes.Add(initializedtarget);


        allMaterials.Add(table1mat);
        allMaterials.Add(table2mat);
        allMaterials.Add(targetmat);


        lockGazeNow = Time.time + 100f;
        lockTargetNow = Time.time + 100f;

        initializedprojectedred1 = Instantiate(projectedred1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedprojectedred1.gameObject.name = "projectedred1";
        initializedprojectedred2 = Instantiate(projectedred2, new Vector3(0, 0, 0), Quaternion.identity);
        initializedprojectedred2.gameObject.name = "projectedred2";
        initializedprojectedblue1 = Instantiate(projectedblue1, new Vector3(0, 0, 0), Quaternion.identity);
        initializedprojectedblue1.gameObject.name = "projectedblue1";
        initializedprojectedblue2 = Instantiate(projectedblue2, new Vector3(0, 0, 0), Quaternion.identity);
        initializedprojectedblue2.gameObject.name = "projectedblue2";


        objects_not_at_target.Add(initializedprojectedred1);
        objects_not_at_target.Add(initializedprojectedred2);
        objects_not_at_target.Add(initializedprojectedblue1);
        objects_not_at_target.Add(initializedprojectedblue2);


        image_position1 = new Vector3();


        cam = Camera.main;

        eyes_pos = new Vector3(); //gets eye gaze position 


    }




    public int GetIndexOfLowestValue(float[] arr) //definition GetIndexOfLowestValue
    {
        float value = float.PositiveInfinity;
        int index = -1;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] < value)
            {
                index = i;
                value = arr[i];
            }
        }
        return index;

    }




    // Update is called once per frame
    void Update()
    {

        //if (getaruco == true)
        //{
        //    image_position1 = image1.transform.position;
        //    if (image_position1 != Vector3.zero)
        //    {
        //        getaruco = false;
        //    }
        //}

        /////////////////////disable rigid bodies when close//////////////////////////////////
        float red1_dist, red2_dist, blue1_dist, blue2_dist;
        float thres = 0.2f;

        red1_dist = Vector3.Distance(initializedprojectedred1.transform.position, initializedred1.transform.position);
        red2_dist = Vector3.Distance(initializedprojectedred2.transform.position, initializedred2.transform.position);
        blue1_dist = Vector3.Distance(initializedprojectedblue1.transform.position, initializedblue1.transform.position);
        blue2_dist = Vector3.Distance(initializedprojectedblue2.transform.position, initializedblue2.transform.position);

        if (red1_dist < thres)
        {
            Rigidbody rb;
            rb = initializedprojectedred1.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        else
        {
            Rigidbody rb;
            rb = initializedprojectedred1.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (red2_dist < thres)
        {
            Rigidbody rb;
            rb = initializedprojectedred2.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        else
        {
            Rigidbody rb;
            rb = initializedprojectedred2.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (blue1_dist < thres)
        {
            Rigidbody rb;
            rb = initializedprojectedblue1.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        else
        {
            Rigidbody rb;
            rb = initializedprojectedblue1.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
        if (blue2_dist < thres)
        {
            Rigidbody rb;
            rb = initializedprojectedblue2.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        else
        {
            Rigidbody rb;
            rb = initializedprojectedblue2.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }




        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position,
        Color.blue);


        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            Debug.Log(hit.transform.name);
            eyes_pos = hit.transform.position; //gets eye gaze position 
            //hit.transform.GetComponent<Renderer>().material.color =
            //detectedMaterial.color;

        }


        for (int i = 0; i < objects_not_at_target.Count; i++) //keep these in place
        {

            if (objects_not_at_target[i].name == "projectedred1")
            {
                objects_not_at_target[i].transform.position = position_red1;
            }

            if (objects_not_at_target[i].name == "projectedred2")
            {
                objects_not_at_target[i].transform.position = position_red2;
            }

            if (objects_not_at_target[i].name == "projectedblue1")
            {
                objects_not_at_target[i].transform.position = position_blue1;
            }

            if (objects_not_at_target[i].name == "projectedblue2")
            {
                objects_not_at_target[i].transform.position = position_blue2;
            }
        }



        /////////////////////////////////////////////////////////////////At the start of every update, we check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////

        /*        if (objects_at_target.Count == 1)
                {
                    objects_at_target[0].transform.position = newpos1;
                }

                if (objects_at_target.Count == 2)
                {
                    objects_at_target[0].transform.position = newpos1;
                    objects_at_target[1].transform.position = newpos2;
                }

                if (objects_at_target.Count == 3)
                {
                    objects_at_target[0].transform.position = newpos1;
                    objects_at_target[1].transform.position = newpos2;
                    objects_at_target[2].transform.position = newpos2;
                }
        */

        /////////////////////////////////////////////////////////////////First must check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////


        position_table1 = image_position1; //static positions
        position_table1.x = position_table1.x - 0.3f;
        position_table2 = image_position1;
        position_table2.x = position_table2.x + 0.3f;
        position_red1 = image_position1; //static positions
        position_red1.x = position_red1.x - 0.4f;
        position_red1.y = position_red1.y + 0.026f;
        position_blue1 = image_position1;
        position_blue1.x = position_blue1.x - 0.2f;
        position_blue1.y = position_blue1.y + 0.026f;
        position_red2 = image_position1;
        position_red2.x = position_red2.x + 0.4f;
        position_red2.y = position_red2.y + 0.036f;
        position_blue2 = image_position1;
        position_blue2.x = position_blue2.x + 0.2f;
        position_blue2.y = position_blue2.y + 0.036f;
        position_target = image_position1;



        initializedtable1.transform.position = position_table1;
        initializedtable2.transform.position = position_table2;
        initializedred1.transform.position = position_red1;
        initializedblue1.transform.position = position_blue1;
        initializedred2.transform.position = position_red2;
        initializedblue2.transform.position = position_blue2;
        initializedtarget.transform.position = position_target;


        float distancetable1, distancetable2;

        distancetable1 = Vector3.Distance(position_table1, eyes_pos);  //finds the distnace between the table and the eyegaze position
        distancetable2 = Vector3.Distance(position_table2, eyes_pos);



        float[] cubeDistances = new float[2]; //initializes the variable cubeDistances with each index as the cube distance
        cubeDistances[0] = distancetable1;
        cubeDistances[1] = distancetable2;


        int closestCubeIndex = GetIndexOfLowestValue(cubeDistances);



        for (int i = 0; i < allCubes.Count; i++)
        {
            if (desiredObjectHighlighted == false)
            {

                if (i == closestCubeIndex)
                {
                    allCubes[i].GetComponent<MeshRenderer>().material = detectedMaterial;
                    current_object = allCubes[i].gameObject.name;


                    if (current_object == lastObject) // wont these always be the same?
                    {

                        Debug.Log("Locking time: " + lockGazeNow + "Current time:" + Time.time);

                        if (Time.time > lockGazeNow) //if the current time > past time + 5 seoncds
                        {
                            Debug.Log("Test3   " + i);
                            GameObject.Find(lastObject).GetComponent<MeshRenderer>().material = lockedMaterial;
                            objectLocked = true;
                            desiredObjectHighlighted = true;
                            //lockGazeNow = Time.time + timetoLock; //5 seconds from now

                            if (lastObject == "table1")
                            {
                                //send to ros;
                            }
                            if (lastObject == "table1")
                            {
                                //send to ros;
                            }

                        }
                    }
                    else
                    {
                        lockGazeNow = Time.time + timetoLock;
                        lastObject = current_object;
                    }
                }
                else
                {
                    allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i]; //turns game object materials back to original color
                }

                Debug.Log("Current object name:" + current_object + " :last object" + lastObject);


            }
        }

        /////////////////////////////////////////////////////    Code to lock target highlight   ////////////////////////////////////////////////////////
        if (objectLocked)
        {
            float distancefromTarget = Vector3.Distance(initializedtarget.transform.position, eyes_pos);  //distance from target to gaze

            if (desiredTargetHighlighted == false)
            {
                if (distancefromTarget < 2f)
                {
                    initializedtarget.GetComponent<MeshRenderer>().material = detectedMaterial;
                    current_target = initializedtarget.gameObject.name;
                }
                else
                {
                    initializedtarget.GetComponent<MeshRenderer>().material = targetmat; //turns target back to original color
                    last_target = "";
                }

                if (current_target == last_target)
                {
                    if (Time.time > lockTargetNow)
                    {
                        initializedtarget.GetComponent<MeshRenderer>().material = lockedMaterial;
                        targetLocked = true;
                        desiredTargetHighlighted = true;
                        lockTargetNow = Time.time + timetoLock;

                    }

                }
                else
                {
                    lockTargetNow = Time.time + timetoLock;
                    last_target = current_target;
                }

            }

        }


        /////////////////////////////////////////////////////////////Code for which Object to move based on count /////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        if (projectred1)
        {
            Debug.Log("MOVING RED1 OBJECT");
            objects_not_at_target.Remove(initializedprojectedred1); //must remove from list so object can indeed move
            first_object = initializedprojectedred1;
            first_rb = first_object.GetComponent<Rigidbody>();
            first_rb.MovePosition(first_rb.transform.position + position_target * Time.deltaTime * speed);
            first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, position_target, speed * Time.deltaTime);
            Debug.Log("First Object Position " + first_object.transform.position + position_target);


            float distance_to_target = Vector3.Distance(first_object.transform.position, position_target);


            //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
            if (distance_to_target < 0.01)
            {
                newpos1 = first_object.transform.position;
                if (!objects_at_target.Contains(initializedprojectedred1)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedred1);
                    targetplaced1 = true;

                }
            }

        }

        if (targetplaced1 == true)
        {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop

            Debug.Log("TARGETPLACED1");
            moveObject1 = false;
            startTime = 0f;
            timetoLock = 3f;
            lockGazeNow = 3f;
            lockTargetNow = 3f;
            lastObject = "";
            current_object = "";
            current_target = "";
            last_target = "";
            current_object_to_move = -1;
            desiredTargetHighlighted = false;
            targetLocked = false;
            objectLocked = false;
            desiredObjectHighlighted = false;


            targetplaced1 = false;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////MOVE OBJECT 2///////////////////////////////////////////////////////////////////////////////////////

        if (projectblue1)
        {
            Debug.Log("MOVING BLUE1 OBJECT");
            objects_not_at_target.Remove(initializedprojectedblue1); //must remove from list so object can indeed move
            first_object = initializedprojectedblue1;
            first_rb = first_object.GetComponent<Rigidbody>();
            first_rb.MovePosition(first_rb.transform.position + position_target * Time.deltaTime * speed);
            first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, position_target, speed * Time.deltaTime);
            Debug.Log("First Object Position " + first_object.transform.position + position_target);


            float distance_to_target = Vector3.Distance(first_object.transform.position, position_target);


            //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
            if (distance_to_target < 0.01)
            {
                newpos1 = first_object.transform.position;
                if (!objects_at_target.Contains(initializedprojectedblue1)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedblue1);
                    targetplaced1 = true;

                }
            }

        }

        if (targetplaced1 == true)
        {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop

            Debug.Log("TARGETPLACED1");
            moveObject1 = false;
            startTime = 0f;
            timetoLock = 3f;
            lockGazeNow = 3f;
            lockTargetNow = 3f;
            lastObject = "";
            current_object = "";
            current_target = "";
            last_target = "";
            current_object_to_move = -1;
            desiredTargetHighlighted = false;
            targetLocked = false;
            objectLocked = false;
            desiredObjectHighlighted = false;


            targetplaced1 = false;

        }

        if (projectred2)
        {
            Debug.Log("MOVING RED2 OBJECT");
            objects_not_at_target.Remove(initializedprojectedred2); //must remove from list so object can indeed move
            first_object = initializedprojectedred2;
            first_rb = first_object.GetComponent<Rigidbody>();
            first_rb.MovePosition(first_rb.transform.position + position_target * Time.deltaTime * speed);
            first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, position_target, speed * Time.deltaTime);
            Debug.Log("First Object Position " + first_object.transform.position + position_target);


            float distance_to_target = Vector3.Distance(first_object.transform.position, position_target);


            //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
            if (distance_to_target < 0.01)
            {
                newpos1 = first_object.transform.position;
                if (!objects_at_target.Contains(initializedprojectedred2)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedred2);
                    targetplaced1 = true;

                }
            }

        }

        if (targetplaced1 == true)
        {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop

            Debug.Log("TARGETPLACED1");
            moveObject1 = false;
            startTime = 0f;
            timetoLock = 3f;
            lockGazeNow = 3f;
            lockTargetNow = 3f;
            lastObject = "";
            current_object = "";
            current_target = "";
            last_target = "";
            current_object_to_move = -1;
            desiredTargetHighlighted = false;
            targetLocked = false;
            objectLocked = false;
            desiredObjectHighlighted = false;


            targetplaced1 = false;

        }

        if (projectblue2)
        {
            Debug.Log("MOVING BLUE2 OBJECT");
            objects_not_at_target.Remove(initializedprojectedblue2); //must remove from list so object can indeed move
            first_object = initializedprojectedblue2;
            first_rb = first_object.GetComponent<Rigidbody>();
            first_rb.MovePosition(first_rb.transform.position + position_target * Time.deltaTime * speed);
            first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, position_target, speed * Time.deltaTime);
            Debug.Log("First Object Position " + first_object.transform.position + position_target);


            float distance_to_target = Vector3.Distance(first_object.transform.position, position_target);


            //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
            if (distance_to_target < 0.01)
            {
                newpos1 = first_object.transform.position;
                if (!objects_at_target.Contains(initializedprojectedblue2)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedblue2);
                    targetplaced1 = true;

                }
            }

        }

        if (targetplaced1 == true)
        {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop

            Debug.Log("TARGETPLACED1");
            moveObject1 = false;
            startTime = 0f;
            timetoLock = 3f;
            lockGazeNow = 3f;
            lockTargetNow = 3f;
            lastObject = "";
            current_object = "";
            current_target = "";
            last_target = "";
            current_object_to_move = -1;
            desiredTargetHighlighted = false;
            targetLocked = false;
            objectLocked = false;
            desiredObjectHighlighted = false;


            targetplaced1 = false;

        }
    }

}