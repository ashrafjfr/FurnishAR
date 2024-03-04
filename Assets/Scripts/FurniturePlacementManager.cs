using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject SpawnableFurniture;
    public ARSessionOrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    Camera arCam;
    private GameObject spawnedObject;
    bool destroyObjectFlag = false;

    private void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {

            if (isOverUIObject(Input.GetTouch(0).position) == false)
            {
                return;
            }

            RaycastHit hit;
            Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

            if (raycastManager.Raycast(Input.GetTouch(0).position, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
                {
                    // bool collision = raycastManager.Raycast(Input.GetTouch(0).position, raycastHits, TrackableType.PlaneWithinPolygon);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.tag == "Spawnable")
                        {
                            spawnedObject = hit.collider.gameObject;
                        }
                        else
                        {
                            //if (collision && isButtonPressed() == false)
                            if (isButtonPressed() == false)
                            {
                                GameObject _object = Instantiate(SpawnableFurniture);
                                _object.transform.position = raycastHits[0].pose.position;
                                _object.transform.rotation = raycastHits[0].pose.rotation;
                            }

                            foreach(var planes in planeManager.trackables)
                            {
                                planes.gameObject.SetActive(false);
                            }

                            planeManager.enabled = false;
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
                {
                    spawnedObject.transform.position = raycastHits[0].pose.position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (destroyObjectFlag)
                    {
                        Destroy(spawnedObject);
                        spawnedObject = null;
                        destroyObjectFlag = false;
                    }
                    else
                    {
                        spawnedObject = null;
                    }
                }
            }
        }
    }

    private bool isOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, results);
        
        return results.Count > 0;
    }

    public bool isButtonPressed()
    {
        if (EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null)
        {
            return false;
        } 
        else 
        {
            return true;
        }
    }

    public void RemoveFurniture()
    {
        destroyObjectFlag = true;
    }

    public void SwitchFurniture(GameObject furniture)
    {
        SpawnableFurniture = furniture;
    }

}