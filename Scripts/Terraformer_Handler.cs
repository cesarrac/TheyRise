﻿using UnityEngine;
using System.Collections;

public class Terraformer_Handler : MonoBehaviour {

	/// <summary>
	/// This script handles the progress of a terraformer and communicates when final stage is done to a Master script
	/// that will load the victory screen and progress to next level.
	/// </summary>

	private float _maxTerraformingTime = 360f; // time in seconds (6 minutes) it takes a terraformer to complete 5 cycles

	private float _maxTerraformCycles = 5; // # of cycles that must that must complete for terraforming to finish
	private float _maxCycleTime = 72f; // time in seconds it takes for a cycle to finish
	public float currProgressTime { get; private set;} // current elapsed time, resets to 0 when a cycle is completed
	private int _currCycleCount = 1; // keeps track of the current cycle terraformer is on
	public int currCycle {get {return _currCycleCount;} set {_currCycleCount = Mathf.Clamp(value, 1, 5);}}


	public enum State {IDLING, WORKING, DONE};
	private State _state = State.IDLING;
	public State curState { get { return _state; }}

	public Enemy_WAVESpawnerV2 waveSpawner; // control over the spawning of enemy waves


	void Awake()
	{
		currProgressTime = 0;

		if (GameObject.FindGameObjectWithTag ("Spawner") != null) {
			waveSpawner = GameObject.FindGameObjectWithTag ("Spawner").GetComponent<Enemy_WAVESpawnerV2> ();
			waveSpawner.terraformer = this;
		}
	}
	
	void Update () 
	{
		if (waveSpawner)
			MyStateMachine(_state);
	}

	void MyStateMachine(State _curState)
	{
		switch (_curState) {
		case State.WORKING:
			// Continue counting down to finish a cycle and allow enemies to spawn
			TerraformingCountdown();
			break;
		case State.DONE:
			//Notify Game Master that this level was succesfully terraformed, load Victory
			Debug.Log ("TERRAFORMER: Terraforming complete! Victory!!!");
			break;
		default:
			// Terraformer is IDLING
			break;
		}
	}

	void TerraformingCountdown()
	{
		if (currProgressTime >= _maxCycleTime) {
			// Cycle is complete, make sure this was not the final stage
			if (currCycle < _maxTerraformCycles) {

				Debug.Log ("TERRAFORMER: Cycle " + currCycle + " Completed succesfully!");
				// Add one to the current cycle
				currCycle++;

				// Reset progress time
				currProgressTime = 0;

				// Go to idling state to await for player to start the machine again
				_state = State.IDLING;

			} else {
				// complete terraforming 
				_state = State.DONE;
			}
		} else {
			currProgressTime += Time.deltaTime;
		}
	}

	public void StartTerraformer()
	{
		if (_state != State.WORKING) {
			Debug.Log("TERRAFORMER: Beginning terraforming cycle " + currCycle);
			_state = State.WORKING;
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Citizen") {
			StartTerraformer();
		}
	}
}
