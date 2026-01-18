# Unity Magic Leap 2 Eye-Gaze Interaction Demo Project

This project demonstrates **eye-tracking (gaze) interaction on Magic Leap 2 using Unity OpenXR**.  
It includes full setup of OpenXR eye-tracking, runtime permission handling, gaze raycasting, and a **gaze-based passcode input system**.

Based on the Magic Leap + Unity OpenXR gaze tutorial series:
https://www.youtube.com/watch?v=IqZtadtnSeI

---

## ✨ Features

- Magic Leap 2 **OpenXR** integration
- **Eye-tracking permission** request at runtime
- Real-time gaze position & rotation tracking
- **Gaze-ray raycasting** for world interaction
- **Gaze-based passcode keypad UI**

---

## 🛠 Requirements

- **Unity 2022.x or newer**
- **Magic Leap 2**
- Magic Leap OpenXR Plugin
- Android Build Support

---

## Android Manifest Permission

Ensure your manifest contains:

```xml
<uses-permission android:name="com.magicleap.permission.EYE_TRACKING" />

---

## Project Details and Modifications

- Imported prefabs from tutorial, however I had to remake and reattach materials to the keypad and cube.
- Logger would cuase crashes in the code so I instead used Debug.Log in the code to display necessary info in the Magic Leap Hub Logs

---



