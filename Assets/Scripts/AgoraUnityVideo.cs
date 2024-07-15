using System;
using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class AgoraUnityVideo : MonoBehaviour
{
    public static AgoraUnityVideo Instance;
    private IRtcEngine mRtcEngine;
    [SerializeField] private string appId;
    [SerializeField] private string token;
    [SerializeField] private string channelName;
    private List<string> inCollision = new List<string>();
    private uint localUserID;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        LoadEngine(appId, token);
    }
    public void JoinAgora()
    {
        Join(channelName);
        Debug.Log(channelName);
    }

    public void LeaveAgora()
    {
        Leave();
    }

    private void OnApplicationQuit()
    {
        Leave();
        UnloadEngine();
    }

    public void LoadEngine(string appID, string token = null)
    {
        this.token = token;
        if (mRtcEngine != null)
            return;
        mRtcEngine = IRtcEngine.getEngine(appID);
    }

    public void Join(string channel)
    {
        if (mRtcEngine == null)
            return;
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.JoinChannelByKey(channelKey: token, channelName: channel);
    }

    public void Leave()
    {
        if (mRtcEngine == null)
            return;
        mRtcEngine.LeaveChannel();
        mRtcEngine.DisableVideoObserver();
        GameObject go = GameObject.Find($"{localUserID}");
        Debug.Log($"User id {localUserID} has left the channel (local)");
        if (go != null)
            Destroy(go);
        for(int i = 0; i < inCollision.Count; i++)
        {
            go = GameObject.Find(inCollision[i]);
            if(go != null)
                Destroy(go);
        }
    }

    public void UnloadEngine()
    {
        if (mRtcEngine != null)
        {
            Debug.Log("Engine Unloaded");
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    public void EnableVideo()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.EnableVideo();
        }
    }

    public void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        localUserID = uid;
        Debug.Log($"User id {localUserID} has joined the channel (local)");
        GameObject childVideo = GetChildVideoLocation(uid);
        MakeImageVideoSurface(childVideo, uid);
    }

    public void OnUserJoined(uint uid, int elapsed)
    {
        GameObject childVideo = GetChildVideoLocation(uid);
        VideoSurface videoSurface = MakeImageVideoSurface(childVideo, uid);
        Debug.Log($"User id {uid} has joined the channel (global)");
        inCollision.Add(uid.ToString());
        if (videoSurface != null)
        {
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }

    public void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        GameObject go = GameObject.Find(uid.ToString());
        Debug.Log($"User id {uid} has left the channel (global)");
        if (go != null)
            Destroy(go);
    }

    private GameObject GetChildVideoLocation(uint uid)
    {
        GameObject go = GameObject.Find("Videos");
        GameObject childVideo = go.transform.Find($"{uid}")?.gameObject;
        if (childVideo == null)
        {
            childVideo = new GameObject($"{uid}");
            childVideo.transform.parent = go.transform;
        }
        return childVideo;
    }

    private VideoSurface MakeImageVideoSurface(GameObject go, uint uid)
    {
        RawImage rawImage = go.AddComponent<RawImage>();
        rawImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200.0f, 700.0f);
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
        rectTransform.localRotation = new Quaternion(0, rectTransform.localRotation.y, -180.0f, rectTransform.localRotation.w);

        // Create and set up the Text component for labeling
        GameObject textGO = new GameObject("Label");
        textGO.transform.parent = go.transform;
        Text text = textGO.AddComponent<Text>();
        text.text = $"User ID: {uid}";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = rectTransform.sizeDelta;
        textRect.localPosition = Vector3.zero;

        return go.AddComponent<VideoSurface>();
    }
}
