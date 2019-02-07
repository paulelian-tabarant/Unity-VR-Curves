using UnityEngine;
using System.IO;

public class LineDrawer : MonoBehaviour {
    // Stores a reference to the controller object data
    private SteamVR_TrackedObject trackedObj;

    // Line drawing useful attributes
    private LineRenderer curLine;
    public Material lineMaterial;
    public float lineWidth;
    int pointsIndex = 0;

    // File writing
    private readonly string filePath = "Assets/Output/data.txt";
    private StreamWriter writer;
    private bool newLine = true;
    private readonly string sep = "; ";

    // Accessing controller input via Controller reference for ease.
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start() {
        // Automatically clears data file on startup
        ClearFileContent();
    }

    private void Update() {
        // Trigger pressed for the first time, should create a new line
        if (Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            AddNewLine();
        // Trigger held down while already drawing a line, should add a new point
        } else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            // Render the line on screen
            AddVertexToCurLine(trackedObj.transform.position);
            // Save line data
            SaveLocalFrame(trackedObj.transform);
        } else if (Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) { 
            // Close the input stream and add a line jump when the user stopped drawing
            writer.WriteLine();
            writer.Close();
        }
    }

    // Creates a new game object in the scene, rendering a line drawing
    // Also initializes file writing for this line
    private void AddNewLine() {
        // Add the game object to the scene
        GameObject lineObj = new GameObject();
        curLine = lineObj.AddComponent<LineRenderer>();
        curLine.material = lineMaterial;
        curLine.startWidth = curLine.endWidth = lineWidth/2;
        pointsIndex = 0;
        newLine = true;

        // Open a new input stream to store the line
        writer = new StreamWriter(filePath, true);
    }

    // Adds a new vertex to the current line
    private void AddVertexToCurLine(Vector3 pos) {
        // Must update the vertices array size each time a new vertex is added
        curLine.positionCount = pointsIndex + 1;
        curLine.SetPosition(pointsIndex, pos);
        pointsIndex++;
    }

    // Saves the local frame (position, orientation & time) in a file
    private void SaveLocalFrame(Transform frame) {
        Vector3 globalPos = frame.position;
        Quaternion globalRot = frame.rotation;

        string data = Time.time + sep
                    + globalPos.ToString() + sep
                    + globalRot.ToString();
        writer.WriteLine(data);
    }

    // Clears content of the file associated to the "path" attribute
    private void ClearFileContent() {
        StreamWriter writer = File.CreateText(filePath);
        writer.Flush();
        writer.Close();
    }
}
