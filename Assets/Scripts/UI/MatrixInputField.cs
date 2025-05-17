using UnityEngine;

namespace UI
{
    public class MatrixInputField : MonoBehaviour
    {
        public ColorConfigUI colorConfigUI;

        public int row;
        public int column;

        public void SetMatrix(string str)
        {
            if (!float.TryParse(str, out var result)) return;

            colorConfigUI.AttractionMatrix[row, column] = result;
            colorConfigUI.SetAttractionMatrix();
        }
    }
}