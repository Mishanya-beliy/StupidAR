using GoogleARCore;
using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine;

public class PlaceTable : MonoBehaviour
{
    public GameObject manipulator, manipulationSystem, nameRoomTabloPrefab;
    private static GameObject nameRoomTablo;

    void Awake()
    {
        StartEditTable();
    }

    public void CreateTablo(string nameRoom)
    {
        if (nameRoomTablo == null)
            nameRoomTablo = Instantiate(nameRoomTabloPrefab, transform.up * 0.7f, transform.rotation);

        nameRoomTablo.GetComponent<Renderer>().enabled = true;
        nameRoomTablo.transform.GetChild(0).GetComponent<TextMesh>().text = nameRoom;
        nameRoomTablo.transform.parent = transform;
    }

    public void StartEditTable()
    {
        Instantiate(manipulationSystem).name = "Manipulation System";
        var manipulator = Instantiate(this.manipulator);
        transform.parent = manipulator.transform;
        manipulator.GetComponent<Manipulator>().Select();
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
    }
    private bool Moving()
    {
        InfoForPlacingTable info = UpdatePlacementPose();
        UpdatePlcamentTable(info.placementPoseIsValid, info.placmentPose);
        /*if (stop && info.placementPoseIsValid)
        {
            var anchor = info.hit.Trackable.CreateAnchor(info.hit.Pose);
            gameObject.transform.parent = anchor.transform;
        }*/
        return info.placementPoseIsValid;
    }
    public bool StopMoving()
    {
        if (Moving())
        {
            //GameObject.Find("Plane Generator").GetComponent<DetectedPlaneGenerator>().VisiblePlane(false);
            GameObject manipulator = transform.parent.gameObject;
            transform.parent = null;
            Destroy(manipulator);
            Destroy(GameObject.Find("Manipulation System"));
            GameObject.Find("Editing Table").SetActive(false);
            GameObject.Find("Scene Controller").GetComponent<GameController>().EditedTable();
            enabled = false;
            return true;
        }
        else return false;        
    }
    private float angleRotation = 0.0f;
    public void AddRotation(float angle)
    {
        angleRotation += angle;
        if (angleRotation >= 360.0f)
            angleRotation -= 360.0f;
    }
    private InfoForPlacingTable UpdatePlacementPose()
    {
        bool placementPoseIsValid;      /**/                     Pose placmentPose = new Pose();
        
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(screenCenter.x, screenCenter.y, raycastFilter, out TrackableHit hit))
        {
            placementPoseIsValid = true;
            placmentPose = hit.Pose;


            if (hit.Trackable is DetectedPlane)
            {
                DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                
                Vector3 forward = Quaternion.Euler(0, angleRotation, 0) * Vector3.forward;
                if (detectedPlane.PlaneType == DetectedPlaneType.Vertical) forward = detectedPlane.CenterPose.rotation * forward;  //new Vector3(angleRotation, 1, 0);//Camera.current.transform.up; //new Vector3(angleRotation, 0, 1);//Camera.current.transform.forward; 
                Vector3 up = detectedPlane.CenterPose.up;
                
                placmentPose.rotation = Quaternion.LookRotation(forward, up);
            }
        }
        else placementPoseIsValid = false;
        return new InfoForPlacingTable(placementPoseIsValid, placmentPose, hit);
    }

    private void UpdatePlcamentTable(bool placementPoseIsValid, Pose placmentPose)
    {
        if (placementPoseIsValid)
        {
            transform.parent.SetPositionAndRotation(placmentPose.position, placmentPose.rotation);
            //transform.parent.Rotate(0.0f, angleRotation, 0.0f);
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = true;
        }
        else foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
    }
    struct InfoForPlacingTable
    {
        public bool placementPoseIsValid;
        public Pose placmentPose;
        public TrackableHit hit;

        public InfoForPlacingTable(bool valid, Pose pose, TrackableHit hit)
        {
            placementPoseIsValid = valid;
            placmentPose = pose;
            this.hit = hit;
        }
    }
}