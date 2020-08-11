using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public static class NavMeshExtensions
    {
        public static Vector3 RandomPosition(this NavMeshAgent agent, Vector3 basePosition, float radius)
        {
            var randDirection = Random.insideUnitSphere * radius;
            randDirection += basePosition;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randDirection, out navHit, radius, -1))
            {
                return navHit.position;
            }
            else
            {
                return basePosition;
            }
        }
    }
}