using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public interface ICatAction
    {
        bool BehaviourWasFinished { get; }

        void Reset();
        void Run();        
    }

    public abstract class AbstractCatAction : ICatAction
    {
        public bool BehaviourWasFinished { get; protected set; }

        protected Animator animator;
        protected NavMeshAgent navMeshAgent;
        protected IBall ball;
        protected Transform transform;
        protected IPlayer player;
        protected AudioClip clipPurr;
        protected AudioSource audioSource;
        protected CatSpeedConfigure navMeshSpeedConfigure;
        protected ICarpet[] carpets;
        protected IRoom[] rooms;
        protected Queue<IFood> food;

        public AbstractCatAction(CatParams catParams)
        {
            animator = catParams.animator;
            navMeshAgent = catParams.navMeshAgent;
            ball = catParams.ball;
            transform = catParams.transform;
            player = catParams.player;
            clipPurr = catParams.clipPurr;
            audioSource = catParams.audioSource;
            navMeshSpeedConfigure = catParams.navMeshSpeedConfigure;
            carpets = catParams.carpets;
            rooms = catParams.rooms;
            food = catParams.food;
        }

        public abstract void Reset();
        public abstract void Run();        

        protected void SetSpeed(CatSpeed speed)
        {
            switch (speed)
            {
                case CatSpeed.Default:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.DefaultAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.DefaultSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.DefaultAngularSpeed;
                    break;
                case CatSpeed.Fast:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.FastAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.FastSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.FastAngularSpeed;
                    break;
                case CatSpeed.SuperFast:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.SuperFastAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.SuperFastSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.SuperFastAngularSpeed;
                    break;
            }
        }
    }
}