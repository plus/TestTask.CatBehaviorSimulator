using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plus.CatSimulator
{
    public interface ICarpet
    {
        Transform Transform { get; }
        void PissOnMe();
    }

    public class Carpet : MonoBehaviour, ICarpet
    {
        public Transform Transform => transform;

        [SerializeField] private Transform puddle;
        private float puddleScale = 0f;

        private void Start()
        {
            UpdatePuddleScale();
        }

        public void PissOnMe()
        {
            puddleScale = Mathf.Clamp01(puddleScale + .1f);
            UpdatePuddleScale();
        }

        private void UpdatePuddleScale()
        {
            puddle.localScale = Vector3.one * puddleScale;
        }
    }
}