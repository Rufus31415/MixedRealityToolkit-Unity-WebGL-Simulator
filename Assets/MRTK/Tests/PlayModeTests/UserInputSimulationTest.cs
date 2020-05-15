﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class UserInputSimulationTest
    {
        GameObject cube;
        Interactable interactable;

        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();

            // Explicitly enable user input to test in editor behavior.
            InputSimulationService iss = PlayModeTestUtilities.GetInputSimulationService();
            Assert.IsNotNull(iss, "InputSimulationService is null!");
            iss.UserInputEnabled = true;


            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, 2);
            cube.transform.localScale = new Vector3(.2f, .2f, .2f);

            interactable = cube.AddComponent<Interactable>();
            
            KeyInputSystem.StartKeyInputStimulation();
        }

        [TearDown]
        public void TearDown()
        {
            KeyInputSystem.StopKeyInputSimulation();
            PlayModeTestUtilities.TearDown();
        }
        
        [UnityTest]
        public IEnumerator HandsFreeInteractionTest()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // start click on the cube
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // release the click on the cube
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Check to see that the cube was clicked on
            Assert.True(wasClicked);
        }

        [UnityTest]
        public IEnumerator ArticulatedHandGestureTest()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Begin right hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure right hand is tracked
            Assert.True(iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct hand is pinching
            Assert.True(iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);

            // End right hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.HandHideTimeout + 0.1f);

            // Make sure right hand is not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Repeat with left hand
            // Begin left hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure left hand is tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(iss.HandDataLeft.IsTracked);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct hand is pinching
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);


            // End left hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.HandHideTimeout + 0.1f);

            // Make sure left hand is not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Lastly with 2 hands
            // Begin hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightHandManipulationKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure hands are tracked
            Assert.True(iss.HandDataRight.IsTracked);
            Assert.True(iss.HandDataLeft.IsTracked);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure hands are pinching
            Assert.True(iss.HandDataRight.IsPinching);
            Assert.True(iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);


            // End hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightHandManipulationKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftHandManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.HandHideTimeout + 0.1f);


            // Make sure hands are not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);
        }
    }
}
#endif