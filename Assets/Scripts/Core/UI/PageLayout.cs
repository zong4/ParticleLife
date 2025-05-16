using System;
using UnityEngine;

namespace Core.UI
{
    [ExecuteInEditMode]
    public class PageLayout : MonoBehaviour
    {
        public bool isVertical = true;
        public float[] sizePercent;

        public void OnValidate()
        {
            if (sizePercent != null && sizePercent.Length == transform.childCount)
                return;

            ResetEqualSizePercent();
        }

        [ContextMenu("Reset Equal Size Percent")]
        private void ResetEqualSizePercent()
        {
            sizePercent = new float[transform.childCount];

            for (var i = 0; i < sizePercent.Length; i++)
            {
                sizePercent[i] = 1.0f / sizePercent.Length;
            }
        }

        private void Update()
        {
            var totalSize = 0f;
            for (var i = 0; i < Math.Min(transform.childCount, sizePercent.Length); i++)
            {
                var rectTransform = transform.GetChild(i).GetComponent<RectTransform>();
                if (isVertical)
                {
                    rectTransform.anchorMin = new Vector2(0, 1 - totalSize - sizePercent[i]);
                    rectTransform.anchorMax = new Vector2(1, 1 - totalSize);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(totalSize, 0);
                    rectTransform.anchorMax = new Vector2(totalSize + sizePercent[i], 1);
                }

                totalSize += sizePercent[i];
            }
        }
    }
}