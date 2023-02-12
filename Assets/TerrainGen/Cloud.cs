using System;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainGen
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField] private Animation animation;
        [SerializeField]
        private float windStrength;
        [SerializeField] private Rigidbody rb;
        public void Start()
        {
            rb.velocity = new Vector3(Random.Range(-1f, 1f) * windStrength, 0, Random.Range(-1f, 1f)) * windStrength;
            //rb.AddForce(,ForceMode.Impulse);
            Destroy(gameObject,180);
        }
    }
}