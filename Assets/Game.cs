using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plus.CatSimulator
{

    public class Game : MonoBehaviour
    {
        [SerializeField] private Text moodText;
        [SerializeField] private Text behaviourDescriptionText;

        private ICat cat;        

        private void Start()
        {
            cat = GameObject.FindObjectOfType<Cat>(); // TODO: check.
            cat.MoodChange += Cat_MoodChange;
            cat.BehaviourUpdate += Cat_BehaviourUpdate;

            UpdateMoodText(cat.Mood.ToString());
            UpdateBehaviourDescriptionText(cat.CurrentBehaviourDescription);
        }

        private void Cat_BehaviourUpdate(object sender, CatBehaviourArgs e)
        {
            UpdateBehaviourDescriptionText(e.BehaviourDescription);
        }

        private void Cat_MoodChange(object sender, CatMoodArgs e)
        {
            UpdateMoodText(e.Mood.ToString());            
        }

        private void UpdateBehaviourDescriptionText(string text)
        {
            behaviourDescriptionText.text = text;
        }

        private void UpdateMoodText(string text)
        {
            moodText.text = "Mood: " + text;
        }
    }
}