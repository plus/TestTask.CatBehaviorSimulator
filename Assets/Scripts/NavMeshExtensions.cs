using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public enum CloseType
    {
        Action = 1,
        VeryClose = 2
    }

    public static class NavMeshExtensions
    {
        public static bool RandomPosition(this NavMeshAgent agent, Vector3 basePosition, float radius, out Vector3 position)
        {
            var randDirection = Random.insideUnitSphere * radius;
            randDirection += basePosition;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randDirection, out navHit, radius, -1))
            {
                position = navHit.position;
                return true;
            }
            else
            {
                position = Vector3.zero;
                return false;
            }
        }

        public static bool IsClose(this Vector3 first, Vector3 second, CloseType closeType)
        {
            var firstXZ = new Vector3(first.x, 0, first.z);
            var secondXZ = new Vector3(second.x, 0, second.z);
            var magnitude = (firstXZ - secondXZ).magnitude;
            switch (closeType)
            {
                case CloseType.Action:
                    return magnitude < 2f;                    

                case CloseType.VeryClose:
                    return magnitude < 0.5f;
            }

            return false;
        }
    }
}