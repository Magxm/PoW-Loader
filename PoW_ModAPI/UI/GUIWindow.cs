using System;
using UnityEngine;

namespace PoW_ModAPI.UI
{

    class GUIWindow
    {
        private string text;
        private float width, height, margin, controlHeight, controlDist, nextControlYOffset;
        private float indentation = 0;
        private float windowX;
        private float windowY;

        //private GUIStyle windowStyle = new GUIStyle(GUI.skin.window);

        public GUIWindow(string _text, float _x, float _y, float _width, float _height, float _margin, float _controlHeight, float _controlDist)
        {
            text = _text;
            windowX = _x;
            windowY = _y;
            width = _width;
            height = _height;
            margin = _margin;
            controlHeight = _controlHeight;
            controlDist = _controlDist;
            nextControlYOffset = 20f;
        }

        readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        public void Render(GUI.WindowFunction onDraw)
        {
            nextControlYOffset = 20f;

            GUI.matrix = Matrix4x4.Scale(Vector3.one);

            //GUI.Box(windowRect, text);
            GUI.backgroundColor = Color.black;
            Rect newWindowRect = GUI.Window(0, new Rect(windowX, windowY, width, height), onDraw, text);
            windowX = newWindowRect.x;
            windowY = newWindowRect.y;
        }

        public void MakeDragable()
        {
            GUI.DragWindow();
        }

        public void Indent(float pixel)
        {
            indentation += pixel;
        }
        public void Unindent(float pixel)
        {
            indentation -= pixel;
        }

        private Rect NextControlRect()
        {
            Rect r = new Rect(windowX + margin + indentation, windowY + nextControlYOffset, width - margin * 2, controlHeight);
            nextControlYOffset += controlHeight + controlDist;

            return r;
        }

        public string MakeEnable(string text, bool state)
        {
            return string.Format("{0}{1}", text, state ? "ON" : "OFF");
        }

        public bool Button(string text, bool state)
        {
            return Button(MakeEnable(text, state));
        }

        public bool Button(string text)
        {
            return GUI.Button(NextControlRect(), text);
        }

        public void Label(string text, float value, int decimals = 2)
        {
            Label(string.Format("{0}{1}", text, Math.Round(value, 2).ToString()));
        }

        public void Label(string text)
        {
            GUI.Label(NextControlRect(), text);
        }

        public float Slider(float val, float min, float max)
        {
            return GUI.HorizontalSlider(NextControlRect(), val, min, max);
        }
    }
}
