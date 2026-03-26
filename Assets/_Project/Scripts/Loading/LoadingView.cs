using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Displays loading progress bar and percentage text.
    /// Attach to Canvas in LoadingScene.
    /// </summary>
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private Slider          _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;

        public void UpdateProgress(float progress)
        {
            // progress is 0.0 to 1.0
            if (_progressBar != null)
                _progressBar.value = progress;

            if (_progressText != null)
                _progressText.text =
                    $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }
}