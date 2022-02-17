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