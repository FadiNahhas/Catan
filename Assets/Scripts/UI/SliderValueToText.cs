using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SliderValueToText : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;

        private void OnEnable()
        {
            slider.onValueChanged.AddListener(UpdateText);
            UpdateText(slider.value);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(UpdateText);
        }

        private void UpdateText(float value)
        {
            text.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}