﻿using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding
{
	public class Unit : MonoBehaviour
	{
		public Transform playerTransform;
		
		const float minPathUpdateTime = .2f;
		const float pathUpdateMoveThreshold = .5f;

		public float speed = 20;
		public float turnSpeed = 3;
		public float turnDst = 5;
		public float stoppingDst = 10;

		Path path;

		public bool isCoroutineFinished = false;
		public Action OnCoroutineFinishedCallback;

		public LayerMask mask;
		public float followTimeAfterEscape;	
		public Transform[] patrolPoints;


		private void Start()
		{
			OnCoroutineFinishedCallback += FinishCoroutine;
		}

		public void GoTo(Transform target)
		{
			StopAllCoroutines();
			StartCoroutine(GoTo_Coroutine(target));
		}
		
		public void Follow(Transform target)
		{
			StopAllCoroutines();
			StartCoroutine(Follow_Coroutine(target));
		}
		
		IEnumerator Follow_Coroutine(Transform target)
		{
			isCoroutineFinished = false;
			
			if (Time.timeSinceLevelLoad < .3f) {
				yield return new WaitForSeconds (.3f);
			}
			PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

			float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
			Vector3 targetPosOld = target.position;

			while (true) {
				yield return new WaitForSeconds (minPathUpdateTime);
				if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
					PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
					targetPosOld = target.position;
				}
			}
		}
		
		public IEnumerator GoTo_Coroutine(Transform target)
		{
			isCoroutineFinished = false;
			
			if (Time.timeSinceLevelLoad < .3f) {
				yield return new WaitForSeconds (.3f);
			}
			PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

			float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
			Vector3 targetPosOld = target.position;
		}
		
		public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
			if (pathSuccessful) {
				path = new Path(waypoints, transform.position, turnDst, stoppingDst);

				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		}

		IEnumerator FollowPath() {

			bool followingPath = true;
			int pathIndex = 0;
			
			transform.LookAt (path.lookPoints [0]);

			float speedPercent = 1;

			while (followingPath) {
				Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
				while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
					if (pathIndex == path.finishLineIndex) {
						followingPath = false;
						OnCoroutineFinishedCallback?.Invoke();
						break;
					} else {
						pathIndex++;
					}
				}

				if (followingPath) {

					if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
						//speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDst);
						if (speedPercent < 0.01f) {
							followingPath = false;
						}
					}

					Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
					transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
					transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
				}

				yield return null;
			}
		}

		private void FinishCoroutine()
		{
			isCoroutineFinished = true;

		}

		public void OnDrawGizmos() {
			if (path != null) {
				//path.DrawWithGizmos ();
			}
		}
	}
}