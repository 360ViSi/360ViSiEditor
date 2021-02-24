using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

public class VideoTextureChanger : MonoBehaviour
{
    //variables used in editor
    [SerializeField]
    private VideoPathPointer_SO videoPointer;
    [SerializeField] private SimulationManager simulationManager;

    //private variables
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] TMPro.TMP_Text videoTimeText;
    float currentVideoEndTime = 1;
    float currentVideoStartTime = 0;
    float currentVideoLoopTime = 0;
    bool currentVideoLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        //initialize Video player
        videoPlayer.prepareCompleted += PrepareCompleted;
    }

    private void Update()
    {
        videoTimeText.text = Utilities.FloatToTime((float)(videoPlayer.time / videoPlayer.length), (float)videoPlayer.length, true);

        if (videoPlayer.length == 0 || !videoPlayer.isPlaying)
            return;


        // S TODO - has to be a better way to solve this, maybe with just checking for a stop?
        var endThresholdFrames = 1;
        if (currentVideoEndTime == 1)
            endThresholdFrames++;
        if (videoPlayer.frame < (videoPlayer.frameCount * currentVideoEndTime - endThresholdFrames))
            return;

        if (currentVideoLoop == false)
        {
            videoPlayer.Stop();
            simulationManager.AutoEnd();
            return;
        }
        StartCoroutine(LoopRoutine((float)videoPlayer.length * currentVideoLoopTime));
    }
    
    IEnumerator LoopRoutine(float time)
    {
        videoPlayer.time = time;
        videoPlayer.Pause();
        yield return new WaitForSeconds(.25f); // it takes some time for the VideoPlayer to seek
        videoPlayer.Play();
    }

    void PrepareCompleted(VideoPlayer videoPlayer)
    {
        Debug.Log("video is ready");
        videoPlayer.time = videoPlayer.length * currentVideoStartTime;
        videoPlayer.Play();

    }

    public void changeVideo(string videoFileName)
    {
        videoPlayer.url = getVideoURL(videoPointer, videoFileName);

        //get currents
        var videoPart = simulationManager.getCurrentVideoPart();
        currentVideoStartTime = videoPart.getStartTime();
        currentVideoEndTime = videoPart.getEndTime();
        currentVideoLoop = videoPart.getLoop();
        currentVideoLoopTime = videoPart.getLoopTime();
        Debug.Log($"start: {currentVideoStartTime}, end: {currentVideoEndTime}, Loop: {currentVideoLoop}, Looptime: {currentVideoLoopTime}");

        videoPlayer.Prepare();
        //videoPlayer.time = videoPlayer.length * currentVideoStartTime;
    }

    private string getVideoURL(VideoPathPointer_SO pointer, string videoFileName)
    {
        //S TODO: placeholder folder for easier dev
        string directory = @"C:\Unity\"; //getVideoFolder(pointer);
        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        string videoURL = "";

        //read every file name in folder
        foreach (var file in dirInfo.GetFiles())
        {
            string fileName = file.Name;
            if (videoFileName != fileName)
            {
                continue;
            }
            //compare to VideoPathPointer_OS extensions
            foreach (var extension in pointer.fileExtensions)
            {
                string nameTail = fileName.Substring(fileName.Length - extension.Length, extension.Length);

                //if extension matches add URL to list
                if (nameTail == extension)
                {
                    videoURL = directory + fileName;
                    break;
                }
            }
        }
        if (videoURL == "")
        {
            Debug.Log("Video " + videoFileName + " is not in " + directory + " folder");
        }
        return videoURL;
    }

    private string getVideoFolder(VideoPathPointer_SO pointer)
    {
        return null; //CANNOT USE ASSETDATABASE IN A BUILD
        
        // Get absolute path to Application
        // and relative path to videoPointer.
        // Both will have same "Assets" folder which need to be removed.
        // Also videoPointer filename need to be removed to get pure folder path.
        // Path separator is used to make the code operationsystem independent
        // string pathToVideo = Application.dataPath;
        // char pathSeparator = Path.DirectorySeparatorChar;
        // string[] relativePathFolders = AssetDatabase.GetAssetPath(videoPointer).Split(pathSeparator);
        // //drop first and last part (i=1 and Length-1)
        // for (int i = 1; i < relativePathFolders.Length - 1; i++)
        // {
        //     pathToVideo += pathSeparator + relativePathFolders[i];
        // }
        // pathToVideo += pathSeparator;
        // return pathToVideo;
    }
}
