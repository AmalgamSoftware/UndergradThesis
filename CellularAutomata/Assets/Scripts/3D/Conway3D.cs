//For a more thorough explanation of the procedure here, see the CownayBool script first.

//Conway3D uses a three-dimensional array of booleans to store the simulation, and a 3D array of cubes to represent it.
//Since the Conway algorithm is meant for a 2D array, there are few rule-sets here that produce visually pleasing results.
//Higher resolutions offer more clear simulations at the cost of performance.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conway3D : MonoBehaviour {

	public GameObject cube;
	public int resolution = 20;
	private GameObject[,,] grid;
	private bool[,,] gridBuffer;

	[Range(1,27)]
	public int livingNeighborMax;
	[Range(0,26)]
	public int livingNeighborMin;
	[Range(0,27)]
	public int deadNeighborMax;
	[Range(0,27)]
	public int deadNeighborMin;

	private Transform tf;
	private bool playSim = false;
	private float simTimeCounter;
	public float simRate = 0.5f;
	private int vidCounter = 0;
	private int timeStamp;

	void Start(){
		timeStamp = System.DateTime.Now.Hour * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second;

		tf = GetComponent<Transform> ();
		grid = new GameObject[resolution, resolution, resolution];
		gridBuffer = new bool[resolution, resolution, resolution];


		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
					if (Random.value < 0.3f) {
						CreateCell (x, y, z);
						gridBuffer [x, y, z] = true;
					} else {
						gridBuffer [x, y, z] = false;
					}
				}
			}
		}
	}
	void CaptureVid(){
		ScreenCapture.CaptureScreenshot ("C:/CapstoneProject/Screenshots/Video/" + timeStamp.ToString() +"cellVid" + vidCounter.ToString() + ".png");
		vidCounter += 1;
	}
	void CreateCell(int xc, int yc, int zc){
		GameObject cell = (GameObject)Instantiate (cube, new Vector3 (xc, yc, zc), Quaternion.identity, tf);
		grid [xc, yc, zc] = cell;
	}
	void DestroyCell(int xc, int yc, int zc){
		GameObject cell = grid [xc, yc, zc];
		grid [xc, yc, zc] = null;
		Destroy (cell);
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			Iterate ();
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			//CaptureVid();
			Iterate ();
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			playSim = !playSim;
			Debug.Log (playSim);
		}
		//check sim play
		if(playSim){
			simTimeCounter += Time.deltaTime;
			if (simTimeCounter >= simRate) {
				Iterate ();
				simTimeCounter = 0;
			}
		}
	}
	void Iterate(){
		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
						int neighbors = 0;
						for (int xTest = -1; xTest < 2; xTest++) {
							for (int yTest = -1; yTest < 2; yTest++) {
								for (int zTest = -1; zTest < 2; zTest++) {
									if (grid [Format (x + xTest), Format (y + yTest), Format (z + zTest)] != null) {
									if (zTest == 0) {
										if (xTest == 0) {
											if (yTest == 0) {
													
											} else {
												neighbors += 1;
											}
										} else {
											neighbors += 1;
										}

									} else {
										neighbors += 1;
									}
						
									}
								}
							}
						}
						//Debug.Log (neighbors);
					//if Cell is living:
					if(grid[x,y,z] != null){
						if (neighbors >= livingNeighborMin && neighbors <= livingNeighborMax) {
							
						}
						else{
							gridBuffer [x, y, z] = false;
						}
					}
					//if Cell is dead:
					else{
						if (neighbors >= deadNeighborMin && neighbors <= deadNeighborMax) {
							gridBuffer [x, y, z] = true;
						}
					}
							
				}
			}
		}
		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
					if (gridBuffer [x, y, z]) {
						if (grid [x, y, z] != null) {
							
						} else {
							CreateCell (x, y, z);
						}
					} else {
						if (grid [x, y, z] != null) {
							DestroyCell (x, y, z);
						} else {
							
						}
					}
				}
			}
		}
	}

	private int Format(int input){
		if (input < 0) {
			input += resolution;
		} else if (input >= resolution) {
			input -= resolution;
		}
		return input;
	}

}
