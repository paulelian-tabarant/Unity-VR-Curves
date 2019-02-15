using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PathDrawer : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;

    // Curves storage useful attributes
    public struct Coords {
        public Vector3 pos;
        public Quaternion rot;
        public float t;

        public Coords(float t, Vector3 pos, Quaternion rot) {
            this.t = t;
            this.pos = pos;
            this.rot = rot;
        }
    }

    // Path rendering
    private List<Coords>[] pathsArray;
    private const int MAXSIZE = 100;
    private int pathIndex = 0;

    // Drawing coordinates setting script reference
    private DrawingToolCoords drawingToolCoords;
    // Curve width setting script reference
    private CurveWidth curveWidth;

    // For ease of manipulation of the XYZ local coordinates hint
    public GameObject axesHintPrefab;
    private GameObject axesHint;

    // Curves rendering script reference
    private PathMeshRenderer curveRenderer;

    // File writing
    private readonly string filePath = "Assets/Output/data.txt";
    private readonly string numberFormat = "0.000000";
    private StreamWriter writer;
    private readonly char _ = ' ';

    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        axesHint = Instantiate(axesHintPrefab);
        axesHint.transform.parent = trackedObj.transform;
        axesHint.transform.position = trackedObj.transform.position;
        AttachLeftController();
    }

    private void Start() {
        pathsArray = new List<Coords>[MAXSIZE];
        curveRenderer = GetComponent<PathMeshRenderer>();
        curveWidth = GetComponent<CurveWidth>();
    }

    private void Update() {
        // Trigger pressed for the first time, should create a new line
        if (Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (drawingToolCoords == null) {
                AttachLeftController();
            }
            CreateNewPath(curveWidth.GetCurValue(), trackedObj.transform);
        // Trigger held down while already drawing a line, should add a new point
        } else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            AddPointToCurPath(Time.time, trackedObj.transform);
        } else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            curveRenderer.RenderCurve(pathsArray[pathIndex]);
            EndCurPath();
        }
    }

    /// <summary>
    /// Initialize new path handling.
    /// </summary>
    /// <param name="coord">The 2-anchor path initial position</param>
    private void CreateNewPath(float width, Transform coord) {
        pathsArray[pathIndex] = new List<Coords>();
        // Clear file content only on the first path drawing to erase previous file content
        bool append = pathIndex != 0;
        writer = new StreamWriter(filePath, append);
        // Start curve block by writing current drawing width
        writer.WriteLine(width);
    }

    /// <summary>
    /// Add a new coordinate to current path
    /// </summary>
    /// <param name="coord">The {position; rotation} transform to be added</param>
    private void AddPointToCurPath(float time, Transform coord) {
        // Get controller orientation & apply local rotation according to the drawing angle set by the user,
        // same for drawing position offset
        Quaternion rotation = drawingToolCoords != null ? 
            coord.rotation * drawingToolCoords.GetDrawingOrientation() : coord.rotation;
        Vector3 position = drawingToolCoords != null ?
            coord.position + drawingToolCoords.GetDrawingPositionOffset() * coord.forward : coord.position;
        Coords point = new Coords(time, position, rotation);
        pathsArray[pathIndex].Add(point);
        SavePoint(writer, point);
    }

    /// <summary>
    /// Save a set of coordinates (transform) into a file
    /// </summary>
    /// <param name="coord">The Transform object data to be stored</param>
    private void SavePoint(StreamWriter writer, Coords point) {
        Vector3 globalPos = point.pos;
        Quaternion globalRot = point.rot;
        string time = point.t.ToString(numberFormat);
        string x = globalPos.x.ToString(numberFormat);
        string y = globalPos.y.ToString(numberFormat);
        string z = globalPos.z.ToString(numberFormat);
        string q0 = globalRot.w.ToString(numberFormat);
        string q1 = globalRot.x.ToString(numberFormat);
        string q2 = globalRot.y.ToString(numberFormat);
        string q3 = globalRot.z.ToString(numberFormat);

        string data = time + _ + x + _ + y + _ + z + _ + q0 + _ + q1 + _ + q2 + _ + q3;
        writer.WriteLine(data);
    }

    /// <summary>
    /// All procedures required to be done when a path has ended being drawn
    /// </summary>
    private void EndCurPath() {
        writer.WriteLine();
        writer.Close();
        if (pathIndex < MAXSIZE)
            pathIndex++;
    }

    private void AttachLeftController() {
        GameObject leftControllerObj = GameObject.FindGameObjectWithTag("LeftController");
        if (leftControllerObj != null)
            drawingToolCoords = leftControllerObj.GetComponent<DrawingToolCoords>();
    }

    /// <summary>
    /// Positions axes hint according to the current drawing coordinates
    /// </summary>
    public void UpdateAxesHint() {
        if (drawingToolCoords == null)
            return;
        float k = drawingToolCoords.GetDrawingPositionOffset();
        Vector3 offset = k * trackedObj.transform.forward;
        Quaternion rotation = drawingToolCoords.GetDrawingOrientation();
        axesHint.transform.position = trackedObj.transform.position + offset;
        axesHint.transform.rotation = trackedObj.transform.rotation * rotation;
    }
}
