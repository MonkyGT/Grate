﻿using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using Grate.Gestures;
using Grate.GUI;
using Grate.Networking;
using Grate.Tools;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.XR;

namespace Grate.Modules.Movement
{
    class Frozone : GrateModule
    {
        public static GameObject IcePrefab;
        public static Vector3 LhandOffset = Vector3.down * 0.05f;
        public static Vector3 RhandOffset = new Vector3(0.05f, -0.1f, 0);
        private  List<GameObject> prevRIce = new List<GameObject>();
        private  List<GameObject> prevLIce = new List<GameObject>();
        Transform leftHandTransform => VRRig.LocalRig.leftHandTransform;
        Transform rightHandTransform => VRRig.LocalRig.rightHandTransform;

        InputTracker inputL, inputR;

        bool leftPress, rightPress;

        public override string GetDisplayName()
        {
            return "Frozone";
        }
        public override string Tutorial()
        {
            return "Like Platforms but you slide!";
        }
        protected override void Start()
        {
            base.Start();
            IcePrefab = Plugin.assetBundle.LoadAsset<GameObject>("Ice");
            IcePrefab.GetComponent<BoxCollider>().enabled = true;
            IcePrefab.AddComponent<GorillaSurfaceOverride>().overrideIndex = 59;
        }


        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            inputL = GestureTracker.Instance.GetInputTracker("grip", XRNode.LeftHand);
            inputL.OnPressed += OnActivate;
            inputL.OnReleased += OnDeactivate;

            inputR = GestureTracker.Instance.GetInputTracker("grip", XRNode.RightHand);
            inputR.OnPressed += OnActivate;
            inputR.OnReleased += OnDeactivate;
        }

        private void OnActivate(InputTracker tracker)
        {
            if (tracker.node == XRNode.LeftHand)
            {
                leftPress = true;
            }
            if (tracker.node == XRNode.RightHand)
            {
                rightPress = true;
            }
        }
        void Unsub()
        {
            if (inputL != null)
            {
                inputL.OnPressed -= OnActivate;
                inputL.OnReleased -= OnDeactivate;
            }
            if (inputR != null)
            {
                inputR.OnPressed -= OnActivate;
                inputR.OnReleased -= OnDeactivate;
            }
        }
        private void OnDeactivate(InputTracker tracker)
        {
            if (tracker.node == XRNode.LeftHand )
            {
                leftPress = false;
            }
            if (tracker.node == XRNode.RightHand)
            {
                rightPress = false;
            }
        }
        private void FixedUpdate()
        {
            if (leftPress)
            {
                if (prevLIce.Count > 19)
                {
                    GameObject ice = prevLIce[0];
                    prevLIce.RemoveAt(0);
                    ice.SetActive(true);
                    ice.transform.position = leftHandTransform.position + LhandOffset;
                    ice.transform.rotation = leftHandTransform.rotation;
                    prevLIce.Add(ice);
                }
                else
                {
                    GameObject ice = Instantiate(IcePrefab);
                    ice.transform.position = leftHandTransform.position + LhandOffset;
                    ice.transform.rotation = leftHandTransform.rotation;
                    prevLIce.Add(ice);
                }
            }
            else
            {
                foreach (GameObject ice in prevLIce)
                {
                    ice.SetActive(false);
                }
            }
            if (rightPress)
            {
                if (prevRIce.Count >= 20)
                {
                    GameObject ice = prevRIce[0];
                    prevRIce.RemoveAt(0);
                    ice.SetActive(true);

                    ice.transform.position = rightHandTransform.position + RhandOffset;
                    ice.transform.rotation = rightHandTransform.rotation;
                    prevRIce.Add(ice);
                }
                else
                {
                    GameObject ice = Instantiate(IcePrefab);
                    ice.transform.position = rightHandTransform.position + RhandOffset;
                    ice.transform.rotation = rightHandTransform.rotation;
                    prevRIce.Add(ice);
                }
            }
            else
            {
                foreach (GameObject ice in prevRIce)
                {
                    ice.SetActive(false);
                }
            }
        }
        protected override void Cleanup()
        {
            Unsub();
        }

        class PlatformSpawner : MonoBehaviour
        {
            public VRRig rig;
            public List<GameObject> prevRIce = new List<GameObject>();
            public List<GameObject> prevLIce = new List<GameObject>();
            NetworkedPlayer networkedPlayer;
            Transform leftHandTransform => rig.leftHandTransform;
            private Transform rightHandTransform => rig.rightHandTransform;

            bool leftPress, rightPress;

            void Start()
            {
                networkedPlayer = rig.GetComponent<NetworkedPlayer>();
                networkedPlayer.OnGripPressed += OnGripPressed;
                networkedPlayer.OnGripReleased += OnGripReleased;
            }

            private void OnGripReleased(NetworkedPlayer player, bool forLeft)
            {
                if (forLeft)
                {
                    leftPress = true;
                }
                else
                {
                    rightPress = true;
                }
            }

            private void OnGripPressed(NetworkedPlayer player, bool forLeft)
            {
                if (forLeft)
                {
                    leftPress = false;
                }
                else
                {
                    rightPress = false;
                }
            }

            private void FixedUpdate()
            {
                if (leftPress)
                {
                    if (prevLIce.Count > 19)
                    {
                        GameObject ice = prevLIce[0];
                        prevLIce.RemoveAt(0);
                        ice.SetActive(true);
                        ice.transform.position = leftHandTransform.position + LhandOffset;
                        ice.transform.rotation = leftHandTransform.rotation;
                        prevLIce.Add(ice);
                    }
                    else
                    {
                        GameObject ice = Instantiate(IcePrefab);
                        ice.transform.position = leftHandTransform.position + RhandOffset;
                        ice.transform.rotation = leftHandTransform.rotation;
                        prevLIce.Add(ice);
                    }
                }
                else
                {
                    foreach (GameObject ice in prevLIce)
                    {
                        ice.SetActive(false);
                    }
                }
                if (rightPress)
                {
                    if (prevRIce.Count > 19)
                    {
                        GameObject ice = prevRIce[0];
                        prevRIce.RemoveAt(0);
                        ice.SetActive(true);

                        ice.transform.position = rightHandTransform.position + LhandOffset;
                        ice.transform.rotation = rightHandTransform.rotation;
                        prevRIce.Add(ice);
                    }
                    else
                    {
                        GameObject ice = Instantiate(IcePrefab);
                        ice.transform.position = rightHandTransform.position + LhandOffset;
                        ice.transform.rotation = rightHandTransform.rotation;
                        prevRIce.Add(ice);
                    }
                }
                else
                {
                    foreach (GameObject ice in prevRIce)
                    {
                        ice.SetActive(false);
                    }
                }
            }
        }
    }
}

