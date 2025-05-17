using UnityEngine;

namespace UI
{
    public class ColorButton : MonoBehaviour
    {
        public ColorConfigUI colorConfigUI;

        public int colorIndex;

        public void EnterPickColor()
        {
            colorConfigUI.pickerIndex = colorIndex;

            // Open the color picker
            colorConfigUI.colorPicker.SetActive(true);
        }
    }
}