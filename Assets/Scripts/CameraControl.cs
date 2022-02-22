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