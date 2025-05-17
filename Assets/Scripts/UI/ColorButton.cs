using UnityEngine;

namespace UI
{
    public class ColorButton : MonoBehaviour
    {
        public int colorIndex;

        public ColorConfigUI colorConfigUI;

        public void EnterPickColor()
        {
            colorConfigUI.pickerIndex = colorIndex;
            colorConfigUI.colorPicker.SetActive(true);
        }
    }
}