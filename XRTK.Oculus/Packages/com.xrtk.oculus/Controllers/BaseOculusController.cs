﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Oculus.Controllers
{
    public class BaseOculusController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="controllerType"></param>
        /// <param name="nodeType"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public BaseOculusController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            OculusApi.Controller controllerType,
            OculusApi.Node nodeType,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            ControllerType = controllerType;
            NodeType = nodeType;
        }

        public OculusApi.Node NodeType { get; } = OculusApi.Node.None;

        public OculusApi.Controller ControllerType { get; } = OculusApi.Controller.None;

        private OculusApi.ControllerState4 previousState = new OculusApi.ControllerState4();
        private OculusApi.ControllerState4 currentState = new OculusApi.ControllerState4();

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Button.A Press", AxisType.Digital, DeviceInputType.ButtonPress, "A"),
            new MixedRealityInteractionMapping(1, "Button.B Press", AxisType.Digital, DeviceInputType.ButtonPress, "B"),
            new MixedRealityInteractionMapping(2, "Button.X Press", AxisType.Digital, DeviceInputType.ButtonPress, "X"),
            new MixedRealityInteractionMapping(3, "Button.Y Press", AxisType.Digital, DeviceInputType.ButtonPress, "Y"),
            new MixedRealityInteractionMapping(4, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, "Start"),
            new MixedRealityInteractionMapping(5, "Button.Back Press", AxisType.Digital, DeviceInputType.ButtonPress, "Back"),
            new MixedRealityInteractionMapping(6, "Button.LShoulder Press", AxisType.Digital, DeviceInputType.ButtonPress, "LShoulder"),
            new MixedRealityInteractionMapping(7, "Axis1D.LIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, "LIndexTrigger"),
            new MixedRealityInteractionMapping(8, "Axis1D.LIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, "LIndexTrigger"),
            new MixedRealityInteractionMapping(9, "Axis1D.LIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, "LIndexTrigger"),
            new MixedRealityInteractionMapping(10, "Axis1D.LIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, "LIndexTrigger"),
            new MixedRealityInteractionMapping(11, "Axis1D.LHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, "LHandTrigger"),
            new MixedRealityInteractionMapping(12, "Axis2D.LThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, "PrimaryThumbstick"),
            new MixedRealityInteractionMapping(13, "Button.LThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, "LThumbstick"),
            new MixedRealityInteractionMapping(14, "Button.LThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, "LThumbstick"),
            new MixedRealityInteractionMapping(15, "Button.LThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "LThumbstick"),
            new MixedRealityInteractionMapping(16, "Button.RShoulder Press", AxisType.Digital, DeviceInputType.ButtonPress, "RShoulder"),
            new MixedRealityInteractionMapping(17, "Axis1D.RIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, "RIndexTrigger"),
            new MixedRealityInteractionMapping(18, "Axis1D.RIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, "RIndexTrigger"),
            new MixedRealityInteractionMapping(19, "Axis1D.RIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, "RIndexTrigger"),
            new MixedRealityInteractionMapping(20, "Axis1D.RIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, "RIndexTrigger"),
            new MixedRealityInteractionMapping(21, "Axis1D.RHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, "RHandTrigger"),
            new MixedRealityInteractionMapping(22, "Axis2D.RThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, "SecondaryThumbstick"),
            new MixedRealityInteractionMapping(23, "Button.RThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, "RThumbstick"),
            new MixedRealityInteractionMapping(24, "Button.RThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, "RThumbstick"),
            new MixedRealityInteractionMapping(25, "Button.RThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "RThumbstick"),
            new MixedRealityInteractionMapping(26, "Axis2D.Dpad", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(27, "Button.DpadUp Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "DpadUp"),
            new MixedRealityInteractionMapping(28, "Button.DpadDown Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "DpadDown"),
            new MixedRealityInteractionMapping(29, "Button.DpadLeft Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "DpadLeft"),
            new MixedRealityInteractionMapping(30, "Button.DpadRight Press", AxisType.Digital, DeviceInputType.ThumbStickPress, "DpadRight"),
            new MixedRealityInteractionMapping(31, "Button.RTouchpad", AxisType.Digital, DeviceInputType.ThumbTouch, "RTouchpad"),
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;
        private float singleAxisValue = 0.0f;
        private Vector2 dualAxisPosition = Vector2.zero;

        /// <summary>
        /// Updates the controller's interaction mappings and ready the current input values.
        /// </summary>
        public void UpdateController()
        {
            if (!Enabled) { return; }

            UpdateControllerData();

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for Windows Mixed Reality Motion Controller {ControllerHandedness}");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        UpdatePoseData(Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.ButtonPress:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.ThumbStickPress:
                        UpdateButtonDataPress(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonTouch:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.ThumbStickTouch:
                        UpdateButtonDataTouch(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonNearTouch:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.ThumbNearTouch:
                    case DeviceInputType.TouchpadNearTouch:
                    case DeviceInputType.ThumbStickNearTouch:
                        UpdateButtonDataNearTouch(Interactions[i]);
                        break;
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.Touchpad:
                        UpdateDualAxisData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [{GetType().Name}]");
                        break;
                }
            }
        }

        private void UpdateControllerData()
        {
            var lastState = TrackingState;
            lastControllerPose = currentControllerPose;
            previousState = currentState;

            currentState = OculusApi.GetControllerState4((uint)ControllerType);

            if (currentState.LIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LIndexTrigger;
            }

            if (currentState.LHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LHandTrigger;
            }

            if (currentState.LThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickUp;
            }

            if (currentState.LThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickDown;
            }

            if (currentState.LThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickLeft;
            }

            if (currentState.LThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickRight;
            }

            if (currentState.RIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RIndexTrigger;
            }

            if (currentState.RHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RHandTrigger;
            }

            if (currentState.RThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickUp;
            }

            if (currentState.RThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickDown;
            }

            if (currentState.RThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickLeft;
            }

            if (currentState.RThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
            {
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickRight;
            }

            if (IsTrackedController(ControllerType))
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = OculusApi.GetNodePositionTracked(NodeType);
                IsPositionApproximate = IsPositionAvailable && OculusApi.GetNodePositionValid(NodeType);
                IsRotationAvailable = OculusApi.GetNodeOrientationTracked(NodeType);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            var pose = OculusApi.GetNodePose(NodeType, OculusApi.stepType);

            currentControllerPose = pose.ToMixedRealityPose();

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastControllerPose != currentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentControllerPose.Position);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentControllerPose.Rotation);
                }
            }
        }

        private static bool IsTrackedController(OculusApi.Controller controller)
        {
            return controller == OculusApi.Controller.Touch ||
                   controller == OculusApi.Controller.LTouch ||
                   controller == OculusApi.Controller.RTouch ||
                   controller == OculusApi.Controller.LTrackedRemote ||
                   controller == OculusApi.Controller.RTrackedRemote;
        }

        private void UpdateButtonDataPress(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            Enum.TryParse<OculusApi.RawButton>(interactionMapping.InputName, out var interactionButton);

            if (interactionButton != OculusApi.RawButton.None)
            {
                if (((OculusApi.RawButton)previousState.Buttons & interactionButton) != 0)
                {
                    interactionMapping.BoolData = false;
                }

                if (((OculusApi.RawButton)currentState.Buttons & interactionButton) != 0 &&
                    ((OculusApi.RawButton)previousState.Buttons & interactionButton) == 0)
                {
                    interactionMapping.BoolData = true;
                }

                interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);
            }
        }

        private void UpdateButtonDataTouch(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            Enum.TryParse<OculusApi.RawTouch>(interactionMapping.InputName, out var interactionButton);

            if (interactionButton != OculusApi.RawTouch.None)
            {
                if (((OculusApi.RawTouch)previousState.Touches & interactionButton) != 0)
                {
                    interactionMapping.BoolData = false;
                }

                if (((OculusApi.RawTouch)currentState.Touches & interactionButton) != 0 &&
                    ((OculusApi.RawTouch)previousState.Touches & interactionButton) == 0)
                {
                    interactionMapping.BoolData = true;
                }

                interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);
            }
        }

        private void UpdateButtonDataNearTouch(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            Enum.TryParse<OculusApi.RawNearTouch>(interactionMapping.InputName, out var interactionButton);

            if (interactionButton != OculusApi.RawNearTouch.None)
            {
                if (((OculusApi.RawNearTouch)previousState.NearTouches & interactionButton) != 0)
                {
                    interactionMapping.BoolData = false;
                }

                if (((OculusApi.RawNearTouch)currentState.NearTouches & interactionButton) != 0 &&
                    ((OculusApi.RawNearTouch)previousState.NearTouches & interactionButton) == 0)
                {
                    interactionMapping.BoolData = true;
                }

                interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);
            }
        }

        private void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            Enum.TryParse<OculusApi.RawAxis1D>(interactionMapping.InputName, out var interactionAxis1D);

            if (interactionAxis1D != OculusApi.RawAxis1D.None)
            {
                switch (interactionAxis1D)
                {
                    case OculusApi.RawAxis1D.LIndexTrigger:
                        singleAxisValue = currentState.LIndexTrigger;

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                    case OculusApi.RawAxis1D.LHandTrigger:
                        singleAxisValue = currentState.LHandTrigger;

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                    case OculusApi.RawAxis1D.RIndexTrigger:
                        singleAxisValue = currentState.RIndexTrigger;

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                    case OculusApi.RawAxis1D.RHandTrigger:

                        singleAxisValue = currentState.RHandTrigger;

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                }
            }

            // Update the interaction data source
            interactionMapping.FloatData = singleAxisValue;

            interactionMapping.UpdateInteractionMappingFloat(InputSource, ControllerHandedness);
        }


        private void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            Enum.TryParse<OculusApi.RawAxis2D>(interactionMapping.InputName, out var interactionAxis2D);

            if (interactionAxis2D != OculusApi.RawAxis2D.None)
            {
                switch (interactionAxis2D)
                {
                    case OculusApi.RawAxis2D.LThumbstick:
                        dualAxisPosition.x = currentState.LThumbstick.x;
                        dualAxisPosition.y = currentState.LThumbstick.y;

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;
                    case OculusApi.RawAxis2D.LTouchpad:
                        dualAxisPosition.x = currentState.LTouchpad.x;
                        dualAxisPosition.y = currentState.LTouchpad.y;

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;
                    case OculusApi.RawAxis2D.RThumbstick:
                        dualAxisPosition.x = currentState.RThumbstick.x;
                        dualAxisPosition.y = currentState.RThumbstick.y;

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;
                    case OculusApi.RawAxis2D.RTouchpad:
                        dualAxisPosition.x = currentState.RTouchpad.x;
                        dualAxisPosition.y = currentState.RTouchpad.y;

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;
                }
            }

            // Update the interaction data source
            interactionMapping.Vector2Data = dualAxisPosition;

            interactionMapping.UpdateInteractionMappingVector2(InputSource, ControllerHandedness);
        }

        private void UpdatePoseData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            if (interactionMapping.InputType != DeviceInputType.SpatialPointer)
            {
                Debug.LogError($"Input [{interactionMapping.InputType}] is not handled for this controller [{GetType().Name}]");
                return;
            }

            currentPointerPose = currentControllerPose;

            // Update the interaction data source
            interactionMapping.PoseData = currentPointerPose;

            interactionMapping.UpdateInteractionMappingPose(InputSource, ControllerHandedness);
        }
    }
}