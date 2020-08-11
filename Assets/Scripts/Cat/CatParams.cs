using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public struct CatParams
    {
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public IBall ball;
        public Transform transform;
        public IPlayer player;
        public AudioClip clipPurr;
        public AudioSource audioSource;
        public CatSpeedConfigure navMeshSpeedConfigure;
        public ICarpet[] carpets;
        public IRoom[] rooms;
        public Queue<IFood> food;

        public CatParams(Animator animator, NavMeshAgent navMeshAgent, IBall ball, Transform transform, IPlayer player, AudioClip clipPurr,
            AudioSource audioSource, CatSpeedConfigure navMeshSpeedConfigure, ICarpet[] carpets, IRoom[] rooms, Queue<IFood> food)
        {
            this.animator = animator;
            this.navMeshAgent = navMeshAgent;
            this.ball = ball;
            this.transform = transform;
            this.player = player;
            this.clipPurr = clipPurr;
            this.audioSource = audioSource;
            this.navMeshSpeedConfigure = navMeshSpeedConfigure;
            this.carpets = carpets;
            this.rooms = rooms;
            this.food = food;
        }
    }
}