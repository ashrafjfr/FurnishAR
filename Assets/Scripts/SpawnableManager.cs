using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager raycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField]
    public GameObject spawnablePrefab;
    [SerializeField]
    Image removeButtonImage;

    Camera arCam;
    GameObject spawnedObject;
    bool destroyObjectFlag = false;

    Color clearButtonColor = Color.gray;
    Color redButtonColor = new Color(1.0f, 0.0f, 0.0f, 0.4f);

    private void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        removeButtonImage.color = clearButtonColor;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if (isOverUIObject(Input.GetTouch(0).position))
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                 if (Physics.Raycast(ray, out hit))
                 {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(hits[0].pose.position);
                    }
                 }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = hits[0].pose.position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (destroyObjectFlag)
                {
                    Destroy(spawnedObject);
                    spawnedObject = null;
                    resetDelete();
                }
                else
                {
                    spawnedObject = null;
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

    public void RemoveFurniture()
    {
        if (destroyObjectFlag)
        {
            resetDelete();
        }
        else
        {
            setDelete();
        }
    }

    public void setDelete()
    {
        destroyObjectFlag = true;
        removeButtonImage.color = destroyObjectFlag ? redButtonColor : clearButtonColor;
    }

    public void resetDelete()
    {
        destroyObjectFlag = false;
        removeButtonImage.color = clearButtonColor;
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
    }

    public void SwitchFurniture(GameObject furniture)
    {
        spawnablePrefab = furniture;
    }
}

