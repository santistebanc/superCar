using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public static class CrossPlatformInputManager
    {
        private static bool m_IsMobileInput = false;

        public static void SetMobileInput(bool mobile)
        {
            m_IsMobileInput = mobile;
        }

        public static float GetAxis(string axisName)
        {
            if (m_IsMobileInput)
            {
                return Input.GetAxis(axisName);
            }

            if (axisName == "Horizontal")
            {
                return Input.GetAxis("Horizontal");
            }
            else if (axisName == "Vertical")
            {
                return Input.GetAxis("Vertical");
            }
            else if (axisName == "Jump")
            {
                return Input.GetButton("Jump") ? 1f : 0f;
            }

            return Input.GetAxis(axisName);
        }

        public static bool GetButton(string buttonName)
        {
            return Input.GetButton(buttonName);
        }

        public static bool GetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }

        public static bool GetButtonUp(string buttonName)
        {
            return Input.GetButtonUp(buttonName);
        }
    }
}