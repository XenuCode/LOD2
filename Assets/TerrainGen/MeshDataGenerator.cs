using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = System.Numerics.Vector2;

public static class MeshDataGenerator
{
    public static int[] GenerateVerticesOrder(int chunkSize, int lodLvl)
    {

        int[] arr = new int[chunkSize * chunkSize * 6];

        int iterator = 0;
        for (int y = 0; y < chunkSize-lodLvl; y+=lodLvl)
        {
            for (int x = 0; x < chunkSize-lodLvl; x+=lodLvl)
            {
                // left triangle
                arr[iterator] = x + y * chunkSize; iterator++; //zerout
                arr[iterator] = x + (y+lodLvl) * chunkSize; iterator++; //up
                arr[iterator] = x + lodLvl + y * chunkSize; iterator++; //down right
                //right triangle
                arr[iterator] = x + lodLvl + y * chunkSize; iterator++;//right
                arr[iterator] = x + (y+lodLvl) * chunkSize; iterator++; //left up
                arr[iterator] = x + lodLvl + (y+lodLvl) * chunkSize; iterator++;//right upz
            }
        }
        return arr;
    }

    /*public static Task<Vector3[]> AsyncHeightMap(int chunkSize,int2 position,float heightMultiplayer,int pointOffset)
    {
        Task<Vector3[]> task = Task.Run(() => HeightMap(chunkSize,position,heightMultiplayer,pointOffset));
        return task;
    }*/
    
    public static Vector3[] HeightMap(int chunkSize,int2 position,float heightMultiplayer,int pointOffset,int heightMultipl,Queue<ChunkGovernor.ChunkData> chunksToRender, AnimationCurve animationCurve)
    {
        //EwCamZk+GgABEQACAAAAAADgQBAAAACIQR8AFgABAAAACwADAAAAAgAAAAMAAAAEAAAAAAAAAD8BFAD//wAAAAAAAD8AAAAAPwAAAAA/AAAAAD8BFwAAAIC/AACAPz0KF0BSuB5AEwAAAKBABgAAj8J1PACamZk+AAAAAAAA4XoUPw==
        FastNoise fastNoise = FastNoise.FromEncodedNodeTree("GQAbABMAzcxMPg0ABQAAAPYoPEAHAABcj8I+AJqZmT4Aw/VIQAEZABsAEwAAAIA/BwAAj8J1PgEbAA0AAwAAAAAAAEATALgehUAHAAAAAAA/AAAAAAAACtcjPQ==");
        //GQAbABMAzcxMPg0ABQAAAPYoPEAHAABcj8I+AJqZmT4Aw/VIQAEZABsAEwAAAIA/BwAAj8J1PgEbAA0AAwAAAAAAAEATALgehUAHAAAAAAA/AAAAAAAACtcjPQ==
        //GQAbABMAzcxMPg0ABQAAAPYoPEAHAABcj8I+AJqZmT4Aw/VIQAEZABsAEwAAAIA/BwAAj8J1PgEbAA0AAwAAAAAAAEATALgehUAHAAAAAAA/AAAAAAAACtcjPQ==
        Vector3[] vertices = new Vector3[chunkSize * chunkSize];
        float[] noiseData = new float[chunkSize * chunkSize];
        var val = fastNoise.GenUniformGrid2D(noiseData, position.x*(chunkSize-1), position.y*(chunkSize-1), chunkSize, chunkSize, 0.02f, 1337);
        //float scale = 255.0f / (minMax.max - minMax.min);
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                var vec = new Vector3(x * pointOffset, noiseData[x + chunkSize * y] * heightMultiplayer, y * pointOffset);
                //Debug.Log(" :::::"+val);
                //vec.y = y*animationCurve.Evaluate((noiseData[x + chunkSize * y] + val.min) /val.max);
                vertices[x+y*chunkSize] =vec ;
            }
        }
        //musi poczekać aż chunksToRender będzie "wolne" i dopiero wtedy modyfikuje QUQUEUEUEUUEUEUE
        lock (chunksToRender) //TUTAJ TAK SAMO inne wątki muszą poczekać aż wykona się ten kod
        {
            chunksToRender.Enqueue(new ChunkGovernor.ChunkData(position, chunkSize, vertices,pointOffset,heightMultipl,1));
        }
        // i to są wsm takie największe zmiany
        return vertices;
    }
}
