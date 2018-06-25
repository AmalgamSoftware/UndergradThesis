//Conway ThreeState cannot use the same optimizations as Conway bool, as it directly uses the Color32 grids to calculate the simulation. For a thorough explanation of the methods used here, see
// the Conway Bool script first.

//ConwayThreeState adds an extra state to the Conway simulation. Instead of dying, cells become 'stable'. Stable cells count as living cells for the purposes of the algorithm. As such, the simulation grows rapidly.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConwayThreeState : MonoBehaviour {

	public Material mat;
	public int resolution = 1024;
	private int resolutionSquared;
	public Color32 living, dead, stable;
    private Texture2D text;
	public Texture2D inputText;
	[Range(1,8)]
	public int livingNeighborMax;
	[Range(0,7)]
	public int livingNeighborMin;
	[Range(0,8)]
	public int deadNeighborMax;
	[Range(0,8)]
	public int deadNeighborMin;
	private Color32[] colors,colorsBuffer;
	private int vidCounter = 0;
	private int timeStamp;
	// Use this for initialization
	void Start () {
		timeStamp = System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second;
		resolutionSquared = resolution * resolution;
		GetComponent<Renderer> ().material = mat;

		text = new Texture2D(resolution,resolution);
		text.filterMode = FilterMode.Point;

		mat.mainTexture = text;
		colors = new Color32[resolution * resolution];
		colorsBuffer = new Color32[resolution * resolution];
		int len = colors.Length;
		for (int i = 0; i < len; i++) {
			if (Random.value > 0.3f) {
				colors [i] = dead;
			} else {
				colors [i] = living;
			}

		}
		text.SetPixels32 (colors);
		colorsBuffer = colors;
		text.Apply ();
	}
	void CaptureVid(){
		ScreenCapture.CaptureScreenshot ("C:/CapstoneProject/Screenshots/Video/" + timeStamp.ToString() +"cellVid" + vidCounter.ToString() + ".png");
		vidCounter += 1;
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Iterate ();
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			Iterate ();
			//CaptureVid ();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			for(int i = 0; i < 100; i ++){
				Iterate ();
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			Reset();
		}
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 30f)) {
				float resAdjust = resolution / 10f;
				int hitpointx = Mathf.FloorToInt(resolution - (hit.point.x + 5f) * resAdjust);
				int hitpointy = Mathf.FloorToInt(resolution - (hit.point.z + 5f) * resAdjust) * resolution;
				int hitpoint =hitpointx + hitpointy;
				//Debug.Log (hitpointy);
				if (colorsBuffer [hitpoint].Equals (dead)) {
					colorsBuffer [hitpoint] = living;
				}
				else if (colorsBuffer [hitpoint].Equals (living)) {
					colorsBuffer [hitpoint] = dead;
				}
				text.SetPixels32 (colorsBuffer);
				text.Apply ();
			}

		}
		if (Input.GetKeyDown (KeyCode.S)) {
			ScreenCapture.CaptureScreenshot ("C:/CapstoneProject/Screenshots/" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "h" + System.DateTime.Now.Minute.ToString() + "m" +  System.DateTime.Now.Second.ToString() + "s.png");
		}
	}
	void Iterate(){
		colors = text.GetPixels32();
		int len = colors.Length;
		for (int x = 0; x < len; x++) {
			int neighbors = 0;
			if (colors [Format(x - 1)].Equals (living) || colors [Format(x - 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x + 1)].Equals (living) || colors [Format(x + 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x + resolution + 1)].Equals (living) || colors [Format(x + resolution + 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x + resolution - 1)].Equals (living) || colors [Format(x + resolution - 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x - resolution + 1)].Equals (living) || colors [Format(x - resolution + 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x - resolution - 1)].Equals (living) || colors [Format(x - resolution - 1)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x + resolution)].Equals (living) || colors [Format(x + resolution)].Equals (stable)) {
				neighbors += 1;
			}
			if (colors [Format(x - resolution)].Equals (living) || colors [Format(x - resolution)].Equals (stable)) {
				neighbors += 1;
			}
			if(colors[x].Equals(living)){
				if (neighbors >= livingNeighborMin && neighbors <= livingNeighborMax) {
					colorsBuffer [x] = stable;
				} else {

					colorsBuffer [x] = stable;
				}
			}
			if(colors[x].Equals(dead)){
				if (neighbors >= deadNeighborMin && neighbors <= deadNeighborMax) {
					colorsBuffer [x] = living;
				} 
			}
		}
		text.SetPixels32 (colorsBuffer);
		text.Apply ();
	}
	void Reset(){
		int len = colorsBuffer.Length;
		for (int i = 0; i < len; i++) {

			colorsBuffer [i] = dead;


		}	
		text.SetPixels32 (colorsBuffer);
		text.Apply ();
	}
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
