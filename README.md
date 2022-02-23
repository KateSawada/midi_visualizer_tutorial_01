# About this
This is the program set and text instruction for [my tutorial video](https://youtu.be/jE6XiUI-a5Q)

## Step3 Handling Camera Input
- Change Edit/Project Settings/Player/Active Input Handling to "Both".
- Unity may restart.
- After Unity has restarted, create "CameraControl.cs" in "Assets/Scripts" and oepn it.
- Edit it.
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Dropdown dropDown;

    WebCamTexture webCamTexture;
    WebCamDevice[] webCamDevices;

    void Start()
    {
        string name;
        this.webCamDevices = WebCamTexture.devices;
        for (int i = 0; i < this.webCamDevices.Length; i++){
            name = this.webCamDevices[i].name.ToString();
            dropDown.options.Add(new Dropdown.OptionData { text = name });
        }
    }

    public void onDeviceChanged () {
        if (webCamTexture != null){
            webCamTexture.Stop();
        }
        webCamTexture = new WebCamTexture(webCamDevices[dropDown.value].name, 1280, 720, 30);
        rawImage.texture = webCamTexture;

        // apply video aspect to rawImage
        float rate = (float)rawImage.texture.width / rawImage.texture.height;
        float imageHeight = rawImage.rectTransform.sizeDelta.y;
        rawImage.rectTransform.sizeDelta = new Vector2(imageHeight * rate, imageHeight);

        webCamTexture.Play();
    }
}
```
- Set the aspect setting in Game tab to 16:9.
- Create Canvas.
- Change canvas settings.
    - Rander Mode: Screen Space - Camera
    - Rander Camera: Main Camera (setting by attaching)
    - Layer: Default
- Create RawImage "CameraImage" as a Canvas' child.
- Change Anchors in Rect Transform to 
    - Min: 0, 0
    - Max: 1, 1
- Create Dropdown "CameraSelector" as a Canvas' child and delete its all options.
- Create EmptyObject "Managers" and move "MidiManager" and "BarManager" to its children.
- Create EmptyObject "CameraManager" as a Managers's child.
- Attach "CameraControl.cs" to "CameraManager".
- Attach "CameraImage" and "CameraSelector" to "CameraControl.cs" in "CameraManager".
- Resister CameraManager's function "CameraControl.onDeviceChanged" to "On Value Changed" in "CameraSelector" 

# Step4 Termination Processing
- Open "MidiScript.cs" and edit it.
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using Minis;

public class MidiScript : MonoBehaviour
{
    // midi note number of lowest key in your midi device
    // 21: A0
    int keyOffset = 21;

    [SerializeField] GameObject barManager;
    MidiDevice midiDevice;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
    }

    void MidiDeviceSettingUp (InputDevice device, InputDeviceChange change) 
    {
        if (change != InputDeviceChange.Added) return;
        midiDevice = device as MidiDevice;
        if (midiDevice == null) return;

        midiDevice.onWillNoteOn += onNoteOn;
        midiDevice.onWillNoteOff += onNoteOff;
    }

    void onNoteOn (MidiNoteControl note, float velocity) {
        Debug.Log(string.Format(
            "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
            note.noteNumber,
            note.noteNumber.GetType(),
            note.shortDisplayName,
            velocity,
            velocity.GetType(),
            (note.device as MidiDevice)?.channel,
            note.device.description.product
        ));

        barManager.GetComponent<BarScript>().onNoteOn(note.noteNumber - keyOffset, velocity);
    }

    void onNoteOff (MidiNoteControl note) {
        Debug.Log(string.Format(
            "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
            note.noteNumber,
            note.shortDisplayName,
            (note.device as MidiDevice)?.channel,
            note.device.description.product
        ));

        barManager.GetComponent<BarScript>().onNoteOff(note.noteNumber - keyOffset);
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
```
- If you changed "keyOffset" value in #1, reflect it again.

# In Play mode
- In Play mdoe, select your video device and adjust main Camera's Position and Rotation.