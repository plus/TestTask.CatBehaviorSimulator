using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plus.CatSimulator
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Text moodText;
        [SerializeField] private Text behaviourDescriptionText;
        [SerializeField] private Image imageMoodBad;
        [SerializeField] private Image imageMoodGood;
        [SerializeField] private Image imageMoodGreat;

        private ICat cat;

        private void Start()
        {
            cat = GameObject.FindObjectOfType<Cat>();
            if (cat is null) throw new Exception("<Cat> no found in scene");
            cat.MoodChange += Cat_MoodChange;
            cat.BehaviourUpdate += Cat_BehaviourUpdate;

            Cat_MoodChange(this, new CatMoodArgs(cat.Mood));

            UpdateBehaviourDescriptionText(cat.CurrentBehaviourDescription);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void Cat_BehaviourUpdate(object sender, CatBehaviourArgs e)
        {
            UpdateBehaviourDescriptionText(e.BehaviourDescription);
        }

        private void Cat_MoodChange(object sender, CatMoodArgs e)
        {
            UpdateMoodText(e.Mood.ToString());
            UpdateMoodBar(e.Mood);
        }

        private void UpdateMoodBar(CatMood mood)
        {
            switch (mood)
            {
                case CatMood.Bad:
                    imageMoodBad.color = Color.red;
                    imageMoodGood.color = Color.gray;
                    imageMoodGreat.color = Color.gray;
                    break;
                case CatMood.Good:
                    imageMoodBad.color = Color.yellow;
                    imageMoodGood.color = Color.yellow;
                    imageMoodGreat.color = Color.gray;
                    break;
                case CatMood.Great:
                    imageMoodBad.color = Color.green;
                    imageMoodGood.color = Color.green;
                    imageMoodGreat.color = Color.green;
                    break;
            }
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