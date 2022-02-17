# About this
This is the program set and text instruction for [my tutorial video](https://youtu.be/3PRifmlwi0I)

## Step1 Handling Midi Event
- Create a new 3D Unity project. I use Unity 2019.4.16f1 on Ubuntu 20.04
- After Unity launched, Open "Packages/manifest.json" by any text editor.
- Add "scopedRegistries" section(you can copy from  the description) below "dependancies" section.

```json
"scopedRegistries": [
  {
    "name": "Keijiro",
    "url": "https://registry.npmjs.com",
    "scopes": [
      "jp.keijiro"
    ]
  }
]
```
- Save "manifest.json" and back to Unity. Package Manager will automatically launch, then, Project Setting will appear.
- Open "Package Manager" from "Window".
- Switch to "My Registry" and search "Minis"
- Install Minis (NOT BoltMinis!!)
- After installation finished, restart Unity(maybe automatically restart)
- After Unity has restarted...
- Create "Scripts" Folder in "Assets".
- Create "MidiScript.cs" in "Assets/Scripts" and open it.
- Edit it.

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MidiScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;
            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) => {
                Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.noteNumber.GetType(),
                    note.shortDisplayName,
                    velocity,
                    velocity.GetType(),
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));
            };
            
            midiDevice.onWillNoteOff += (note) => {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));
            };
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
```

- This program sets consol outputs as Midi event handler.
- Create Empty Object "MidiManager" and attach MidiScript.cs to this.
- Connect Midi device, press "Play " and show the console.
- Midi event information will be displayed when you hit or release Midi keyboard.


## Step2 Visualize notes

- Create "BarScript.cs" in "Assets/Scripts" and oepn it.
- Edit it.

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    const int keysCount = 88;
    [SerializeField] GameObject barManager;
    GameObject[] barsPressed = new GameObject[keysCount]; // bars linked to the pressed key
    [SerializeField] List<GameObject> barsReleased = new List<GameObject>(); // bars linked to the released key

    bool[] isKeyPressed = new bool[keysCount];

    float barSpeed = (float)0.05;
    float upperPositionLimit = (float)100;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 88; i++){
            // initialize: keys are not pressed
            isKeyPressed[i] = false;
            barsPressed[i] = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // currently pressed keys
        for (int i = 0; i < 88; i++){
            if (isKeyPressed[i] && barsPressed[i] != null) {
                Vector3 scale = barsPressed[i].transform.localScale;
                scale.y += barSpeed * 2;
                barsPressed[i].transform.localScale = scale;
                Vector3 pos = barsPressed[i].transform.position;
                pos.y += barSpeed;
                barsPressed[i].transform.position = pos;
            }
        }

        // released keys
        for(int i = barsReleased.Count - 1; i >= 0; i--){
            Vector3 pos = barsReleased[i].transform.position;

            // destroy bars when it reached upperPositionLimit
            if (pos.y > upperPositionLimit){
                Destroy(barsReleased[i]);
                barsReleased.RemoveAt(i);
            } else {
                pos.y += barSpeed * 2;
                barsReleased[i].transform.position = pos;
            }
        }
    }

    public void onNoteOn(int noteNumber, float velocity)
    {
        // clearfy that the key is pressed
        isKeyPressed[noteNumber] = true;

        // craete bar object
        GameObject barPrefab;
        barPrefab = (GameObject)Resources.Load("Prefab/Bar");
        barsPressed[noteNumber] = Instantiate(barPrefab);
        barsPressed[noteNumber].transform.position = new Vector3(noteNumber, 0, 0);
        barsPressed[noteNumber].transform.SetParent(barManager.transform, true);
    }

    public void onNoteOff(int noteNumber)
    {
        barsReleased.Add(Clone(barsPressed[noteNumber]));
        DestroyImmediate(barsPressed[noteNumber]);

        isKeyPressed[noteNumber] = false;
    }

    GameObject Clone( GameObject obj )
    // reference: https://develop.hateblo.jp/entry/2018/06/30/142319
    {
        var clone = GameObject.Instantiate( obj ) as GameObject;
        clone.transform.parent = obj.transform.parent;
        clone.transform.localPosition = obj.transform.localPosition;
        clone.transform.localScale = obj.transform.localScale;
        return clone;
    }
}
```

- Change "keysCount" value according to your Midi keyboard.
- Create Empty Object "BarManager" and attach BarScript.cs to this.
- In "BarManager" inspector, attach it to "Bar Manager" in "Bar Script(Script)"
- Create Cube Object "Bar" and change its scale Y to 0.1.
- Create "Resources" Folder in "Assets" and create "Prefab" Folder in "Assets/Resources"
- Drag & Drop "Bar" into "Prefab" and delete "Bar" from Scene.
- Open "MidiScript.cs" and edit it.

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MidiScript : MonoBehaviour
{
    // midi note number of lowest key in your midi device
    // 21: A0
    int keyOffset = 21;

    [SerializeField] GameObject barManager;

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;
            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) => {
                Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.noteNumber.GetType(),
                    note.shortDisplayName,
                    velocity,
                    velocity.GetType(),
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                barManager.GetComponent<BarScript>().onNoteOn(note.noteNumber - keyOffset, velocity);
            };
            
            midiDevice.onWillNoteOff += (note) => {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                barManager.GetComponent<BarScript>().onNoteOff(note.noteNumber - keyOffset);
            };
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
```

- Change "keyOffset" value according to your Midi keyboard. This value is Midi note number of the lowest key of your device. (e.g. A0 is 21)
- Open MidiManager's inspector and attach "BarManager" object
to "Bar Manager" in "Midi Script(Script)"
- Now, it's complete. Play Unity and hit Midi keyboard.
