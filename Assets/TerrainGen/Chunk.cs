using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class Chunk : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField]private MeshFilter _meshFilter;
    public ChunkData chunkData; //wszystkie dane o położeniu itp chunka są w osobnej klasie

    [SerializeField] public GameObject Treeprefab;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetMesh()
    {
        transform.position = new Vector3(chunkData.position.x*chunkData.pointOffset* (chunkData.chunkSize-1) - (chunkData.chunkSize - 1)*chunkData.pointOffset,-1000,chunkData.position.y*chunkData.pointOffset* (chunkData.chunkSize-1)-(chunkData.chunkSize - 1)*chunkData.pointOffset);
        var mesh = _meshFilter.mesh;
        mesh.vertices = chunkData.vertecies;
        mesh.triangles = MeshDataGenerator.GenerateVerticesOrder(chunkData.chunkSize,4);
        chunkData.lodLvl = 4;
        // Vector3[] normals = new Vector3[chunkData.chunkSize*chunkData.chunkSize];
        // for (int x = 0; x < normals.Length; x++)
        // {
        //     normals[x] = Vector3.back;
        // }

        //mesh.normals = normals;
        mesh.RecalculateNormals();
        StartCoroutine(PopUp());
        //PopulateWithTrees();
    }

    IEnumerator PopUp()
    {
        var pos = transform;
        for (int i = 0; i < 60; i++)
        {
            pos.position = transform.position + new Vector3(0, 16.7f, 0);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void PopulateWithTrees()
    {
        var mesh = _meshFilter.mesh;
        Debug.Log(mesh.normals[40]);
        for (int x = 0; x < chunkData.chunkSize; x++)
        {
            for (int y = 0; y < chunkData.chunkSize; y++)
            {
                Random random = new Random();
                if (random.Next(0,10000) == 1)
                {
                    if (mesh.normals[x + y * chunkData.chunkSize].y > 0.98)
                    {
                        Vector3 vec = mesh.vertices[x + y * chunkData.chunkSize];
                        vec.x *= chunkData.position.x;
                        vec.z *= chunkData.position.y;
                        Instantiate(Treeprefab, vec, Quaternion.identity);
                    }
                }
            }   
        }
    }
    
    //TODO yep, use onCollisionEnter / onCollisionStay for doing lod and generating/destroing chunks 

    public void SetLOD(int f_LodLvl)
    {
        chunkData.lodLvl = f_LodLvl;
        _meshFilter.mesh.triangles = MeshDataGenerator.GenerateVerticesOrder(chunkData.chunkSize, f_LodLvl);
        _meshFilter.mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
