using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using JMRSDK;
using agora_gaming_rtc;

namespace ScreenCasting
{
    public class ScreenCastHandler : MonoBehaviour
    {
        [Tooltip("Only set True when need to check logs")]
        private const bool isDebug = false;

        private readonly float BackgroundFOV = 60f;
        private readonly float CameraFOV = 25f;

        private Texture defaultEnvironmentModel = null;

        public Camera screenCastCameraPreview;

        public Camera castCamera;
        public Image cameraFeed = null;

        //For testing in editor mode, to removed
        public bool TestInEditor = false;
        public bool ConnectWithToken = false;

        public Texture2D texture = null;
        Texture2D mTexture;

        public IRtcEngine mRtcEngine;

        [SerializeField]
        private string appId = "59185037cebd4acdb839e6d2d60060cb";
        [SerializeField]
        private string channelName = "temp";
        [SerializeField]
        private string Token = "temp";
        [SerializeField]
        private uint UUID = 999;
        [SerializeField]
        private Camera povCam;


        int fileCounter = 10;
        int skipframes = 0;

        float time = 0;

        ExternalVideoFrame externalVideoFrame;

        JMRScreenCastBackground currentScreenBackgroundType;

        JMRScreenCastBackground Background;

        byte[] bytes;
        bool isScreenSharing = false;

        RoomCreds roomCreds = new RoomCreds();
        

        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        Skybox skyboxBg;


        void Start()
        {
            Background = JMRScreenCastManager.Instance.GetCurrentBackground();

            screenCastCameraPreview.enabled = false;
            povCam.enabled = false;

            if (screenCastCameraPreview == null)
            {
                if(isDebug)
                    Debug.LogError("POV CAMERA IS NULL");
                if (screenCastCameraPreview == null)
                {
                    if (isDebug)
                        Debug.LogError("POV CAMERA IS STILL NULL");
                    return;
                }
            }

            cancellationTokenSource = new CancellationTokenSource();

            InvokeRepeating("HandleInternetConnection", 3f, 3f);

            GetSkyboxComponent();
        }

        void Update()
        {
            if (JMRCameraManager.Instance != null)
            {
                try
                {
                    if (isScreenSharing)
                    {
                        //Setting up cancellation token for async task
                        cancellationToken = cancellationTokenSource.Token;
                        ShareScreen(cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Screen Share Exception : " + e.Message);

                }

                //Check if Internet is working
                //If yes screen share else
                //retry screen share
                if(isDebug)
                    HandleInternetConnection();
            }

        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Capture();
            }
        }

        /// <summary>
        /// Subrscribe to events
        /// </summary>
        void OnEnable()
        {
            JMRScreenCastManager.OnStartCastingRequested += OnScreenCastingRequested;
            JMRScreenCastManager.OnStopCastingRequested += LeaveChanel;
            JMRScreenCastManager.OnStopCastingRequested += SendScreenCastOff;
            JMRScreenCastManager.OnStopCastingRequested += StopCameraPreview;
        }

        /// <summary>
        /// Leave channel whenever script is disabled
        /// </summary>
        void OnDisable()
        {
            JMRScreenCastManager.OnStartCastingRequested -= OnScreenCastingRequested;
            JMRScreenCastManager.OnStopCastingRequested -= LeaveChanel;
            JMRScreenCastManager.OnStopCastingRequested -= SendScreenCastOff;
            JMRScreenCastManager.OnStopCastingRequested -= StopCameraPreview;

            LeaveChanel(Background);
            CancelAsyncTask();
        }


        /// <summary>
        /// Leave chanel when application pauses
        /// Join Chanel when application resumes
        /// </summary>
        void OnApplicationPause(bool isPaused)
        {
            if (isDebug)
                Debug.Log("On Application Pause");
            if (isPaused)
            {
                time = 0f;
                CancelAsyncTask();
                LeaveChanel(Background);

                if (JMRScreenCastManager.Instance.GetCastingState() == JMRScreenCastManager.Instance.STATE_ON)
                {
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_PAUSED);
                }
            }
            else
            {
                //check if internet is available
                if (TestInEditor)
                {
                    Setup();
                }

                if (JMRScreenCastManager.Instance.GetCastingState() == JMRScreenCastManager.Instance.STATE_OFF)
                {
                    isScreenSharing = false;
                }

                cancellationTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Send pause state to the service
        /// </summary>
        void SendScreenCastOff(JMRScreenCastBackground backgroundID)

        {
            NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_OFF);
            CancelInvoke();
            //Camera Preview to only start for JioGlass Ent
            if(JMRRigManager.Instance.getDeviceID()==(int)JMRRigManager.DeviceType.JioGlass)
            {
                JMRCameraManager.OnCameraPreviewStart -= StartScreenShare;
                JMRCameraManager.OnCameraError -= CameraError;
            }
        }

        /// <summary>
        /// Unload the agora engine when application stops
        /// </summary>
        void OnApplicationQuit()
        {
            CancelAsyncTask();
            UnloadEngine();
        }

        /// <summary>
        /// Notify JMRCamera Errors to service
        /// </summary>
        /// <param name="error"></param>
        void CameraError(string error)
        {
            Debug.LogError($"Screen Cast Camera Error : {error}");

            if (error.Equals("ERROR_CAMERA_PERMISSION") || error.Equals("ERROR_CAMERA_RECORD_AUDIO_PERMISSION"))
            {
                currentScreenBackgroundType.type = JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND;
                Setup();

            }

            if (error.Equals("ERROR_CAMERA_PREVIEW") || error.Equals("ERROR_CAMERA_NOT_SUPPORTED"))
            {
                if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
                {
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR, JMRScreenCastManager.Instance.CASTING_ERROR_CAMERA_ERROR);
                }
                else
                {
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR);

                }
            }

            if (error.Equals("ERROR_CAMERA_OPEN"))
            {
                currentScreenBackgroundType.type = JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND;
                Setup();
            }

            if (error.Equals("ERROR_CAMERA_BUSY"))
            {
                Debug.LogError("Screen Cast Camera Error : ERROR_CAMERA_BUSY");
            }
        }


        /// <summary>
        /// Logs agora sdk error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        void OnError(int errorCode, string message)
        {
            Debug.LogError($"Screen Cast Error : {errorCode} : Message : {message}");

            NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR, JMRScreenCastManager.Instance.CASTING_ERROR_CONNECTION_FAILED);
        }


        /// <summary>
        /// Starts the screen sharing process when the app launches for the first time
        /// </summary>
        void Setup()
        {
            StartCoroutine(CheckInternetConnection(isConnected =>
            {
                if (isConnected)
                {
                    if (isDebug)
                    {
                        Debug.Log("Current Background Type :" + currentScreenBackgroundType);
                        Debug.Log("Current Background :" + Background.ToString());
                    }
                    if (castCamera == null)
                    {
                        var camera = GameObject.Find("POV Camera");
                        castCamera = camera.GetComponent<Camera>();
                        if (castCamera == null)
                        {
                            Debug.LogError("Couldnt find Cast camera for FOV changes");
                        }
                    }

                    GetSkyboxComponent();

                    if (currentScreenBackgroundType.type == JMRScreenCastManager.Instance.BG_TYPE_CAMERA )
                    {

                        if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
                        {
                            if (castCamera != null)
                            {
                                castCamera.fieldOfView = CameraFOV;
                            }
                            if (isDebug)
                                Debug.Log("ScreenCastV2: StartCameraPreview");
                            StartCameraPreview();
                        }
                        else
                        {
                            Debug.LogError("JMRSDK->ScreenCasting>: BG_TYPE_CAMERA is not supported for the selected device type");

                        }
                    }
                    else
                    {
                        if (isScreenSharing)
                        {
                            StopCameraPreview(Background);
                        }
                        if (castCamera != null)
                        {
                            castCamera.fieldOfView = BackgroundFOV;
                        }

                        if (currentScreenBackgroundType.type == JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND)
                        {
                        // dont need to show physical camera feed on this state
                        // disabling image component since last frame gets stuck on the raw image 
                            cameraFeed.enabled = false;
                        }

                        StartScreenShare();
                    }
                }
                else
                {
                    if (isDebug)
                        Debug.LogError("No Internet Connection available");

                //Notify services about the error, send error code and error state.
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR, JMRScreenCastManager.Instance.CASTING_ERROR_NO_INTERNET);
                }
            }));
        }

        /// <summary>
        /// Notify about the state change and error if any occurs
        /// </summary>
        /// <param name="state"></param>
        /// <param name="errorCode"></param>
        void NotifyStateAndErrorCode(int state = 0, int errorCode = 0)
        {
            if (state != 0)
            {
                JMRScreenCastManager.Instance.NotifyCastingState(state);
            }
            if (errorCode != 0)
            {
                JMRScreenCastManager.Instance.NotifyCastingError(errorCode);
            }
        }

        /// <summary>
        /// Leave Agora channel
        /// Disable the video observer
        /// </summary>
        void LeaveChanel(JMRScreenCastBackground backgroundID)
        {
            screenCastCameraPreview.enabled = false;
            povCam.enabled = false;

            if (isDebug)
                Debug.Log("On Disable leave the channel");

            if (mRtcEngine != null && isScreenSharing)
            {
                // leave channel
                mRtcEngine.LeaveChannel();
                // deregister video frame observers in native-c code
                mRtcEngine.DisableVideoObserver();
                if (isDebug)
                    Debug.Log("Stopped Screen sharing");
            }

            isScreenSharing = false;
        }

        /// <summary>
        /// Cancel async screen sharing task
        /// </summary>
        void CancelAsyncTask()
        {
            try
            {
                cancellationTokenSource.Cancel();
            }
            catch
            {
                if (isDebug)
                    Debug.Log("Async Tasks cancelled");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Destroys the Agora RTC engine instance
        /// </summary>
        public void UnloadEngine()
        {
            if (isDebug)
                Debug.Log("calling unloadEngine");

            // delete
            if (mRtcEngine != null)
            {
                IRtcEngine.Destroy();  // Place this call in ApplicationQuit
                mRtcEngine = null;
            }
        }

        /// <summary>
        /// Get a screenshot of the current view
        /// </summary>
        public void Capture()
        {
            RenderTexture activeRenderTexture = RenderTexture.active;
            RenderTexture.active = screenCastCameraPreview.targetTexture;

            screenCastCameraPreview.Render();

            Texture2D image = new Texture2D(screenCastCameraPreview.targetTexture.width, screenCastCameraPreview.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, screenCastCameraPreview.targetTexture.width, screenCastCameraPreview.targetTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = activeRenderTexture;

            byte[] bytes = image.EncodeToPNG();
            Destroy(image);
            Debug.Log(Application.dataPath);
            File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + fileCounter + ".png", bytes);
            fileCounter++;
        }

        /// <summary>
        /// Start Camera preview
        /// </summary>
        public void StartCameraPreview()
        {
            if (cameraFeed != null)
            {
                //Setting maximum resolution of camera
                //List<FrameSize> previewResolutionsList = JMRCameraManager.Instance.GetPreviewResolutions();
                //JMRCameraManager.Instance.SetPreviewResolution(previewResolutionsList[previewResolutionsList.Count - 1]);
                if (isDebug)
                    Debug.Log("Start Camera Preview");
                if (JMRCameraManager.Instance.BindCameraTexture(cameraFeed))
                {
                    //Start camera preview
                    JMRCameraManager.Instance.StartPreview();
                }
            }
            else
            {
                if (isDebug)
                    Debug.LogError("Camera Preview Image/RawImage is null");
                if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
                {
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR, JMRScreenCastManager.Instance.CASTING_ERROR_CAMERA_ERROR);
                }
                else
                {
                    NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR);

                }
            }
        }

        /// <summary>
        /// Stop camera preview
        /// </summary>
        public void StopCameraPreview(JMRScreenCastBackground backgroundID )
        {
            if (JMRCameraManager.Instance != null)
            {

                JMRCameraManager.Instance.StopPreview();
            }
            else
            {
                if (isDebug)
                    Debug.LogError("Camera Manager instance is null");
            }
        }

        /// <summary>
        /// Starts screen sharing to the respective joined channel
        /// </summary>
        void StartScreenShare()
        {
            screenCastCameraPreview.enabled = true;
            povCam.enabled = true;

            RenderTexture.active = screenCastCameraPreview.targetTexture;

            if (isDebug)
                Debug.Log("ScreenShare Activated");

            if (ConnectWithToken)
            {
                mRtcEngine = IRtcEngine.GetEngine(appId);
            }
            else
            {
                mRtcEngine = IRtcEngine.GetEngine("59185037cebd4acdb839e6d2d60060cb");
            }

            mRtcEngine.SetVideoEncoderConfiguration(SetVideoConfiguration());

            // Creates a texture of the rectangle you create.
            mTexture = new Texture2D((int)screenCastCameraPreview.targetTexture.width, (int)screenCastCameraPreview.targetTexture.height, TextureFormat.RGBA32, false);

            // Sets the output log level of the SDK.
            mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
            // Sets the audio profile
            mRtcEngine.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_HIGH_QUALITY, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_GAME_STREAMING);
            // Enables the video module.
            mRtcEngine.EnableVideo();
            // Enables the video observer.
            mRtcEngine.EnableVideoObserver();
            // Configures the external video source.
            mRtcEngine.SetExternalVideoSource(true, false);
            // Joins a channel
            if (ConnectWithToken)
            {
                if (isDebug)
                    Debug.LogError("Screen Cast Join Room with Key");
                if (TestInEditor)
                {
                    mRtcEngine.JoinChannelByKey(Token, channelName, "", UUID);
                }
                else
                {
                    mRtcEngine.JoinChannelByKey(roomCreds.token, roomCreds.channel, "", roomCreds.userUid);
                }
            }
            else
            {
                mRtcEngine.JoinChannel("temp", null, 0);
            }

            // Creates a texture of the rectangle you create.
            mTexture = new Texture2D((int)screenCastCameraPreview.targetTexture.width, (int)screenCastCameraPreview.targetTexture.height, TextureFormat.RGBA32, false);

            externalVideoFrame = new ExternalVideoFrame();

            isScreenSharing = true;

            NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_ON);

            //mRtcEngine.OnError += OnError;
            mRtcEngine.OnUserJoined += OnUserJoined;

            if (currentScreenBackgroundType.type == JMRScreenCastManager.Instance.BG_TYPE_CAMERA)
            {
                if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
                {
                    cameraFeed.enabled = true;
                }
                else
                {
                    Debug.LogError("JMRSDK->ScreenCasting>: BG_TYPE_CAMERA is not supported for the selected device type");
                }
            }
            if (isDebug)
                Debug.Log("ScreenCastV2: StartShareScreen Completed");
        }

        void OnScreenCastingRequested(RoomCreds rc, JMRScreenCastBackground background)
        {
            if (isDebug)
                Debug.Log("ScreenCastV2: OnScreenCastingRequested");
            Background = background;
            currentScreenBackgroundType = JMRScreenCastManager.Instance.GetCurrentBackground();
            //Deprecated

            if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
            {
                JMRCameraManager.OnCameraPreviewStart += StartScreenShare;
                JMRCameraManager.OnCameraError += CameraError;
            }
            else
            {
                Debug.LogError("JMRSDK->ScreenCasting>: BG_TYPE_CAMERA is not supported for the selected device type");
            }


            if (rc != null)
            {
                roomCreds = rc;
            }


            Setup();
            if (isDebug)
                Debug.LogError("Screen Cast : Load Environment");

            //currentScreenBackground = background.type;

            if (isDebug)
                Debug.LogError("Screen Cast : Loading Environment");

            
            LoadEnvironment(Background);
            if (isDebug)
                Debug.LogError("Screen Cast : Environment Loaded");
        }

        void OnUserJoined(uint uuid, int elapse)
        {
            if (isDebug)
                Debug.LogError($"UUID : {UUID} : Elapsed : {elapse}");
        }

        /// <summary>
        /// Set Video stream configuration
        /// </summary>
        /// <returns></returns>
        VideoEncoderConfiguration SetVideoConfiguration()
        {
            VideoEncoderConfiguration videoEncoderConfiguration = new VideoEncoderConfiguration();

            VideoDimensions dimensions = new VideoDimensions();
            dimensions.height = 1920;
            dimensions.width = 1080;

            videoEncoderConfiguration.dimensions = dimensions;
            videoEncoderConfiguration.frameRate = FRAME_RATE.FRAME_RATE_FPS_60;
            videoEncoderConfiguration.minFrameRate = 30;
            videoEncoderConfiguration.bitrate = 6300;
            videoEncoderConfiguration.minBitrate = 3150;
            videoEncoderConfiguration.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_BALANCED;
            return videoEncoderConfiguration;
        }

        /// <summary>
        /// Send Screen share data to web RTC engine
        /// </summary>
        async void ShareScreen(CancellationToken cancellationToken)
        {
            //yield return new WaitForEndOfFrame();
            await Task.Delay(0, cancellationToken);

            //cam.targetTexture = rt;
            RenderTexture.active = screenCastCameraPreview.targetTexture;
            screenCastCameraPreview.Render();

            //Read pixel data
            mTexture.ReadPixels(new Rect(0, 0, screenCastCameraPreview.targetTexture.width, screenCastCameraPreview.targetTexture.height), 0, 0);

            RenderTexture.active = null;

            // Reads the Pixels of the rectangle you create.
            // mTexture.ReadPixels(mRect, 0, 0);

            // Applies the Pixels read from the rectangle to the texture.
            mTexture.Apply();
            // Gets the Raw Texture data from the texture and apply it to an array of bytes.
            bytes = mTexture.GetRawTextureData();

            // Gives enough space for the bytes array.
            // int size = Marshal.SizeOf(bytes[0]) * bytes.Length;

            // Checks whether the IRtcEngine instance is existed.
            IRtcEngine rtc = IRtcEngine.QueryEngine();
            if (rtc != null)
            {
                // Creates a new external video frame.
                //ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();

                // Sets the buffer type of the video frame.
                externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
                // Sets the format of the video pixel.
                externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
                // Applies raw data.
                externalVideoFrame.buffer = bytes;
                // Sets the width (pixel) of the video frame.
                externalVideoFrame.stride = (int)screenCastCameraPreview.targetTexture.width;
                // Sets the height (pixel) of the video frame.
                externalVideoFrame.height = (int)screenCastCameraPreview.targetTexture.height;
                // Removes pixels from the sides of the frame
                externalVideoFrame.cropLeft = 10;
                externalVideoFrame.cropTop = 10;
                externalVideoFrame.cropRight = 10;
                externalVideoFrame.cropBottom = 10;
                // Rotates the video frame (0, 90, 180, or 270)
                externalVideoFrame.rotation = 180;
                // Calculates the video timestamp in milliseconds according to the system time.
                externalVideoFrame.timestamp = System.DateTime.Now.Ticks / 10000;
                // Pushes the external video frame with the frame you create.
                int a = rtc.PushVideoFrame(externalVideoFrame);
            }

            bytes = null;
        }

        void HandleInternetConnection()
        {
            if (!Application.isEditor)
            {
                StartCoroutine(CheckInternetConnection(isConnected =>
                {
                    if (isConnected)
                    {
                        if (TestInEditor)
                        {
                            StartScreenShare();
                        }
                        else
                        {                            
                            if(currentScreenBackgroundType != null)
                            {
                                if (currentScreenBackgroundType.type == JMRScreenCastManager.Instance.BG_TYPE_CAMERA)
                                {
                                    if (JMRRigManager.Instance.getDeviceID() == (int)JMRRigManager.DeviceType.JioGlass)
                                    {
                                        CheckCastingStateIfInternet();
                                    }
                                    else
                                    {
                                        Debug.LogError("JMRSDK->ScreenCasting>: BG_TYPE_CAMERA is not supported for the selected device type");
                                    }

                                }
                                else if (!isScreenSharing && JMRScreenCastManager.Instance.GetCastingState() == JMRScreenCastManager.Instance.STATE_CASTING_ERROR)
                                {
                                    if (currentScreenBackgroundType.type == JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND)
                                    {
                                        cameraFeed.enabled = false;
                                    }

                                    StartScreenShare();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isDebug)
                            Debug.LogError("No Internet Connection available : HandleInternet");

                        LeaveChanel(Background);
                        StopCameraPreview(Background);

                        //NotifyStateAndErrorCode(JMRScreenCastManager.Instance.STATE_CASTING_ERROR, JMRScreenCastManager.Instance.CASTING_ERROR_NO_INTERNET);

                    }

                }));
            }
        }

        private void CheckCastingStateIfInternet()
        {
            if (!isScreenSharing)
            {
                if (JMRScreenCastManager.Instance.GetCastingState() == JMRScreenCastManager.Instance.STATE_CASTING_ERROR)
                {
                    Invoke("StartCameraPreview", 1f);
                }
            }
        }

        /// <summary>
        /// Check if internet connection is working or not
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator CheckInternetConnection(Action<bool> action)
        {
            UnityWebRequest request = new UnityWebRequest("https://jiocinema.blob.core.windows.net/cinema/ping.txt");

            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.LogError("Internet Connected : Error");
                action(false);
            }
            else
            {
                if (isDebug)
                    Debug.Log("Internet Connected : Success");
                action(true);
            }
        }

        #region Loading Environment
        IEnumerator DownloadBackground(JMRScreenCastBackground background)
        {
            if (isDebug)
                Debug.Log("DownloadBackground " + background.link);
            
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(background.link);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if(defaultEnvironmentModel != null)
                {
                    Destroy(defaultEnvironmentModel);
                    defaultEnvironmentModel = null;
                }
                defaultEnvironmentModel = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (isDebug)
                    Debug.Log("defaultEnvironmentModel " + defaultEnvironmentModel.name);
                //UpdateBackground(index);
                UpdateBackground(background);
            }
            /*defaultEnvironment = (ScreenCastModelEnum)environmentSO.environments[index - 1].modelEnum;*/
            
        }

        public void LoadEnvironment(JMRScreenCastBackground screenCast)
        {
            if (isDebug)
                Debug.LogError("Screen Cast : Background Type: " + screenCast.type);
            if (screenCast.type == JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND)
            {
                if (isDebug)
                    Debug.LogError("Casting Environment: No background for this index: " + screenCast.type);
                UpdateBackground(screenCast);
                return;
            }
            /*defaultEnvironment = (ScreenCastModelEnum)screenCast;*/
            if (screenCast.type == JMRScreenCastManager.Instance.BG_TYPE_BACKGROUND)
            {
                StartCoroutine(DownloadBackground(screenCast));

            }
        }

        /// <summary>
        /// For Editor
        /// </summary>
        /// <param name="index"></param>
        private void UpdateBackground(JMRScreenCastBackground background)
        {

            if (isDebug)
                Debug.Log(" defaultEnvironmentModel " + defaultEnvironmentModel == null);


            if (defaultEnvironmentModel != null)
            {
                if (isDebug)
                {
                    Debug.Log(" castCamera " + castCamera == null);
                    Debug.Log(" skyboxBg " + skyboxBg == null);
                    Debug.Log(" skyboxBg.material " + skyboxBg.material == null);

                }
                castCamera.clearFlags = CameraClearFlags.Skybox;
                skyboxBg.material.SetTexture("_MainTex", defaultEnvironmentModel);
                //RenderSettings.skybox.SetTexture("_MainTex", defaultEnvironmentModel);
            }
            else
            {
                if (isDebug)
                    Debug.Log("UpdateBackground defaultEnvironmentModel is null");
                castCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            

            if (background.type == JMRScreenCastManager.Instance.BG_TYPE_NO_BACKGROUND)
            {
                ClearBackground();
            }
            if (isDebug)
                Debug.Log("Environment Updated to :" + background.type);
        }

        private void ClearBackground()
        {
            if (defaultEnvironmentModel != null)
            {
                Destroy(defaultEnvironmentModel);
                defaultEnvironmentModel = null;
            }
            castCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void GetSkyboxComponent()
        {
            if(castCamera == null)
            {
                return;
            }
            skyboxBg = castCamera.GetComponent<Skybox>();
            if (skyboxBg == null)
            {
                castCamera.clearFlags = CameraClearFlags.SolidColor;
            }
        }
        #endregion
    }
}