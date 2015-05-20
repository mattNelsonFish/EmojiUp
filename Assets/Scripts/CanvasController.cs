using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasController : MonoBehaviour {

    [SerializeField] Canvas mainmenuCanvas;
    [SerializeField] Canvas gameCanvas;
    [SerializeField] Canvas backdropCanvas;
    [SerializeField] Canvas rulesCanvas;
    List<Canvas> canvasList;
    List<Canvas> prevList;
    void Awake() {
        Init();
    }

    void Init() {
        prevList = new List<Canvas>();
        canvasList = new List<Canvas>();
        canvasList.Add(mainmenuCanvas);
        canvasList.Add(gameCanvas);
        canvasList.Add(backdropCanvas);
        canvasList.Add(rulesCanvas);

        mainmenuCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
        backdropCanvas.gameObject.SetActive(true);
        rulesCanvas.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeCanvas(string canvasName) {
        Canvas temp = GetCanvas(canvasName);


        if (temp != null) {
            canvasName = canvasName.ToLower();
            switch (canvasName) {
                case "mainmenu":
                    temp = mainmenuCanvas;
                    mainmenuCanvas.gameObject.SetActive(true);
                    gameCanvas.gameObject.SetActive(false);
                    backdropCanvas.gameObject.SetActive(true);
                    rulesCanvas.gameObject.SetActive(false);
                    break;
                case "game":
                    temp = gameCanvas;
                    mainmenuCanvas.gameObject.SetActive(false);
                    gameCanvas.gameObject.SetActive(true);
                    backdropCanvas.gameObject.SetActive(true);
                    rulesCanvas.gameObject.SetActive(false);
                    break;
                case "backdrop":
                    temp = backdropCanvas;
                    gameCanvas.gameObject.SetActive(false);
                    mainmenuCanvas.gameObject.SetActive(false);
                    backdropCanvas.gameObject.SetActive(true);
                    rulesCanvas.gameObject.SetActive(false);
                    break;
                case "rules":
                    temp = rulesCanvas;
                    gameCanvas.gameObject.SetActive(false);
                    backdropCanvas.gameObject.SetActive(true);
                    mainmenuCanvas.gameObject.SetActive(false);
                    rulesCanvas.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("unknown canvas wanted, but not found: " + canvasName);
                    break;
            }
            zeroHiddenCanvases();
            temp.sortingOrder = GetTopmostCanvas().sortingOrder + 1;
            prevList.Clear();
        }
    }

    /// <summary>
    /// Additively changes canvas focus. Activates the chosen canvas as the highest in sorting order.
    /// </summary>
    /// <param name="canvasName"></param>
    public void AddChangeCanvas(string canvasName) {
        Canvas temp = GetCanvas(canvasName);


        if (temp != null) {
            temp.sortingOrder = GetTopmostCanvas().sortingOrder + 1;
            temp.gameObject.SetActive(true);
            prevList.Add(temp);
        }
    }

    Canvas GetTopmostCanvas() {
        Canvas temp = null;
        int canvlayer = -999;
        foreach (Canvas canv in canvasList) {
            if (temp == null || canv.sortingOrder > canvlayer) {
                temp = canv;
                canvlayer = canv.sortingOrder;
            }
        }

        return temp;
    }

    /// <summary>
    /// from a provided name, a canvas will be returned.
    /// </summary>
    Canvas GetCanvas(string canvasName) {
        Canvas canvas = null;
        canvasName = canvasName.ToLower();
        switch (canvasName) {
            case "mainmenu":
                canvas = mainmenuCanvas;
                break;
            case "game":
                canvas = gameCanvas;
                break;
            case "backdrop":
                canvas = backdropCanvas;
                break;
            case "rules":
                canvas = rulesCanvas;
                break;
            default:
                Debug.LogWarning("unknown canvas wanted, but not found: " + canvasName);
                break;
        }

        return canvas;
    }

    void zeroHiddenCanvases() {
        foreach (Canvas canv in canvasList) {
            if ( !canv.gameObject.activeSelf) {
                canv.sortingOrder = 0;
            }
        }
    }

    public void HideTopCanvas() {
        GetTopmostCanvas().gameObject.SetActive(false);
        zeroHiddenCanvases();
    }

    public void ShowPrevCanvas() {
        if (prevList.Count > 0) {
            HideTopCanvas();
        }
    }
}
