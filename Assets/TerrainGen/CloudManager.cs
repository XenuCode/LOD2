using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainGen
{
    public class CloudManager : MonoBehaviour
    {
        [SerializeField] private int cloudAmount=4;
        public void Start()
        {
            StartCoroutine(AddClouds());
        }

        [SerializeField] private float heightOffset,varyOffset;
        [SerializeField] private GameObject cloudPrefab;
        [SerializeField] private float spawnSize;
        IEnumerator AddClouds()
        {
            for(;;)
            {
                for(int x=0; x<cloudAmount; x++) {
                    var pos = Camera.main.gameObject.transform.position;
                    Vector2 position = new Vector2(pos.x, pos.z);
                    Vector3 spawnPos = new Vector3(
                        Mathf.Clamp(Random.Range(-spawnSize + pos.x, spawnSize + pos.x), -spawnSize + pos.x,
                            spawnSize + pos.x), heightOffset + Random.Range(-varyOffset, varyOffset),
                        Mathf.Clamp(Random.Range(-spawnSize + pos.z, spawnSize + pos.z), -spawnSize + pos.z,
                            spawnSize + pos.z));

                    Instantiate(cloudPrefab, spawnPos, Quaternion.Euler(new Vector3(90, 0, 0)));
                }
                yield return new WaitForSeconds(5);
            }
        }
    }
    
    
}