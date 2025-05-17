using UnityEngine;

namespace Core.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class AnchorsAdjuster : MonoBehaviour
    {
        // ReSharper disable once InconsistentNaming
        public bool isLRSymmetrical = true;

        // ReSharper disable once InconsistentNaming
        public bool isUDSymmetrical;

        [Range(0f, 1f)] public float xMinAnchor;
        [Range(0f, 1f)] public float xMaxAnchor;
        [Range(0f, 1f)] public float yMinAnchor;
        [Range(0f, 1f)] public float yMaxAnchor;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();

            xMinAnchor = _rectTransform.anchorMin.x;
            xMaxAnchor = _rectTransform.anchorMax.x;
            yMinAnchor = _rectTransform.anchorMin.y;
            yMaxAnchor = _rectTransform.anchorMax.y;
        }

        // Maybe use context menu to update
        private void Update()
        {
            if (isLRSymmetrical) xMaxAnchor = 1 - xMinAnchor;
            if (isUDSymmetrical) yMaxAnchor = 1 - yMinAnchor;

            _rectTransform.anchorMin = new Vector2(xMinAnchor, yMinAnchor);
            _rectTransform.anchorMax = new Vector2(xMaxAnchor, yMaxAnchor);
        }
    }
}