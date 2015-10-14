using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Building_ClickHandler : MonoBehaviour {

	public int mapPosX;
	public int mapPosY;
	public Building_UIHandler buildingUIhandler;
	public ResourceGrid resourceGrid;

	// UI Handler feeds this when this is a new building so it may Swap Tiles
	[HideInInspector]
	public TileData.Types tileType, myTileType;

	// get the bounds of this collider to know where to place the options panel
	BoxCollider2D myCollider;
	float vertExtents;

	[SerializeField]
	private Canvas buildingCanvas;

	[SerializeField]
	private GameObject buildingPanel;


	public Building_StatusIndicator buildingStatusIndicator;

	// Adding this object Pool here so we can feed it to the buildings as they are built
	public ObjectPool objPool;

	[Header ("For Gun Towers Only:")]
	public Tower_TargettingHandler tower;

	// Storing the building's energy cost here to access it from other scripts
	public int energyCost {  get; private set; }

	private float disassemblyTime = 10f;
	private bool isDissasembling = false, isFading = false;
	SpriteRenderer s_renderer;

	Color A = Color.white;
	Color B = Color.clear;
	public float colorChangeDuration = 2;
	private float colorTime;


	void OnEnable()
	{
		// Make sure to reset the color
		s_renderer = GetComponent<SpriteRenderer> ();
		if (s_renderer.color == B) {
			s_renderer.color = A;
		}
		// reset timer variables
		isDissasembling = false;
		isFading = false;

	}

	void Start () {


		if (buildingCanvas == null) {
			Debug.Log("CLICK HANDLER: Building Canvas not set!");
		} 
		if (buildingPanel == null) {
			Debug.Log("CLICK HANDLER: Building Panel not set!");
		}

		if (buildingStatusIndicator == null) {
			Debug.Log("CLICK HANDLER: Building Status Indicator not set!");
		}

		if (buildingCanvas != null) {
			buildingCanvas.worldCamera = Camera.main;
		}

					// IF THIS BUILDING is spawned by the UI Handler, it won't need to make this search
		if (buildingUIhandler == null) {
			buildingUIhandler = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		myCollider = GetComponent<BoxCollider2D> ();
		vertExtents = myCollider.bounds.extents.y;

		// get my tiletype
		myTileType = CheckTileType ((int)transform.position.x,(int) transform.position.y);


	}

	void OnMouseUpAsButton(){
		Debug.Log("You clicked on " + gameObject.name);

		if (!buildingUIhandler.currentlyBuilding)
			ActivateBuildingUI ();
	}

	void Update()
	{
		if (myTileType != TileData.Types.capital) {
			if (!isDissasembling){
				Dissasemble ();
			}else{
				Debug.Log("Tower dissasembling!");
			}
		} 
		if (isFading) {
			s_renderer.color = Color.Lerp(A, B, colorTime);

			if (colorTime < 1){ 
				// increment colorTime it at the desired rate every update:
				colorTime += Time.deltaTime/colorChangeDuration;
			}

			if (s_renderer.color == B){
				isFading = false;
				isDissasembling = false;
				Sell ();
			}

		}
	}

	public void ActivateBuildingUI(){
		Vector3 offset = new Vector3 (transform.position.x, transform.position.y + vertExtents);
//		if (!buildingUIhandler.currentlyBuilding)
//			buildingUIhandler.CreateOptionsButtons (offset, CheckTileType(mapPosX, mapPosY), mapPosX, mapPosY, buildingPanel, buildingCanvas);

		if (!buildingPanel.gameObject.activeSelf) {
			buildingPanel.gameObject.SetActive(true);
		}

	}

	public void ClosePanel(){
		if (buildingPanel.gameObject.activeSelf) {
			buildingPanel.gameObject.SetActive(false);
		}
	}

	public void Sell(){
		if (resourceGrid != null) {
			resourceGrid.SwapTileType(mapPosX, mapPosY, TileData.Types.empty);
		}
	}


	void Dissasemble()
	{
		if (disassemblyTime <= 0) {
			disassemblyTime = 10f;
			isDissasembling = true;
			isFading = true;

		} else {

			disassemblyTime -= Time.deltaTime;
		}

	}

	TileData.Types CheckTileType(int x, int y){
		TileData.Types type = resourceGrid.tiles [x, y].tileType;
		return type;
	}

	public void ChangeBuildingStatus(string change){

		switch (change) {
		case "Starve":

			// starve
			buildingStatusIndicator.CreateStatusMessage("Power Down!");

			break;
		case "Unstarve":

			// unstarve
			buildingStatusIndicator.CreateStatusMessage("Powering Up!");

			break;
		case "Reload":
			// show reloading message
			buildingStatusIndicator.CreateStatusMessage("Reloading...");
			break;
		case "Acquired":
			// show target acquired
			buildingStatusIndicator.CreateStatusMessage("Target acquired!");
			break;
		case "Siege":
			// under siege
			buildingStatusIndicator.CreateStatusMessage("Under Attack!");
			break;
		default:
			// building name initialized
			break;
		}

	}

//	void OnMouseOver()
//	{
//
//		// Turn ON Manual Control for Gun Towers:
//		if (tower != null) {
//			if (tower.state != Tower_TargettingHandler.State.MANUAL_CONTROL && 
//			    tower.state != Tower_TargettingHandler.State.MANUAL_SHOOTING && 
//			    tower.state != Tower_TargettingHandler.State.STARVED){
//
//				if (Input.GetMouseButtonDown(1)){
//
//					tower.state = Tower_TargettingHandler.State.MANUAL_CONTROL;
//
//					// Also turn off the Building Menus so they don't get in the way
//					buildingUIhandler.currentlyBuilding = true;
//
//				}
//
//			}else if (tower.state == Tower_TargettingHandler.State.MANUAL_CONTROL || 
//			          tower.state == Tower_TargettingHandler.State.MANUAL_SHOOTING){
//				
//				if (Input.GetMouseButtonDown(1)){
//					
//					tower.state = Tower_TargettingHandler.State.SEEKING;
//					
//					// Turn Building Menus back on
//					buildingUIhandler.currentlyBuilding = false;
//					
//				}
//			}
//
//		}
//
//	}
	
//	void OnMouseExit()
//	{
//		// Turn OFF Manual Control
//		if (tower.state == Tower_TargettingHandler.State.MANUAL_CONTROL || 
//		    tower.state == Tower_TargettingHandler.State.MANUAL_SHOOTING){
//			
//			if (Input.GetMouseButtonDown(1)){
//				
//				tower.state = Tower_TargettingHandler.State.SEEKING;
//				
//				// Turn Building Menus back on
//				buildingUIhandler.currentlyBuilding = false;
//				
//			}
//		}
//	}
}
