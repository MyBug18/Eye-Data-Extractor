using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = 0;
            
            if (Input.GetKey(KeyCode.A)) h = -0.15f;
            if (Input.GetKey(KeyCode.D)) h = 0.15f;
            
            float v = 0.5f;
            if (Input.GetKey(KeyCode.W)) v = 1;
#if !MOBILE_INPUT
            float handbrake = 0;
            if (Input.GetKey(KeyCode.S)) handbrake = 0.7f;
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
