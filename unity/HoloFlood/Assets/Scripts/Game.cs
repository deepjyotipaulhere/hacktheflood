using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using UnityEngine.Networking;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    private float floodSpeed = 0.05f;
    private float floodMax = 2.5f;
    private float floodCurrent = 0f;

    private float secondWave = 30f;
    public GameObject prefabHuman;
    public MeshRenderer canvas;

    public GameObject textScore;
    public GameObject textCollisions;
    private Transform flood;

    public int score = 0;

    public Image intro;
    private float introCountdown = 3f;

    public int numCollisions = 0;
    public string backendURL = "https://hacktheflood.azurewebsites.net/flood";
    private PhotoCapture photoCaptureObject = null;

    void Start(){
        Spawn();
        flood = GameObject.Find("Flood").GetComponent<Transform>();
    }

    void Spawn(int num = 8){    

        var humans = GameObject.Find("Humans").GetComponent<Transform>();

        // Create a human on each path
        var pths = GameObject.Find("Paths");
        int i = 0;
        foreach (Transform path in pths.GetComponentInChildren<Transform>()) {
            if (i > num-1){
                break;
            }
            // Vector3 pos = path.GetChild(0).transform.position;
            var human = Instantiate(prefabHuman, humans);
            human.GetComponent<Human>().SetPath(path);
        }
    }

    public void AddScore(){
        score += 1;
        if (textScore.GetComponent<TextMeshPro>() != null){
            textScore.GetComponent<TextMeshPro>().text = score.ToString();
        }
    }

    public void AddCollision(){
        numCollisions += 1;
        if (textCollisions.GetComponent<TextMeshPro>() != null){
            textCollisions.GetComponent<TextMeshPro>().text = numCollisions.ToString();
        }
    }

    // Taking a photo and diffusing an alternate flooded reality
    public void TakePhoto(){
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject){
        photoCaptureObject = captureObject;
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result){
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            Texture2D croppedTexture = CropTexture(targetTexture, new Vector2(786, 786));

            canvas.material.mainTexture = croppedTexture;

            byte[] imgBytes;
            imgBytes = croppedTexture.EncodeToPNG();
            // string enc = Convert.ToBase64String(imgBytes);
            StartCoroutine(FloodedAlternateReality(imgBytes));
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    IEnumerator FloodedAlternateReality(byte[] imgRaw){
        WWWForm formrequest = new WWWForm();
        formrequest.AddBinaryData("file", imgRaw, "input.png", "image/png");
        UnityWebRequest www = UnityWebRequest.Post(backendURL, formrequest);

        Debug.Log("Sending image to Stable Diffusion backend");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log(www.error);
        }
        else {
            // Load resulting b64 into texture2d
            string resp = www.downloadHandler.text;
            resp = resp.Substring(11, resp.Length - 13);
            Debug.Log(resp);
            var b64_bytes = System.Convert.FromBase64String(resp);
            var tex = new Texture2D(1,1);
            tex.LoadImage(b64_bytes);
            canvas.material.mainTexture = tex;
        }
    }

    public Texture2D CropTexture(Texture2D texture, Vector2 crop, int x = 0, int y = 0) {
        if (crop.x < 0f || crop.y < 0f) {
            return texture;
        }
        Rect sizes = new Rect ();
        Texture2D result = new Texture2D ((int) crop.x, (int) crop.y);
        if (crop.x != 0f && crop.y != 0f) {
            sizes.x = 0;
            sizes.y = 0;
            sizes.width = crop.x;
            sizes.height = crop.y;
            
            sizes.x = (texture.width - crop.x) / 2f;
            sizes.y = (texture.height - crop.y) / 2f;

            if ((texture.width < sizes.x + crop.x) || (texture.height < sizes.y + crop.y) || (sizes.x > texture.width) || (sizes.y > texture.height) || (sizes.x < 0) || (sizes.y < 0) || (crop.x < 0) || (crop.y < 0)) {
                return texture;
            }
            result.SetPixels (texture.GetPixels (Mathf.FloorToInt (sizes.x), Mathf.FloorToInt (sizes.y), Mathf.FloorToInt (sizes.width), Mathf.FloorToInt (sizes.height)));
            result.Apply();
        }
        return result;
    }

    void Update()
    {
        float after = introCountdown - Time.deltaTime;
        if (introCountdown > 0 && after < 0){
            intro.color = new Color(0, 0, 0, 0);
        } else if (after > 0){
            intro.color = new Color(1, 1, 1, 1f - (after / 6f));
        }
        introCountdown = after;

        secondWave -= Time.deltaTime;
        if (secondWave < 0){
            Spawn(4);
            secondWave = 60f;
        }
        floodCurrent += Time.deltaTime * floodSpeed;
        flood.position = new Vector3(0, floodCurrent, 0);
        if (floodCurrent > floodMax){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}