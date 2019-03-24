using UnityEngine;

namespace AbruptLib
{
    public class EditorLikeCamera : MonoBehaviour
    {
        private static readonly string[] DESCRIPTION = new string[]
        {
        "WASD to move",
        "RMB to rotate",
        "Space, Ctrl to jump",
        "Shift to accelerate"
        };

        public bool DrawInfo = true;
        public bool ScaleTimeByNumKeys;
        public float LiniarSpeed = 10;
        public float AngularRotationArc = 130;

        private Vector3 _lastMousePosition;

        private void Update()
        {
            SimulateMotion(Time.deltaTime);
            SimulateRotation();

            if (ScaleTimeByNumKeys)
            {
                if (Input.GetKeyDown(KeyCode.KeypadPlus)) Time.timeScale = Mathf.Min(Time.timeScale * 3, 81);
                if (Input.GetKeyDown(KeyCode.KeypadMinus)) Time.timeScale = Mathf.Max(Time.timeScale / 3, 1 / 9);
            }
        }

        private void SimulateMotion(float deltaTime)
        {
            var motion = Vector3.zero;
            var jump = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) motion.z = 1;
            if (Input.GetKey(KeyCode.S)) motion.z = -1;
            if (Input.GetKey(KeyCode.D)) motion.x = 1;
            if (Input.GetKey(KeyCode.A)) motion.x = -1;
            if (Input.GetKey(KeyCode.Space)) jump.y = 1;
            if (Input.GetKey(KeyCode.LeftControl)) jump.y = -1;
            motion = deltaTime * LiniarSpeed * motion.normalized;
            jump = deltaTime * LiniarSpeed * jump;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                motion *= 5;
                jump *= 5;
            }
            transform.Translate(motion);
            transform.Translate(jump, Space.World);
        }

        private void SimulateRotation()
        {
            if (Input.GetMouseButtonDown(1))
                _lastMousePosition = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                var mouseDelta = Input.mousePosition - _lastMousePosition;
                var x = Mathf.Abs(mouseDelta.x / Screen.width) * AngularRotationArc * Mathf.Sign(mouseDelta.x);
                var y = -Mathf.Abs(mouseDelta.y / Screen.height) * AngularRotationArc * Mathf.Sign(mouseDelta.y);
                transform.Rotate(Vector3.right, y);
                transform.Rotate(0, x, 0, Space.World);
                _lastMousePosition = Input.mousePosition;
            }
        }

        private void OnGUI()
        {
            if (!DrawInfo) return;
            DrawTextLines(DESCRIPTION);
            GUI.TextField(new Rect(Screen.width - 120, Screen.height - 24, 120, 24), string.Format("Time: {0:N2}", Time.time));
            GUI.TextField(new Rect(Screen.width - 150, Screen.height - 24, 30, 24), "x" + Time.timeScale);
        }

        private void DrawTextLines(string[] lines)
        {
            var lineHeight = 24;
            var lineWidth = 256;
            for (var i = 0; i < lines.Length; i++)
                GUI.TextField(new Rect(0, lineHeight * i, lineWidth, lineHeight), lines[i]);
        }
    }
}