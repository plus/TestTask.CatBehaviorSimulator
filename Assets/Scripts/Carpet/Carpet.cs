using UnityEngine;

namespace Plus.CatSimulator
{
    public class Carpet : MonoBehaviour, ICarpet
    {
        public Vector3 Position => transform.position;

        #pragma warning disable 0649
        [SerializeField] private Transform puddle;
        #pragma warning restore 0649

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