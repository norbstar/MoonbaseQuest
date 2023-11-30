using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter.Tests
{
    public class RotarDriverSpeedTest : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] Transform rotars;

        [Header("Config")]
        [SerializeField] float rotarSpeed = 750f;

        // Update is called once per frame
        void Update() => rotars.transform.Rotate(Vector3.up * rotarSpeed * Time.deltaTime, Space.Self);
    }
}