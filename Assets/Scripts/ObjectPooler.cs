using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary< string, Queue<GameObject> > poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary< string, Queue<GameObject> >(); 
        foreach( Pool pool in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for(int i=0; i<pool.size; i++){
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }       
    }

    public GameObject SpawnFromPool(string poolTag, Vector2 position)
    {
        if( !poolDictionary.ContainsKey(poolTag) ){
            Debug.LogWarning("Invalid pool tag");
            return null;
        }
        GameObject ObjectToSpawn = poolDictionary[poolTag].Dequeue();

        ObjectToSpawn.SetActive(true);
        ObjectToSpawn.transform.position = position;

        poolDictionary[poolTag].Enqueue(ObjectToSpawn);

        return ObjectToSpawn;
    }


}
