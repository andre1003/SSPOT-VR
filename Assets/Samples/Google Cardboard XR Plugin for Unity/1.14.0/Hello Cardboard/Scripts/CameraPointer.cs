//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour {
    public CrosshairController crosshairController; // Reference to CrosshairController script

    public Camera uiCamera;

    // Player
    public GameObject playerHands; // Player hand

    private const float _maxDistance = 100;
    private GameObject _gazedAtObject = null;

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update() {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance)) {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject) {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExit", SendMessageOptions.DontRequireReceiver);
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter", SendMessageOptions.DontRequireReceiver);

                if(_gazedAtObject.CompareTag("Clickable"))
                    crosshairController.SetCrosshairScale(new Vector3(1.5f, 1.5f, 1.5f));
            }
        }
        else {
            // No GameObject detected in front of the camera.
            _gazedAtObject?.SendMessage("OnPointerExit", SendMessageOptions.DontRequireReceiver);
            _gazedAtObject = null;
            crosshairController.SetCrosshairScale(new Vector3(1f, 1f, 1f));
        }

        // Checks for screen touches.
        if (Google.XR.Cardboard.Api.IsTriggerPressed || /*Input.GetTouch(0).phase == TouchPhase.Began ||*/ Input.GetButtonDown("Fire1")) {
            // If is a non clickable area and there is any cube in player hands, remove it
            if((!_gazedAtObject ||
                (!_gazedAtObject.CompareTag("Clickable") && !_gazedAtObject.CompareTag("NoPointerAction")))
                && playerHands.transform.childCount == 1)
            {
                Destroy(playerHands.transform.GetChild(0).gameObject);
            }
            // If not, call OnPointerClick method
            else
            {
                _gazedAtObject?.SendMessage("OnPointerClick", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
