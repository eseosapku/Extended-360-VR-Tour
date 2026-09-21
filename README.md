Extended 360 VR Tour

A VR tour application built for Meta Quest 2 using Unity's XR Interaction Toolkit. The project presents two separate 360° tours — a video-based tour of Holberton's campus and a photo-based tour of ALU's campus — navigated through room-to-room hotspots and a shared main menu.

Overview

The app is split across three Unity scenes:

Scene	Description
MainMenuScene	Entry point. Lets the user choose which tour to enter.
IntranetTourScene	360° video tour of Holberton's campus. Each room is a sphere with a video wrapped around its interior.
CustomCampusTourScene	360° photo tour of ALU's campus, using static equirectangular photos instead of video.

Every scene change and every room-to-room jump fades to black before switching, rather than cutting instantly — a deliberate VR comfort choice to avoid disorienting the user with sudden visual changes.

Requirements
Unity 6000.0.75f1 (Unity 6) or later
XR Interaction Toolkit 3.4.1
XR Plugin Management, with the OpenXR provider enabled for the Android build target and Meta Quest Support feature checked
Android build support module installed in Unity Hub
Player Settings → Android → Minimum API Level: Android 10.0 (API 29) or higher
A Meta Quest 2 with Developer Mode and USB Debugging enabled, for building directly to device
How each room works

Video rooms (IntranetTourScene): each room is a sphere with a Video Player component set to render into a Render Texture, which is applied to the sphere's material. The material's shader is URP Unlit with Render Face set to Back, so the video is visible from the inside of the sphere rather than the (normally hidden) outside.

Photo rooms (CustomCampusTourScene): same idea, but the material's Base Map is a static photo directly — no Video Player or Render Texture needed.

UI in VR: every Canvas in the project is World Space, using a Tracked Device Graphic Raycaster instead of the default Graphic Raycaster, since the default one only understands mouse/touch input and can't see an XR controller ray or gaze at all. Each scene also requires its own XR Interaction Manager object — without one, no interactor in that scene can register input.

Scripts
TourManager.cs

Lives once per scene. Tracks every room (a name, its sphere GameObject, and its Video Player if it has one) and switches between them via SwitchToSphere(string). Fades to black, swaps which room is active, then fades back in — the swap itself happens invisibly behind the black screen.

SceneTransitionManager.cs

Persists across scene loads (DontDestroyOnLoad) and handles jumping between entire scenes. Fades to black, asynchronously loads the target scene, then fades back in.

The fade itself is a small black sphere that follows whichever camera is currently active, rendered from the inside — the same inside-out technique used for the video/photo rooms. A standard Screen Space - Overlay UI fade was tried first but doesn't reliably render inside a real headset (it only previewed correctly in the flat Editor view), so this world-space approach is used instead.

SceneNavButton.cs

A small helper used on scene-change buttons. Since SceneTransitionManager only exists at runtime (carried in from MainMenuScene), a button can't hold a permanent Inspector reference to it while editing a different scene. This script looks up SceneTransitionManager.Instance dynamically at the moment the button is clicked instead, so the reference can never go stale.

InfoBoxToggle.cs

Attached to each room's info button. Toggles an info panel's visibility on and off, showing a short description of that location.

Building and running
Open the project in Unity 6000.0.75f1+.
Confirm Project Settings → XR Plug-in Management → Android tab has OpenXR checked, with Meta Quest Support enabled.
Confirm Player Settings → Android → Minimum API Level is set to 29 or higher.
Add all three scenes to File → Build Settings → Scenes In Build.
To test on-device: enable Developer Mode and USB Debugging on the headset, connect via USB, and use Build and Run.
To test without a headset: enable "Use XR Device Simulator in Scenes" under Project Settings → XR Interaction Toolkit, then enter Play Mode and use the simulator's on-screen controls to simulate controller/gaze input.
Notes / lessons learned
A sphere's default material renders faces outward — remember to flip Render Face to Back (or Both) on any sphere the camera will be standing inside of.
World Space UI needs a Tracked Device Graphic Raycaster, not the default Graphic Raycaster, to respond to XR input.
Every scene needs its own XR Interaction Manager object.
Screen Space - Overlay UI is unreliable for full-screen VR effects like fades — a world-space object that moves with the camera is more consistent across devices.
A Button's On Click() reference breaks if the object it points to gets destroyed (e.g. by a singleton's duplicate-cleanup logic on scene reload) — prefer looking up a runtime singleton dynamically (see SceneNavButton.cs) over a fixed Inspector reference when the target object's lifetime spans multiple scenes.
