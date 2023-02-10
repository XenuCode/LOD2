using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ChunkGovernor : MonoBehaviour
{
    List<ChunkData> my_queue = new List<ChunkData>();
    [SerializeField] private GameObject _chunkPrefab;
    [SerializeField] private int heightMultiplayer;
    [SerializeField] private int pointOffset = 15;
    [SerializeField] private int REND=5;
    [SerializeField] private int N = 241;
    public AnimationCurve _animationCurve;
    private Dictionary<int2, Chunk> loadedChunks = new Dictionary<int2, Chunk>();
    private List<int2> toRemove = new List<int2>();
    public Transform camera;
    public Vector2 cameraChunkPosition;//operates on chunks
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI rendDistTxt;

    public Queue<ChunkData> chunksToRender = new Queue<ChunkData>();
    private Dictionary<int2, ChunkData> chunksToGenerate = new Dictionary<int2, ChunkData>();
    private List<int2> chunksBeingGenerated = new List<int2>();

    public void Start()
    { 
        StartCoroutine(LoadChunks());
        StartCoroutine(ScanChunks());
        
        slider.onValueChanged.AddListener((value =>
        {
            rendDistTxt.text = "render distance: " + value.ToString("N");
            REND = (int)value;
        }));
        //new Thread(new ThreadStart(() => ChunkScanningThread())).Start();
    }

//a no i jeszcze jedno egghh
    public void FixedUpdate()
    {
        //Debug.Log("chunksToRender "+chunksToRender.Count);
        //Debug.Log("chunksToGenerate "+chunksToGenerate.Count);
        //Debug.Log("chunksBeingGenerated "+chunksBeingGenerated.Count);
        lock (chunksToGenerate)
        {
            foreach (var chunkData in chunksToGenerate)
            {//TO jest całkiem nowe względem tego jak było wcześniej
                void ThreadStart()
                {
                    //Debug.Log(chunkData.Value.position.x + " " + chunkData.Value.position.y);
                    GenerateChunk(new int2(chunkData.Value.position.x, chunkData.Value.position.y));
                } //to deklaruje prywatną funkcję
                //Task.Factory.StartNew(ThreadStart);
                new Thread(ThreadStart).Start(); //a tu ją uruchamia na nowym wątku
                chunksBeingGenerated.Add(chunkData.Value.position);
            }
            chunksToGenerate.Clear();
        }
        // JA SIĘ TUTAJ TAK PRODUKUJE a ty nawet nie patrzyłaś …………………… :(
        //to "lock" jest kwintesencją tego że cokolwiek tu działa bo robi takie fancy cus
        // że jak inny wątek chce zmienić wartość chunksToRender a wykonuje się ta część kodu
        // to musi poczekać aż się ona zakończy więc np 
        lock (chunksToRender)
        {
            if (chunksToRender.Count != 0)
            {
                var chunkData = chunksToRender.Dequeue();
                SetMeshData(chunkData);
            }
        }
    }

    public void RenderDistanceChange(int change)
    {
        REND = change;
    }
    

    public void GenerateChunk(int2 position)
    {//to 
        MeshDataGenerator.HeightMap(N, position, heightMultiplayer, pointOffset,heightMultiplayer, chunksToRender,_animationCurve);
    }

    public void SetMeshData(ChunkData chunkData)
    {
        GameObject chunkObj = Instantiate(_chunkPrefab);
        var chunkScript = chunkObj.GetComponent<Chunk>();
        chunkScript.chunkData = chunkData;
        chunkScript.SetMesh();
        loadedChunks.Add(chunkData.position,chunkScript);
        chunksBeingGenerated.Remove(chunkData.position);
    }

    IEnumerator LoadChunks()
    {
        List<int2> arr = new List<int2>();
        for(;;){
            var pos = camera.transform.position / ((N - 1)*pointOffset);
            Debug.Log(pos);
            cameraChunkPosition = new Vector2(pos.x, pos.z);
            for (int x = -REND + (int)cameraChunkPosition.x; x <= REND + (int)cameraChunkPosition.x; x++)
            {
                for (int y = -REND + (int)cameraChunkPosition.y; y <= REND + (int)cameraChunkPosition.y; y++)
                {
                    lock (loadedChunks)
                    {
                        lock (chunksToGenerate)
                        {
                            lock (chunksBeingGenerated)
                            {
                                if (loadedChunks.ContainsKey(new int2(x, y)) || chunksToGenerate.ContainsKey(new int2(x,y)) || chunksBeingGenerated.Contains(new int2(x,y)))
                                {
                                    //   Debug.Log("there is a chunk at: " + new int2(x, y));
                                }
                                else
                                {
                                    lock (chunksToGenerate)
                                    {
                                        // Debug.Log("there is no chunk at: " + new int2(x, y));
                                        chunksToGenerate.Add(new int2(x, y), new ChunkData(new int2(x, y), N));
                                    }
                                }
                            } 
                        }
                    }
                }
            } 
            arr.Clear();
            yield return new WaitForSeconds(0.3f);
        }
    }
    
     IEnumerator ScanChunks(){ //for unloading chunks as well as LOD 
         List<int2> toRemove = new List<int2>();
         for (;;)
        {
            var pos= camera.transform.position / ((N - 1)*pointOffset);
            cameraChunkPosition = new Vector2(pos.x,pos.z);
            //Debug.Log("Chunk Scan, cam pos: "+ cameraChunkPosition);
            toRemove.Clear();
            lock (chunksToRender)
            {
                foreach (var chunk in loadedChunks) //iterates over each element so yeee
                {//Vector.Distance to taka dosyć expensive funkcja bo to jest pierwiastek z kwadratu x i y a pierwiastki w obliczeniach na procesorze to przekleństwo
                    //może jak coś potem przerobię na sumę kwadratów bo jest wtedy bez pierwiastka
                    float distance = Vector2.Distance(cameraChunkPosition , new Vector2(chunk.Value.chunkData.position.x,chunk.Value.chunkData.position.y));
                    if (distance > REND*1.5)
                    {
                        toRemove.Add(chunk.Key);
                    }
                    /*else if (distance > REND*0.6 && chunk.Value.chunkData.lodLvl != 12 && distance < REND*1.5)
                    {
                        chunk.Value.SetLOD(12);
                    }
                    else if (distance > REND*0.4 && chunk.Value.chunkData.lodLvl != 8 && distance < REND*0.6)
                    {
                        chunk.Value.SetLOD(8);
                    }#1#
                    */
                    else if (distance > REND*0.8 && chunk.Value.chunkData.lodLvl != 6 && distance < REND*1.5)
                    {
                        chunk.Value.SetLOD(4);
                    }
                    else if (distance > REND*0.5 && chunk.Value.chunkData.lodLvl != 2 && distance < REND*0.8)
                    {
                        chunk.Value.SetLOD(2);
                    }
                    else if (distance > REND*0.1 && chunk.Value.chunkData.lodLvl != 1 && distance < REND*0.5)
                    {
                        chunk.Value.SetLOD(1);
                    }
                }
                
            }
            //usuwanie chunków
            Chunk chunkz;
            foreach(var VARIABLE in toRemove)
            {
                loadedChunks.TryGetValue(VARIABLE,out chunkz);
                Destroy(chunkz.gameObject); //dosyć EXPENSIVE eeee refrence to gameobject i samo usunięcie
                loadedChunks.Remove(VARIABLE);
            }
            //toRemove.Clear();

            //yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(2f); //cała funkcja wykonuje się co 1s
            // suspend execution for 1 second
        }
    }
}

public class ChunkData
{
    public int2 position;
    public int chunkSize;
    public Vector3[] vertecies;
    public float pointOffset;
    public int heihghtMultipl;
    public int[] triangles;
    public int lodLvl = 1;

    public ChunkData(int2 position, int chunkSize, Vector3[] vertecies, float pointOffset,int heightMultipl, int lodLvl)
    {
        this.position = position;
        this.chunkSize = chunkSize;
        this.vertecies = vertecies;
        this.pointOffset = pointOffset;
        this.lodLvl = lodLvl;
        this.heihghtMultipl = heihghtMultipl;
    }

    public ChunkData(int2 position, int chunkSize)
    {
        this.position = position;
        this.chunkSize = chunkSize;
    }

    public ChunkData(int2 position, int chunkSize, Vector3[] vertecies)
    {
        this.position = position;
        this.chunkSize = chunkSize;
        this.vertecies = vertecies;
    }
}
