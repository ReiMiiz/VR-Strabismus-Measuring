using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System.IO;

public class TestController : MonoBehaviour {

    public GameObject board1;
    public GameObject board2;
    public Camera LeftCamera;
    public Camera RightCamera;
    public Camera CapCamera;
    public Camera MenuCamera;
    public GameObject HUD;
    public GameObject savedUI1;
    public GameObject savedUI2;

    public InputField logname;
    public KeyCode saveLog;

    public string folder;

    private int counter = 0;

    private Controller ctr1;
    private Controller ctr2;

    private Vector3 CapturePos = new Vector3(.75f, .0f, .0f);

    // Use this for initialization
    void Start () {
        SetCamera();

        ctr1 = board1.GetComponent<Controller>();
        ctr2 = board2.GetComponent<Controller>();

        CapCamera.enabled = false;
        MenuCamera.enabled = false;

        board1.SetActive(true);
        board2.SetActive(false);

        ctr1.activated = true;
        ctr2.activated = false;

        HUD.SetActive(false);
        savedUI1.SetActive(false);
        savedUI2.SetActive(false);

        //LeftCamera.transform.Rotate(new Vector3(-5.0f, -5.0f, 0.0f));
    }
	
    void SetCamera()
    {
        InputTracking.disablePositionalTracking = true;
        XRDevice.DisableAutoXRCameraTracking(LeftCamera, true);
        XRDevice.DisableAutoXRCameraTracking(RightCamera, true);
        XRDevice.DisableAutoXRCameraTracking(CapCamera, true);
        LeftCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        RightCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        CapCamera.transform.SetPositionAndRotation(CapturePos, Quaternion.Euler(Vector3.zero));
    }

    IEnumerator SwitchDevice(string deviceName)
    {
        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName(deviceName);

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        // Not needed, since loading the None (`""`) device takes care of this.
        XRSettings.enabled = true;

        // Restore 2D camera settings.
        //SetCamera();
    }

    // Update is called once per frame
    void Update () {

        if (ctr1.isFinished && ctr2.isFinished)
        {
            //Debug.Log("FINISHED!!");
            HUD.SetActive(true);

            board1.SetActive(true);
            board2.SetActive(true);

            ctr1.activated = false;
            ctr2.activated = false;

            CapCamera.enabled = false;
            RightCamera.enabled = false;
            LeftCamera.enabled = false;
            MenuCamera.enabled = true;

            StartCoroutine(SwitchDevice("None"));
        }
        else if (ctr1.isFinished)
        {
            board1.SetActive(false);
            board2.SetActive(true);

            ctr1.activated = false;
            ctr2.activated = true;

            //LeftCamera.transform.rotation = Quaternion.Euler(new Vector3(5.0f, 5.0f, 0.0f));

            LeftCamera.transform.position = new Vector3(1.5f, 0.0f, 0.0f);
            RightCamera.transform.position = new Vector3(1.5f, 0.0f, 0.0f);

        }

        if(Input.GetKeyDown(saveLog))
        {
            saveLogFile();
        }
    }

    public void saveLogFile()
    {
        string filename = uniqueFilename();

        var f = System.IO.File.CreateText(filename);
        f.Write(ctr1.log + ctr2.log);
        f.Close();
        Debug.Log(string.Format("Wrote logfile: {0}", filename));
        savedUI1.SetActive(true);

    }


    private string uniqueFilename()
    {
        // if folder not specified by now use a good default
        if (folder == null || folder.Length == 0)
        {
            folder = Application.persistentDataPath;
            if (Application.isEditor)
            {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }
            folder += "/screenshots";

            // make sure directoroy exists
            System.IO.Directory.CreateDirectory(folder);

            // count number of files of specified format in folder
            string mask = string.Format("ResultFile*.txt");
            counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
        }

        string filename;
        // use width, height, and counter for unique file name
        if (logname.text == null || logname.text.Length == 0)
            filename = string.Format("{0}/ResultFile{1}.txt", folder, counter);
        else
            filename = string.Format("{0}/{1}.txt", folder, logname.text);

        //debugtext.GetComponent<UnityEngine.UI.Text>().text = string.Format("path:{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, counter, format.ToString().ToLower());

        // up counter for next call
        ++counter;

        // return unique filename
        return filename;
    }
}
