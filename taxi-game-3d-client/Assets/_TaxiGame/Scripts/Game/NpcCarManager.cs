using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class NpcCarManager : MonoBehaviour
    {
        [SerializeField]
        GameObject[] carPrefabs;
        [SerializeField]
        PathCreator[] paths;
        [SerializeField]
        Transform poolPosition;
        [SerializeField]
        [Min(0.001f)]
        float minSpawnDelay = 1f;
        [SerializeField]
        [Min(0.001f)]
        float maxSpawnDelay = 1f;
        [SerializeField]
        float moveSpeed = 50f;

        HashSet<NpcCar> activeCars = new();
        Queue<NpcCar> carPool = new();

        public bool IsPlaying
        {
            get;
            private set;
        }

        public void Play()
        {
            IsPlaying = true;
            for (int i = 0; i < paths.Length; i++)
                StartCoroutine(WaitAndSpawn(i));
        }

        public void Stop()
        {
            IsPlaying = false;
            StopAllCoroutines();
        }

        void Start()
        {
            foreach (var prefab in carPrefabs)
            {
                prefab.transform.SetPositionAndRotation(
                    poolPosition.position, poolPosition.rotation
                );
            }
        }

        void Update()
        {
            if (!IsPlaying)
                return;

            var desapwns = new Queue<NpcCar>();
            var moveAmount = moveSpeed * Time.deltaTime;
            foreach (var car in activeCars)
            {
                car.UpdateMoving(moveAmount);
                if (car.IsArrive)
                    desapwns.Enqueue(car);
            }
            while (desapwns.Count > 0)
                Despawn(desapwns.Dequeue());
        }

        IEnumerator WaitAndSpawn(int pathIndex)
        {
            if (!IsPlaying)
                yield break;

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            
            NpcCar car = null;
            if (carPool.Count > 0)
            {
                car = carPool.Dequeue();
            }
            else
            {
                var go = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
                car = go.GetComponent<NpcCar>();
            }
            car.SetPath(paths[pathIndex].path);
            car.gameObject.SetActive(true);
            activeCars.Add(car);

            StartCoroutine(WaitAndSpawn(pathIndex));
        }

        void Despawn(NpcCar car)
        {
            car.gameObject.SetActive(false);
            car.transform.SetPositionAndRotation(
                poolPosition.position, poolPosition.rotation
            );
            activeCars.Remove(car);
            carPool.Enqueue(car);
        }
    }
}
