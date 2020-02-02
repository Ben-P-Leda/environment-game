using System;
using UnityEngine;

namespace Player
{
    public class ProximityDetector : MonoBehaviour
    {
        public event Action<bool> EnteredProximity;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Proximity Collider")
            {
                EnteredProximity?.Invoke(true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.tag == "Proximity Collider")
            {
                EnteredProximity?.Invoke(false);
            }
        }
    }
}