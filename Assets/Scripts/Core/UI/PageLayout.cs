using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    [ExecuteInEditMode]
    public class PageLayout : MonoBehaviour
    {
        public enum Layout
        {
            Vertical,
            Horizontal,
        }

        public Layout layout = Layout.Vertical;
        public List<float> sizePercent;

        public void OnValidate()
        {
            if (sizePercent != null && sizePercent.Count == transform.childCount)
                return;

            sizePercent ??= new List<float>();
            while (transform.childCount > sizePercent.Count)
            {
                sizePercent.Add(0);
            }

            if (transform.childCount < sizePercent.Count)
            {
                sizePercent.RemoveRange(transform.childCount, sizePercent.Count - transform.childCount);
            }
        }

        [ContextMenu("Reset Equal Size Percent")]
        private void ResetEqualSizePercent()
        {
            sizePercent = new List<float>();
            for (var i = 0; i < transform.childCount; i++)
            {
                sizePercent.Add(1.0f / transform.childCount);
            }
        }

        // Maybe use context menu to update
        private void Update()
        {
            var totalSize = 0f;
            for (var i = 0; i < Math.Min(transform.childCount, sizePercent.Count); i++)
            {
                var rectTransform = transform.GetChild(i).GetComponent<RectTransform>();
                switch (layout)
                {
                    case Layout.Vertical:
                        rectTransform.anchorMin = new Vector2(0, 1 - totalSize - sizePercent[i]);
                        rectTransform.anchorMax = new Vector2(1, 1 - totalSize);
                        break;
                    case Layout.Horizontal:
                        rectTransform.anchorMin = new Vector2(totalSize, 0);
                        rectTransform.anchorMax = new Vector2(totalSize + sizePercent[i], 1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                totalSize += sizePercent[i];
            }
        }
    }
}