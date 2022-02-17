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