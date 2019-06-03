using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controller : MonoBehaviour
{
    public GameObject outerdots;
    public GameObject innerdots;
    public GameObject originDot;
    public GameObject dotPrefab;

    public GameObject innerOutput;
    public GameObject outerOutput;
    public GameObject HUD;

    public bool activated;
    public bool followCurve;
    public bool useLine;
    public GameObject circleTarget;
    public GameObject lineTarget;
    public GameObject circleTester;
    public GameObject lineTester;
    public GameObject moveDummy;
    public float xSensitivity;
    public float ySensitivity;
    public float rotateSensitivity;
    public float slowSensitivity;

    public KeyCode acceptButton;
    public KeyCode slowButton;
    public KeyCode resetButton;
    public KeyCode rotateLeft;
    public KeyCode rotateRight;

    [HideInInspector]
    public bool isFinished = false;
    [HideInInspector]
    public string log = "";

    private GameObject[] iDots;
    private GameObject[] oDots;
    private GameObject[] Dots;

    public float xoffset;
    private float velx, vely, posx, posy, posz;
    private float resX, resY;
    private LineRenderer innerLine, outerLine;

    private int currentIndex = 0;
    private int maxIndex;
    private float slowFactor = 1.0f;

    private GameObject resultList;

    void Start()
    {
        Debug.Log("Initializing...");

        maxIndex = innerdots.transform.childCount + outerdots.transform.childCount + 1;

        resultList = new GameObject();
        oDots = new GameObject[outerdots.transform.childCount];
        iDots = new GameObject[innerdots.transform.childCount];
        Dots = new GameObject[maxIndex];

        Dots[0] = originDot;

        for (int i = 0; i < innerdots.transform.childCount; i++)
        {
            Dots[i+1] = innerdots.transform.GetChild(i).gameObject;
            iDots[i] = innerdots.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < outerdots.transform.childCount; i++)
        {
            Dots[i + innerdots.transform.childCount + 1] = outerdots.transform.GetChild(i).gameObject;
            oDots[i] = outerdots.transform.GetChild(i).gameObject;
        }

        innerLine = innerOutput.GetComponent<LineRenderer>();
        outerLine = outerOutput.GetComponent<LineRenderer>();

        //this.gameObject.SetActive(activated);

        if (useLine)
        {
            circleTarget.SetActive(false);
            circleTester.SetActive(false);
            lineTarget.SetActive(true);
            lineTester.SetActive(true);
        }
        else
        {
            circleTarget.SetActive(true);
            circleTester.SetActive(true);
            lineTarget.SetActive(false);
            lineTester.SetActive(false);
        }

        log = "============" + name + "=============\n" +
            "Original Position ::::::: User Position, Rotation\n";

        Debug.Log("Finished...");
    }

    void moveTarget()
    {
        moveDummy.transform.position += new Vector3(velx, vely, 0.0f) * Time.deltaTime * slowFactor;

        posx = Mathf.Sin(moveDummy.transform.position.x - xoffset) + xoffset;
        posy = Mathf.Cos(moveDummy.transform.position.y + Mathf.PI/2);
        posz = Mathf.Cos(moveDummy.transform.position.x - xoffset) * Mathf.Sin(moveDummy.transform.position.y + Mathf.PI / 2);

        circleTarget.transform.position = new Vector3(posx, posy, posz - 0.03f);
        lineTarget.transform.position = new Vector3(posx, posy, posz - 0.03f);
    }

    void inputUpdate()
    {
        if (!activated) return;

        velx = Input.GetAxis("Horizontal") * xSensitivity;
        vely = Input.GetAxis("Vertical") * -ySensitivity;

        if (Input.GetKeyDown(acceptButton))
        {
            if (currentIndex < maxIndex)
            {

                resX = Mathf.Rad2Deg * Mathf.Asin(posx - xoffset);
                resY = (Mathf.Rad2Deg * Mathf.Acos(posy)) - 90.0f;

                float oriX = Mathf.RoundToInt(Mathf.Rad2Deg * Mathf.Asin(Dots[currentIndex].transform.position.x - xoffset));
                float oriY = Mathf.RoundToInt(Mathf.Rad2Deg * Mathf.Acos(Dots[currentIndex].transform.position.y)) - 90;
                float rotZ = lineTarget.transform.rotation.eulerAngles.z;

                Debug.Log(string.Format("{0},{1}::::::{2},{3} Angle: {4}", oriX, oriY, resX, resY, rotZ));

                //HUD.GetComponent<UnityEngine.UI.Text>().text += string.Format("{0},{1}::::::{2},{3}\n", oriX, oriY, resX, resY);
                log += string.Format("{0},{1}:::{2},{3} Angle: {4}\n", oriX, oriY, resX, resY, rotZ);

                setResultLine();
            }

        }
        else if (Input.GetKeyDown(slowButton))
        {
            slowFactor = slowSensitivity;
        }
        else if (Input.GetKeyUp(slowButton))
        {
            slowFactor = 1.0f;
        }
        else if (Input.GetKeyDown(resetButton))
        {
            resetBoard();
        }
        else if (Input.GetKey(rotateLeft))
        {
            lineTarget.transform.Rotate(new Vector3(0.0f, 0.0f, rotateSensitivity * Time.deltaTime));
        }
        else if (Input.GetKey(rotateRight))
        {
            lineTarget.transform.Rotate(new Vector3(0.0f, 0.0f, -rotateSensitivity * Time.deltaTime));
        }
    }

    public void resetBoard()
    {
        currentIndex = 0;
        outerLine.positionCount = 0;
        innerLine.positionCount = 0;
        outerLine.loop = false;
        innerLine.loop = false;

        freeze(false);
        foreach (Transform child in resultList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void setResultLine()
    {
        if (currentIndex == 0)
        {
            plotDot();
            currentIndex += 1;
        }
        else if (currentIndex < iDots.Length + 1)
        {
            plotDot();
            innerLine.positionCount += 1;
            innerLine.SetPosition(innerLine.positionCount - 1, circleTarget.transform.position + new Vector3(0.0f, 0.0f, -0.01f));
            currentIndex += 1;
        }
        else if (currentIndex < maxIndex)
        {
            plotDot();
            outerLine.positionCount += 1;
            outerLine.SetPosition(outerLine.positionCount - 1, circleTarget.transform.position + new Vector3(0.0f, 0.0f, -0.01f));
            currentIndex += 1;
        }
        
    }

    void plotDot()
    {
        GameObject clone = Instantiate(dotPrefab, circleTarget.transform.position, circleTarget.transform.rotation, resultList.transform);

    }

    void resultUpdate()
    {
        if (currentIndex < 9) circleTester.transform.position = Dots[currentIndex].transform.position + new Vector3(0 , 0, -0.03f);
        else
        {
            //Debug.Log("TEST FINISHED2!!");

            isFinished = true;
            outerLine.loop = true;

            freeze(true);
        }

        if (currentIndex == innerdots.transform.childCount+1) innerLine.loop = true;
    }

    void freeze(bool f)
    {
        circleTarget.SetActive(!f);
        circleTester.SetActive(!f);
        lineTarget.SetActive(!f);
        lineTester.SetActive(!f);
    }

    void Update()
    {
        inputUpdate();
        resultUpdate();
        moveTarget();

        //lineTarget.transform.position = circleTarget.transform.position;
        lineTester.transform.position = circleTester.transform.position;
    }

}
