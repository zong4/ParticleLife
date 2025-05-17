using UnityEngine;

namespace Core.UI
{
    [ExecuteInEditMode]
    public class GridsLayout : MonoBehaviour
    {
        public int rowNumber;
        public int columnNumber;

        [ContextMenu("Update Transforms")]
        public void UpdateTransforms()
        {
            for (var i = 0; i < rowNumber; i++)
            {
                for (var j = 0; j < columnNumber; j++)
                {
                    var rectTransform = transform.GetChild(i * columnNumber + j).GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2((float)j / columnNumber, 1 - (float)(i + 1) / rowNumber);
                    rectTransform.anchorMax = new Vector2((float)(j + 1) / columnNumber, 1 - (float)i / rowNumber);
                }
            }
        }
    }
}