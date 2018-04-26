using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
	[Header("Specify the Random Objects to spawn from:")]
	public GameObject[] randomObjects;

	// This will be filled with all scene objects tagged "SpawnPoint" (case sensitive)
	GameObject[] spawnPoints;

	void Start()
	{
		int spawnCount = 0;

		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

		// Iterate through the spawn points
		foreach(GameObject spawnPoint in spawnPoints)
		{
			// A 50/50 chance: 1 is an object, 0 is no object
			if( Random.Range(0, 2) == 1 )
			{
				// Randomly choose one of the objects
				int index = Random.Range(0, randomObjects.Length);

				// Spawn the object at the spawn point
				Instantiate(randomObjects[index], spawnPoint.transform.position, spawnPoint.transform.rotation);

				spawnCount ++;
			}
		}

		// Ensure at least one random object is spawned
		if(spawnCount == 0)
		{
			// Randomly choose one of the objects
			int index = Random.Range(0, randomObjects.Length);

			// Randomly choose one of the spawn points
			int point = Random.Range(0, spawnPoints.Length);

			// Spawn the object at the spawn point
			Instantiate(randomObjects[index], spawnPoints[point].transform.position, spawnPoints[point].transform.rotation);
		}
	}
}
