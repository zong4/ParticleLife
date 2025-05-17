using UnityEngine;

namespace UI
{
    public static class DrawRect
    {
        public static Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
        {
            var x = Mathf.Min(screenPosition1.x, screenPosition2.x);
            var y = Screen.height - Mathf.Max(screenPosition1.y, screenPosition2.y);
            var width = Mathf.Abs(screenPosition1.x - screenPosition2.x);
            var height = Mathf.Abs(screenPosition1.y - screenPosition2.y);
            return new Rect(x, y, width, height);
        }

        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        }

        private static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}