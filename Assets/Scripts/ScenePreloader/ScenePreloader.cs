using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Plus.CatSimulator
{
    public class ScenePreloader : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Text TextLoading;
        [SerializeField] private Button PlayButton;
        [SerializeField] private Image ProgressLine;
        #pragma warning disable 0649

        private bool buttonPressed;
        AsyncOperation asyncSceneLoading;

        private void Start()
        {
            StartCoroutine(SceneLoader());
        }

        private void Update()
        {
            if (asyncSceneLoading != null)
            {
                var progress = asyncSceneLoading.progress;
                UpdateLoadingText(progress);
                if (progress >= .9f)
                {
                    PlayButton.gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        IEnumerator SceneLoader()
        {
            yield return new WaitForSeconds(0.2f);

            asyncSceneLoading = SceneManager.LoadSceneAsync("SampleScene");
            asyncSceneLoading.allowSceneActivation = false;
        }

        private void UpdateLoadingText(float progress)
        {
            var progress01 = Mathf.Clamp01(progress / 0.9f);
            TextLoading.text = "Loading... " + Mathf.RoundToInt(progress01 * 100f) + "%";
            ProgressLine.fillAmount = progress01;
        }

        public void Play()
        {
            if (asyncSceneLoading.progress >= .9f)
                asyncSceneLoading.allowSceneActivation = true;
        }
    }
}