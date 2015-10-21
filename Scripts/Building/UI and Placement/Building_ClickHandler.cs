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

	private bool playerIsNear = false;// Only turns true if the player walks up to the building

	public enum State { ASSEMBLING, READY, DISSASEMBLING, RECYCLE_NANOBOTS }
	private State _state;
	public State state { get { return _state; } set { _state = value; } }

	NanoBuilding_Handler nano_builder; // this will allow the building to give back the nanobots when sold, getting it from resourceGrid


	void OnEnable()
	{
		// Make sure to reset the color
		s_renderer = GetComponent<SpriteRenderer> ();

		FadeIn ();
		// reset timer variables
		isDissasembling = false;
		isFading = false;

		// Assemble
		_state = State.ASSEMBLING;

	}
	void Awake()
	{
		_state = State.ASSEMBLING;
		s_renderer = GetComponent<SpriteRenderer> ();
		s_renderer.color = B;
		FadeIn ();
	}

	void FadeIn()
	{
		s_renderer.color = Color.Lerp (B, A, colorTime);
		
		if (colorTime < 1){ 
			// increment colorTime it at the desired rate every update:
			colorTime += Time.deltaTime/colorChangeDuration;
		}

		if (s_renderer.color == A){
			colorTime = 0;
			_state = State.READY;
		}
	}

	void Start () {

		nano_builder = resourceGrid.Hero.GetComponent<NanoBuilding_Handler> ();

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

	void Update()
	{			
		MyStateMachine (_state);
	}

	void MyStateMachine(State _curState)
	{
		switch (_curState) {
		case State.ASSEMBLING:
			FadeIn();
			break;
		case State.READY:

			DissasemblyControl();

			if (Input.GetKeyDown (KeyCode.F) && playerIsNear) {
				if (!buildingUIhandler.currentlyBuilding){
					if (!buildingPanel.gameObject.activeSelf) {
						ActivateBuildingUI ();
						
					}else{
						ClosePanel();
					}
					
				}
			}
			break;
		case State.DISSASEMBLING:
			FadeOutControl();
			break;
		case State.RECYCLE_NANOBOTS:
			CreateNanoBots();
			break;
		default:
			break;
		}
	}

	void DissasemblyControl()
	{
		// NOTE: Right now this is making ALL buildings fade! I would have to type them ALL in...
		if (myTileType != TileData.Types.capital && myTileType != TileData.Types.generator ) {
			if (!isDissasembling){
				Dissasemble ();
			}else{
				Debug.Log("Tower dissasembling!");
			}
		} 

	}

	void FadeOutControl()
	{
		if (isFading) {
			s_renderer.color = Color.Lerp(A, B, colorTime);
			
			if (colorTime < 1){ 
				// increment colorTime it at the desired rate every update:
				colorTime += Time.deltaTime/colorChangeDuration;
			}
			
			if (s_renderer.color == B){
				colorTime = 0;
				isFading = false;
				isDissasembling = false;
				// Now turn into nanobots that will return to the Player
//				Sell ();
				_state = State.RECYCLE_NANOBOTS;
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
			_state = State.DISSASEMBLING;

		} else {

			disassemblyTime -= Time.deltaTime;
		}

	}

	// Once this building is dissasembled it will return the bots to the Hero
	void CreateNanoBots()
	{
		// TODO: Change this hardcoded value of nanobots to the building nanobot cost

		// Create nanobots
		for (int i =0; i <= 10; i++) {
			GameObject nanobot = objPool.GetObjectForType("NanoBot", true);
			if (nanobot){
				nanobot.transform.position = transform.position;
				nanobot.GetComponent<NanoBot_MoveHandler>().player = resourceGrid.Hero.transform;
				nanobot.GetComponent<NanoBot_MoveHandler>().objPool = objPool;
			}
		}
		// After nanobots are created now Sell to swap the tile
		Sell ();
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


	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag ("Citizen")) {
			playerIsNear = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag ("Citizen")) {
			playerIsNear = false;
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
