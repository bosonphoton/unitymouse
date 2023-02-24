using UnityEngine;
using System.Collections;
public class ObjectHighlight : MonoBehaviour
{
    //When the mouse hovers over the GameObject, it turns to this color (red)
    //Color m_MouseOverColor = Color.yellow;

    //public Material detectedMaterial, lockedMaterial;

    ////This stores the GameObject’s original color
    //Color m_OriginalColor;

    ////Get the GameObject’s mesh renderer to access the GameObject’s material and color
    //MeshRenderer m_Renderer;

    //void Start()
    //{
    //    //Fetch the mesh renderer component from the GameObject
    //    m_Renderer = GetComponent<MeshRenderer>();
    //    //Fetch the original color of the GameObject
    //    m_OriginalColor = m_Renderer.material.color;
    //}

    //void OnMouseOver()
    //{
    //    // Change the color of the GameObject to red when the mouse is over GameObject
    //    m_Renderer.material.color = m_MouseOverColor;
    //}

    //void OnMouseExit()
    //{
    //    // Reset the color of the GameObject back to normal
    //    m_Renderer.material.color = m_OriginalColor;
    //}




    Camera cam;
    public LayerMask mask;

    public Material detectedMaterial, lockedMaterial;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Draw Ray
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
            hit.transform.GetComponent<Renderer>().material.color =
            detectedMaterial.color;
            
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 100, mask))
        //    {
        //        Debug.Log(hit.transform.name);
        //        hit.transform.GetComponent<Renderer>().material.color =
        //        lockedMaterial.color;
        //    }
        //}
    }
}