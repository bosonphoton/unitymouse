using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class StaticAruco : MonoBehaviour
{
    private string publication_id;
   
    //public GameObject image1; //aruco marker initialization
    //public GameObject eyes;

    public GameObject red, green, blue, plane, target;
    public GameObject projectedred, projectedgreen, projectedblue;

    private GameObject initializedred, initializedgreen, initializedblue, initializedplane, initializedtarget;
    private GameObject initializedprojectedred, initializedprojectedgreen, initializedprojectedblue;
    private GameObject first_object, second_object, third_object;
    //AR representation objects


    private List<GameObject> allCubes = new List<GameObject>();
    private List<GameObject> objects_at_target = new List<GameObject>();
    private List<GameObject> objects_not_at_target = new List<GameObject>();

    public Material cube1Mat, cube2Mat, cube3Mat, targetMat;
    public Material detectedMaterial, lockedMaterial;

    public List<Material> allMaterials = new List<Material>();

    public float startTime = 0f;
    public float timetoLock = 5f; // 5 seconds to lock in gaze

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
    string lastObject = "";
    string current_object = "";
    string current_target = "";
    string last_target = "";
    float lockGazeNow = 5f;
    float lockTargetNow = 5f;
    float speed = 0.4f;
    float count = 0; // stores the number of objects moved
    int current_object_to_move = -1;

    Vector3 image_position1;
    Vector3 position_red, position_green, position_blue, position_plane, targetbase;
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

        initializedred = Instantiate(red, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedred.gameObject.name = "red";
        initializedgreen = Instantiate(green, new Vector3(0, 0, 0), Quaternion.identity);
        initializedgreen.gameObject.name = "green";
        initializedblue = Instantiate(blue, new Vector3(0, 0, 0), Quaternion.identity);
        initializedblue.gameObject.name = "blue";
        initializedtarget = Instantiate(target, new Vector3(0, 0, 0), Quaternion.identity);
        initializedtarget.gameObject.name = "target";
        initializedplane = Instantiate(plane, new Vector3(0, 0, 0), Quaternion.identity);
        initializedplane.gameObject.name = "plane";

        


        allCubes.Add(initializedred);
        allCubes.Add(initializedgreen);
        allCubes.Add(initializedblue);
        allCubes.Add(initializedtarget);


        allMaterials.Add(cube1Mat);
        allMaterials.Add(cube2Mat);
        allMaterials.Add(cube3Mat);
        allMaterials.Add(targetMat);


        lockGazeNow = Time.time + 100f;
        lockTargetNow = Time.time + 100f;

        initializedprojectedred = Instantiate(projectedred, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
        initializedprojectedred.gameObject.name = "red_Transparent";
        initializedprojectedgreen = Instantiate(projectedgreen, new Vector3(0, 0, 0), Quaternion.identity);
        initializedprojectedgreen.gameObject.name = "green_Transparent";
        initializedprojectedblue = Instantiate(projectedblue, new Vector3(0, 0, 0), Quaternion.identity);
        initializedprojectedblue.gameObject.name = "blue_Transparent";

        objects_not_at_target.Add(initializedprojectedred);
        objects_not_at_target.Add(initializedprojectedgreen);
        objects_not_at_target.Add(initializedprojectedblue);

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

            if (objects_not_at_target[i].name == "red_Transparent")
            {
                objects_not_at_target[i].transform.position = position_red;
            }

            if (objects_not_at_target[i].name == "green_Transparent")
            {
                objects_not_at_target[i].transform.position = position_green;
            }

            if (objects_not_at_target[i].name == "blue_Transparent")
            {
                objects_not_at_target[i].transform.position = position_blue;
            }
        }



        /////////////////////////////////////////////////////////////////At the start of every update, we check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////

        if (objects_at_target.Count == 1)
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


        /////////////////////////////////////////////////////////////////First must check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////


        position_red = image_position1; //static positions
        position_red.x = position_red.x - 0.3f;
        position_green = image_position1;
        position_blue = image_position1;
        position_blue.x = position_blue.x + 0.3f;
        targetbase = image_position1;
        targetbase.z = targetbase.z - 0.35f;
        position_plane = image_position1;
        position_plane.y = position_plane.y - 0.1f;

        initializedplane.transform.position = position_plane;
        initializedred.transform.position = position_red;
        initializedgreen.transform.position = position_green;
        initializedblue.transform.position = position_blue;
        initializedtarget.transform.position = targetbase;

        
        float distanceCube1, distanceCube2, distanceCube3;

        distanceCube1 = Vector3.Distance(position_red, eyes_pos);  //finds the distnace between the cube and the eyegaze position
        distanceCube2 = Vector3.Distance(position_green, eyes_pos);
        distanceCube3 = Vector3.Distance(position_blue, eyes_pos);


        float[] cubeDistances = new float[3]; //initializes the variable cubeDistances with each index as the cube distance
        cubeDistances[0] = distanceCube1;
        cubeDistances[1] = distanceCube2;
        cubeDistances[2] = distanceCube3;


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

                            if (lastObject == "red")
                            {
                                current_object_to_move = 0;
                            }
                            if (lastObject == "green")
                            {
                                current_object_to_move = 1;
                            }
                            if (lastObject == "blue")
                            {
                                current_object_to_move = 2;
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

                Debug.Log("Current object name:"+current_object+" :last object"+lastObject);

               
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
                    initializedtarget.GetComponent<MeshRenderer>().material = targetMat; //turns target back to original color
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

                        count += 1; // increment count by one after target is locked every time
                        Debug.Log("COUNT IS" + count);

                        if (count == 1)
                        {
                            moveObject1 = true;
                        }

                        if (count == 2)
                        {
                            moveObject2 = true;
                        }
                        else
                        {
                            moveObject3 = true;
                        }


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


        if (moveObject1)
        {
            Debug.Log("MOVING FIRST OBJECT");

            if (current_object_to_move == 0)
            {
                Debug.Log("MOVING RED OBJECT");
                objects_not_at_target.Remove(initializedprojectedred); //must remove from list so object can indeed move
                first_object = initializedprojectedred;
                first_rb = first_object.GetComponent<Rigidbody>();
                first_rb.MovePosition(first_rb.transform.position + targetbase * Time.deltaTime * speed);
                first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, targetbase, speed * Time.deltaTime);
                Debug.Log("First Object Position " + first_object.transform.position + targetbase);


                float distance_to_target = Vector3.Distance(first_object.transform.position, targetbase);


                //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
                if (distance_to_target < 0.1)
                {
                    newpos1 = first_object.transform.position;
                    if (!objects_at_target.Contains(initializedprojectedred)) //if list does not contain the red , add the red to list
                    {
                        objects_at_target.Add(initializedprojectedred);
                        targetplaced1 = true;

                    }
                }

                //if (first_object.transform.position == targetbase)
                //{
                //    newpos1 = first_object.transform.position;
                //    if (!objects_at_target.Contains(initializedprojectedred)) //if list does not contain the red , add the red to list
                //    {
                //        objects_at_target.Add(initializedprojectedred);
                //        targetplaced1 = true;

                //    }
                //}


                /*                    ////////ROS STUFF////////////////
                                    message = new std_msgs.String(initializedprojectedred.gameObject.name);
                                    this.PublishGaze(message);
                                    ////////ROS STUFF////////////////*/
            }
            else if (current_object_to_move == 1)
            {
                Debug.Log("MOVING GREEN OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen);
                first_object = initializedprojectedgreen;
                first_rb = first_object.GetComponent<Rigidbody>();
                first_rb.MovePosition(first_rb.transform.position + targetbase * Time.deltaTime * speed);
                first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, targetbase, speed * Time.deltaTime);

                float distance_to_target = Vector3.Distance(first_object.transform.position, targetbase);


                //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
                if (distance_to_target < 0.1)
                {
                    newpos1 = first_object.transform.position;
                    if (!objects_at_target.Contains(initializedprojectedgreen)) //if list does not contain the red , add the red to list
                    {
                        objects_at_target.Add(initializedprojectedgreen);
                        targetplaced1 = true;

                    }
                }




            }
            else if (current_object_to_move == 2)
            {
                Debug.Log("MOVING BLUE OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue);
                first_object = initializedprojectedblue;
                first_rb = first_object.GetComponent<Rigidbody>();
                first_rb.MovePosition(first_rb.transform.position + targetbase * Time.deltaTime * speed);
                first_object.transform.position = Vector3.MoveTowards(first_object.transform.position, targetbase, speed * Time.deltaTime);


                float distance_to_target = Vector3.Distance(first_object.transform.position, targetbase);


                //make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list
                if (distance_to_target < 0.1)
                {
                    newpos1 = first_object.transform.position;
                    if (!objects_at_target.Contains(initializedprojectedblue)) //if list does not contain the red , add the red to list
                    {
                        objects_at_target.Add(initializedprojectedblue);
                        targetplaced1 = true;

                    }
                }


            }


        }

        if (targetplaced1 == true)
        {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop

            Debug.Log("TARGETPLACED1");
            moveObject1 = false;
            startTime = 0f;
            timetoLock = 5f;
            lockGazeNow = 5f;
            lockTargetNow = 5f;
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
        ////////////////Problem: It goes straight to moveObject2 without getting the eyegaze of the second objects////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////MOVE OBJECT 2///////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////MOVE OBJECT 2///////////////////////////////////////////////////////////////////////////////////////

        if (moveObject2)
        {
            Debug.Log("moving second object");

            drop_pos2 = first_object.transform.position; //get the position of the first object 
            drop_pos2.y = drop_pos2.y + 0.5f; //new position to "drop" the object from


            if (current_object_to_move == 0)
            {
                objects_not_at_target.Remove(initializedprojectedred);
                second_object = initializedprojectedred;
                second_rb = second_object.GetComponent<Rigidbody>();
                second_rb.MovePosition(second_rb.transform.position + drop_pos2 * Time.deltaTime * speed);
                second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, drop_pos2, speed * Time.deltaTime);
            }

            if (current_object_to_move == 1)
            {
                objects_not_at_target.Remove(initializedprojectedgreen);
                second_object = initializedprojectedgreen;
                second_rb = second_object.GetComponent<Rigidbody>();
                second_rb.MovePosition(second_rb.transform.position + drop_pos2 * Time.deltaTime * speed);
                second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, drop_pos2, speed * Time.deltaTime); //modify code so it only goes near the base
                                                                                                                                             //// add code so it gets drop from a height

            }
            else if (current_object_to_move == 2)
            {
                objects_not_at_target.Remove(initializedprojectedblue);
                second_object = initializedprojectedblue;
                second_rb = second_object.GetComponent<Rigidbody>();
                second_rb.MovePosition(second_rb.transform.position + drop_pos2 * Time.deltaTime * speed);
                second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, drop_pos2, speed * Time.deltaTime);

            }
        }


        //if (moveObject3)
        //{
        //    Debug.Log("moving third object");

        //    newpos3 = second_object.transform.position; //get the position of the first object 
        //    newpos3.y = newpos3.y + 0.5f; //new position to "drop" the object from


        //    if (current_object_to_move == 0)
        //    {
        //        third_object = initializedprojectedred;
        //        third_rb = third_object.GetComponent<Rigidbody>();
        //        third_rb.MovePosition(third_rb.transform.position + newpos3 * Time.deltaTime * speed);
        //        third_object.transform.position = Vector3.MoveTowards(third_object.transform.position, newpos3, speed * Time.deltaTime);
        //    }

        //    if (current_object_to_move == 1)
        //    {
        //        third_object = initializedprojectedgreen;
        //        third_rb = third_object.GetComponent<Rigidbody>();
        //        third_rb.MovePosition(third_rb.transform.position + newpos3 * Time.deltaTime * speed);
        //        third_object.transform.position = Vector3.MoveTowards(third_object.transform.position, newpos3, speed * Time.deltaTime); //modify code so it only goes near the base
        //                                                                                                                                 //// add code so it gets drop from a height

        //        targetplaced2 = true;

        //    }
        //    else if (current_object_to_move == 2)
        //    {
        //        third_object = initializedprojectedblue;
        //        third_rb = third_object.GetComponent<Rigidbody>();
        //        third_rb.MovePosition(third_rb.transform.position + newpos3 * Time.deltaTime * speed);
        //        third_object.transform.position = Vector3.MoveTowards(third_object.transform.position, newpos3, speed * Time.deltaTime);


        //        targetplaced2 = true;

        //    }



        //}

    }
}

