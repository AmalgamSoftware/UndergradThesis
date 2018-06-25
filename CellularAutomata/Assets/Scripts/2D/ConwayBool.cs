//Stephen Kreiss, 2017
//"Novice Level Implementations of Cellular Automata for the Purpose of Data Manupulation"
//
// This script allows the iteration of a modifiable version of Conway's Game of Life, a famous Cellular Automata algorithm.
// This semi-optimized version uses a pair of two-dimensional boolean arrays to store and progress the simulation at the user's request. To draw the simulation to the screen,
// the relevant boolean array is converted to a two-dimensional array of Color32 (or 32-bit colors). Further explanation of procedures can be found below.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConwayBool : MonoBehaviour {

    //mat is the Unity material that will contain the texture that displays our simulation
	public Material mat;
    //these two integers are the x and y resolutions of the experiment. if a texture is supplied to "inputText" below, these are overwritten with appropriate values;
	public int resolutionX = 1024;
	public int resolutionY = 1024;
    //resolutionSquared is self explanatory, and is used frequently in some functions because our simulation is stored in flattened 2d arrays.
	private int resolutionSquared;
    //These colors will represent the cells visually in the simulation, and can be changed whenever.
	public Color32 living, dead;
    //Text is the texture that is drawn to the screen, and is used by the Material 'mat'.
    private Texture2D text;
    //tf is the displayed plane's Transform component.
	private Transform tf;
    //ColorThreshold is used to parse the colors of inputText. When converted to grayscale, if a pixel's value is below the threshold, it is interpreted as a dead cell, vice versa.
	[Range(0,255)]
	public int colorThreshold;
    //This is an optional texture that will be parsed to produce the starting state of the simulation. Any image can be used.
	public Texture2D inputText;
    //mip defines the mip-level of the texture, used to resize large images into sizes that are more appropriate for the simulation.
	public int mip;
    //The Game of Life algorithm is as follows:
    //Any living cell with more than 3 living neighbors dies. (livingNeighborMax)
    //Any living cell with fewer than 2 living neighbors dies. (livingNeigborMin)
    //Any dead cell with exactly 3 living neighbors becomes a living cell. (deadNeighborMax & min)

    //As such, these variables are modifiable so that the user can change the rules of the simulation, and observe the effect.
	[Range(1,8)]
	public int livingNeighborMax = 3;
	[Range(0,7)]
	public int livingNeighborMin = 2;
	[Range(0,8)]
	public int deadNeighborMax = 3;
	[Range(0,8)]
	public int deadNeighborMin = 3;

    //Colors contains the (flattened) 2D array of colors that represent our simulation. Colorsbuffer is unused in this script.
	private Color32[] colors,colorsBuffer;
    //Cells is the primary storage for our simulation grid. CellsBuffer is used to store the results during an iteration, so that all cells can be operated upon without affect the next cell in each cycle.
	private bool[] cells, cellsBuffer;
    //vidcounter simply counts the screenshots.
	private int vidCounter = 0;
    //Timestamp is used to mark screenshot files with unique values.
	private int timeStamp;


	// Start happens at the very beginning, before the first Update() call.
	void Start () {
        //Get a reference to the current transform component on the plane.
		tf = GetComponent<Transform> ();
        //Set up the timestamp with current values.
		timeStamp = System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second;

        //if there is an input texture, set our resolution variables to it's dimensions.
		if(inputText != null){
			int mipInt = (int)(Mathf.Pow(2f,(float)mip));
			resolutionX = inputText.width / mipInt;
			resolutionY = inputText.height / mipInt;

		}
		//Set factorX (the resolution ratio;
		float factorX = (float)resolutionX / (float)resolutionY;
        //change the scale of the plane to reflect the resolution ratio.
		tf.localScale = new Vector3 ((tf.localScale.x ), tf.localScale.y, tf.localScale.z / factorX);
        //Calculate resolution squared.
		resolutionSquared = resolutionX * resolutionY;
        //Get a reference for the current material.
		GetComponent<Renderer> ().material = mat;

        //Initialize all arrays, and create the texture for Text;
		cells = new bool[resolutionSquared];
		cellsBuffer = new bool[cells.Length];
		colors = new Color32[cells.Length];
		text = new Texture2D(resolutionX,resolutionY);
		mat.mainTexture = text;

        //if not using an input texture, set the grid randomly.
		if (inputText == null) {
			int len = cells.Length;
			for (int i = 0; i < len; i++) {
				if (Random.value > 0.3f) {
					cellsBuffer [i] = false;
				} else {
					cellsBuffer [i] = true;
				}
			}
		}
        //if using an input texture, report its resolution to the console.
        //Then, for each pixel, convert the pixel to black and white, compare it to our threshold, and send it to the cellsBuffer as either a living cell or a dead cell.
		else{
			Debug.Log (resolutionX);
			colors = inputText.GetPixels32(mip);

			int len = cells.Length;
			int trueCount = 0, falseCount = 0;
			for (int i = 0; i < len; i++) {
				colors [i].a = 255;
				int avg = (colors [i].r + colors [i].g + colors [i].b) / 3;
				if (avg < colorThreshold) {
					trueCount += 1;
					cellsBuffer [i] = true;
				} else {
					falseCount += 1;
					cellsBuffer [i] = false;
				}

			}

		}
        //Importantly, set our display texture's filter mode to point filtering. otherwise, the simulation will be blurry and unintelligible.
		text.filterMode = FilterMode.Point;


        //Finally, Draw the starting state of our simulation to the image, which is subsequently drawn on the screen.
		SetTexture ();

	}
    //SetColors parses our simulation grid (a 2D array of booleans) into colors that can be drawn onscreen.
	void SetColors(){
		int l = cells.Length;
		for(int i = 0; i < l; i++){
			if (cellsBuffer [i]) {
				colors [i] = living;
			} else {
				colors [i] = dead;
			}
		}
	}
    //SetTexture calls SetColors and subsequently updates the texture so that the simulation can be viewed.
	void SetTexture(){
		SetColors ();
		text.SetPixels32 (colors);
		text.Apply ();
	}
    //Capture vid saves a screenshot of the frame it is called on. This can be combined into a video later, or kept as individual shots;
	void CaptureVid(){
		ScreenCapture.CaptureScreenshot ("C:/CapstoneProject/Screenshots/Video/" + timeStamp.ToString() +"cellVid" + vidCounter.ToString() + ".png");
		vidCounter += 1;
	}

    //Update is called once, every frame
	void Update () {
        //Right arrow will Iterate once.
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Iterate (true);
		}
        //Down arrow can be held to iterate every frame
		if (Input.GetKey(KeyCode.DownArrow)) {
			Iterate (true);
			//CaptureVid ();
		}
        //Up arrow iterates exactly 100 times. Iterate is called with the 'false' parameter so that it does not call SetTexture for every one of the 100 iterations. Instead, we call it manually afterward.
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			for(int i = 0; i < 100; i ++){
				Iterate (false);
			}
			SetTexture ();
		}
        //Space to clear the simulation (disabled)
		if(Input.GetKeyDown(KeyCode.Space)){
			//Reset();
		}
        //Decomment this to allow the user to draw cells on the simulation plane.
		/*if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 30f)) {
				float resAdjust = resolution / 10f;
				int hitpointx = Mathf.FloorToInt(resolution - (hit.point.x + 5f) * resAdjust);
				int hitpointy = Mathf.FloorToInt(resolution - (hit.point.z + 5f) * resAdjust) * resolution;
				int hitpoint =hitpointx + hitpointy;
				//Debug.Log (hitpointy);
				if (cellsBuffer [hitpoint]) {
					cellsBuffer [hitpoint] = false;
				} else {
					cellsBuffer [hitpoint] = true;
				}
				SetTexture ();
			}

		}*/
		if (Input.GetKeyDown (KeyCode.S)) {
			ScreenCapture.CaptureScreenshot ("C:/CapstoneProject/Screenshots/" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "h" + System.DateTime.Now.Minute.ToString() + "m" +  System.DateTime.Now.Second.ToString() + "s.png");
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			int len = cells.Length;
			int liver = 0;
			for(int i = 0; i < len; i++){
				if (cells [i]) {
					liver += 1;
				}
			}
			Debug.Log (liver);
		}
	}

    //The Iterate function induces one iteration of our simulation.
    //For each cell, count up its neighbors, decide its next state according to our custom rules, and assign its next state to the buffer.
	void Iterate(bool setAfter){
		cells = (bool[])cellsBuffer.Clone();
		int len = cells.Length;
		for (int x = 0; x < len; x++) {
			int neighbors = 0;
			if (cells [Format(x - 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x + 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x + resolutionX + 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x + resolutionX - 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x - resolutionX + 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x - resolutionX - 1)]) {
				neighbors += 1;
			}
			if (cells [Format(x + resolutionX)]) {
				neighbors += 1;
			}
			if (cells [Format(x - resolutionX)]) {
				neighbors += 1;
			}
			if(cells[x]){
				if (neighbors >= livingNeighborMin && neighbors <= livingNeighborMax) {
					cellsBuffer [x] = true;
				} else {
					cellsBuffer [x] = false;
				}
			}
			if(!cells[x]){
				if (neighbors >= deadNeighborMin && neighbors <= deadNeighborMax) {
					cellsBuffer [x] = true;
				} 
			}
		}
		if (setAfter) {
			SetTexture ();
		}

	}

    //Reset clears the current simulation, leaving only dead cells
	void Reset(){
		int len = cellsBuffer.Length;
		for (int i = 0; i < len; i++) {

			cellsBuffer [i] = false;


		}	
		SetTexture ();
	}

    //This function takes an integer as input, and formats it to be within the bounds of our simulation arrays. This method produces a wrap-around.
	int Format(int inputInt){
		if (inputInt < 0) {
			return inputInt + resolutionSquared;
		} else if (inputInt >= resolutionSquared) {
			return inputInt - resolutionSquared;
		} else {
			return inputInt;
		}
	}
}
