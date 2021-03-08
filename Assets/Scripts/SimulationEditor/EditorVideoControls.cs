﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EditorVideoControls : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Camera videoCamera;
    [SerializeField] LayerMask videoLayer;
    [SerializeField] EditorVideoPlayer editorVideoPlayer;
    [SerializeField] GameObject nodeCanvas;

    [Header("Play/Pause")]
    [SerializeField] Image playPauseImage;
    [SerializeField] Sprite playSprite;
    [SerializeField] Sprite pauseSprite;

    [Header("Time Line Rects")]
    [SerializeField] RectTransform loopPointImageRect;
    [SerializeField] RectTransform actionStartPointImageRect;
    [SerializeField] RectTransform actionEndPointImageRect;
    [SerializeField] RectTransform sliderHandleRect; //S TODO replace with timeline's position

    [Header("Buttons")]
    [SerializeField] GameObject loopButton;
    [SerializeField] GameObject videoStartButton;
    [SerializeField] GameObject videoEndButton;
    [SerializeField] GameObject actionStartButton;
    [SerializeField] GameObject actionEndButton;

    bool placingWorldSpaceMarker = false;

    public bool PlacingWorldSpaceMarker
    {
        get { return placingWorldSpaceMarker; }
        set
        {
            nodeCanvas.SetActive(!value);
            placingWorldSpaceMarker = value;
        }
    }

    public GameObject NodeCanvas { get => nodeCanvas; set => nodeCanvas = value; }

    private void Update()
    {
        if (placingWorldSpaceMarker)
            SetWorldSpaceActionPosition();

        if (Input.GetKeyDown(KeyCode.Escape))
            PlacingWorldSpaceMarker = false;

        //S TODO set a real input for this
        ToggleNodeCanvas();
        //S LATER - set this to happen with events or smth
        TogglePlayPauseSprite();
    }

    private void SetWorldSpaceActionPosition()
    {
        if (Input.GetMouseButtonDown(0) == false) return;
        var ray = videoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, videoLayer, QueryTriggerInteraction.Collide))
        {
            NodeInspector.instance.CurrentActionNode.setWorldPosition(hit.point);
            NodeInspector.instance.CreateWorldMarkers();

            PlacingWorldSpaceMarker = false;

            if (NodeInspector.instance.CurrentActionNode.getActionType() == ActionType.AreaButton)
                NodeInspector.instance.EditAreaMarkerPositioning();
        }
    }

    private void TogglePlayPauseSprite()
    {
        if (videoPlayer.isPlaying)
            playPauseImage.sprite = pauseSprite;
        else playPauseImage.sprite = playSprite;
    }

    private void ToggleNodeCanvas()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            nodeCanvas.SetActive(!nodeCanvas.activeInHierarchy);
    }

    public void TogglePause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else videoPlayer.Play();
    }

    #region Marker points
    public void SetLoopPoint(bool setToVideo = false)
    {
        loopPointImageRect.anchoredPosition = sliderHandleRect.anchoredPosition;
        if (setToVideo)
            editorVideoPlayer.SetLoopTimeToVideo();
    }
    public void SetVideoStartPoint(float value)
    {
        editorVideoPlayer.SetStartTimeToVideo(value);
    }
    public void SetVideoEndPoint(float value)
    {
        editorVideoPlayer.SetEndTimeToVideo(value);
    }

    public void SetActionStartPoint(bool setToVideo = false)
    {

        actionStartPointImageRect.anchoredPosition = sliderHandleRect.anchoredPosition;
        if (setToVideo)
            editorVideoPlayer.SetStartTimeToAction();
    }
    public void SetActionEndPoint(bool setToVideo = false)
    {
        actionEndPointImageRect.anchoredPosition = sliderHandleRect.anchoredPosition;
        if (setToVideo)
            editorVideoPlayer.SetEndTimeToAction();
    }
    #endregion
    public void SetCurrentControlsToVideo(bool value)
    {
        var loop = NodeInspector.instance.CurrentVideoNode.getLoop();
        loopButton.SetActive(loop);
        loopPointImageRect.gameObject.SetActive(loop);

        if (NodeInspector.instance.CurrentActionNode != null)
        {
            if (NodeInspector.instance.CurrentActionNode.getAutoEnd())
                value = !value;

            if (!NodeInspector.instance.CurrentActionNode.getIsTimed())
                return;
        }

        actionStartButton.SetActive(!value);
        actionEndButton.SetActive(!value);
        actionEndPointImageRect.gameObject.SetActive(!value);
        actionStartPointImageRect.gameObject.SetActive(!value);
    }
}
