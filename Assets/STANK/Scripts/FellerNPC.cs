using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace STANK {
    public class FellerNPC : MonoBehaviour
    {
        Feller feller;
        NavMeshAgent agent;
        [SerializeField] Transform[] waypoints;

        private System.Random _random = new System.Random();

        public Transform[] ShuffleWaypoints(Transform[] array)
        {
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = _random.Next(0, n);
                Transform t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
            return array;
        }

        // Start is called before the first frame update
        void Start()
        {
            feller = GetComponentInChildren<Feller>();
            agent = GetComponent<NavMeshAgent>();
            SetNewDestination();
        }

        void SetNewDestination(){
            ShuffleWaypoints(waypoints);
            agent.SetDestination(waypoints[0].position);
        }

        // Update is called once per frame
        void Update()
        {
            if(Vector3.Distance(transform.position, waypoints[0].position) < 5f){
                SetNewDestination();
            }
        }
    }
}