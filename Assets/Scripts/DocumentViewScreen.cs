using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZenFulcrum.EmbeddedBrowser;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RevolutionGames
{

    public class SortNameDocument : IComparer<DataClass.ProjectDetails>
    {
        public int Compare(DataClass.ProjectDetails x, DataClass.ProjectDetails y)
        {
            return x.folder_name.CompareTo(y.folder_name);
        }
    }

    public class DocumentViewScreen : MonoBehaviour
    {
        #region Variables

        public GameObject selectText;
        public GameObject webView;
        public Browser browser;
        public RectTransform scrollContent;
        public GridLayoutGroup contentGridLayoutGroup;
        public GameObject folderPrefab;
        public GameObject filePrefab;
        public Text documentNameText;
        public GameObject loadingScreen;
        public UploadFileScreen uploadFileScreen;
        public GameObject uploadFileScreenObject;
        public GameObject dummyViewOject;
        public GameObject annotationPhotosScreen;

        private List<GameObject> folderObjectList = new List<GameObject>();
        private List<GameObject> fileObjectList = new List<GameObject>();
        private List<GameObject> bothObjectList = new List<GameObject>();
        private GameObject parentFolder;

        private Color32 normalColor = new Color32(229, 229, 229, 0);
        private Color32 selectColor = new Color32(242, 140, 43, 255);

        private List<DataClass.ProjectDetails> projectDetails;
        private List<DataClass.ProjectDetails> projectDetailsDummy = new List<DataClass.ProjectDetails>();
        private List<int> folderLevels = new List<int>();
        private List<float> gridCellSizeList = new List<float>();

        private float default_width = 285;
        private float final_width = 285;
        private int folderCount = 1;
        private float defaultGridSizeX = 400;
        private float defaultGridSizeY = 60;
        private float addGridSizeX = 120;
        private float parentMoveValue = 200;
        private float folderLevelAddValue = 20;
        private bool selectPdfFlag = false;
        private string pdfPath;
        private float initialWebViewHeight;
        private bool initialStart = false;

        // Annotation Media
        private float annotationScreenHeight = 170.7f;
        private List<GameObject> annotationMediaList = new List<GameObject>();
        public GameObject annotationMediaPrefab;
        public RectTransform annotationMediaContent;
        public GameObject noMediaFound;
        public GameObject annotationMediaPreviewScreen;

        #endregion

        #region Built in Methods
        void Start()
        {
            //GetProjectFolder();
            //CreateTreeViewRootFolder();
            uploadFileScreen.initializeText.SetActive(false);
            uploadFileScreen.messageText.SetActive(false);
            browser.m_MyEvent.AddListener(AnnotationClickedFromWeb);
            //LoadDummyImages();
        }
        void OnEnable()
        {
            scrollContent.anchoredPosition = Vector2.zero;
            //LoadDummyImages();
            if (initialStart == false)
            {
                initialStart = true;
                initialWebViewHeight = webView.transform.GetComponent<RectTransform>().rect.height;
            }

            webView.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWebViewHeight);
            annotationMediaPreviewScreen.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWebViewHeight);

            selectText.SetActive(true);
            webView.SetActive(false);
            annotationPhotosScreen.SetActive(false);
            annotationMediaPreviewScreen.SetActive(false);
            noMediaFound.SetActive(false);
            documentNameText.text = "";
            CreateTreeViewRootFolder();
        }
        #endregion

        #region Custom Methods

        public void OnBackButtonClicked()
        {
            if(webView.activeSelf)
              browser.CookieManager.ClearAll();

            uploadFileScreen.DeleteOldFiles();
            uploadFileScreenObject.SetActive(true);
            dummyViewOject.SetActive(false);
            this.gameObject.SetActive(false);
        }

        public void GetProjectFolder()
        {
            projectDetails = DataClass.Instance().projectDetails;

            for (int i = 0; i < scrollContent.childCount; i++)
            {
                Destroy(scrollContent.GetChild(i).gameObject);
            }
            folderObjectList.Clear();
            fileObjectList.Clear();
            bothObjectList.Clear();
            projectDetailsDummy.Clear();
            for (int i = 0; i < 3; i++)
            {

                if (i == 0)
                {
                    GameObject folder = Instantiate(folderPrefab, scrollContent);
                    folderObjectList.Add(folder);
                    bothObjectList.Add(folder);
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    parentFolder = folderObjectList[0];

                    folderObjectList[i].transform.GetChild(0).GetChild(0).localPosition = new Vector2((i * 40) + 200, 0);

                    float value = folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                    float finalValue = 0;
                    if (value > 285)
                    {
                        finalValue = value - 285;
                    }
                    print(finalValue);
                    contentGridLayoutGroup.cellSize = new Vector2((i * 40) + finalValue + 400, (i + 1) * 60);
                    
                     for (int j = 0; j < folderObjectList.Count; j++)
                     {
                        folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);

                    }

                }

                else
                {
                    GameObject folder = Instantiate(filePrefab, parentFolder.transform);
                    fileObjectList.Add(folder);
                    bothObjectList.Add(folder);
                    int count = fileObjectList.Count - 1;
                    fileObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = name ;
                    fileObjectList[count].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    //parentFolder = folderObjectList[0];

                    fileObjectList[count].transform.GetChild(0).localPosition = new Vector2((i * 40) + 200, 0);

                    float value = fileObjectList[count].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                    float finalValue = 0;
                    if (value > 285)
                    {
                        finalValue = value - 285;
                    }
                    print(finalValue);
                    contentGridLayoutGroup.cellSize = new Vector2((i * 40) + finalValue + 400, (i + 1) * 60);
                    for (int j = 0; j < folderObjectList.Count; j++)
                    {
                        folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);

                    }
                    for (int j = 0; j < fileObjectList.Count; j++)
                    {
                        fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);

                    }
                }


            }

        }



        public void CreateTreeViewRootFolder()
        {
            folderCount = 1;
            SortNameDocument sortName = new SortNameDocument();
            DataClass.Instance().projectDetails.Sort(sortName);
            projectDetails = DataClass.Instance().projectDetails;

            for (int i = 0; i < scrollContent.childCount; i++)
            {
                Destroy(scrollContent.GetChild(i).gameObject);
            }
            folderObjectList.Clear();
            fileObjectList.Clear();
            bothObjectList.Clear();
            projectDetailsDummy.Clear();
            folderLevels.Clear();

            for (int i = 0; i < projectDetails.Count; i++)
            {
                folderLevels.Add(projectDetails[i].folder_level);
            }

            int leastLevel = Mathf.Min(folderLevels.ToArray());

            for (int i = 0; i < projectDetails.Count; i++)
            {
                projectDetails[i].folder_level -= leastLevel;
                print(projectDetails[i].folder_level);
            }

            print(leastLevel);

            for (int i = 0; i < projectDetails.Count; i++)
            {

                if (projectDetails[i].folder_level == 0)
                {
                    if (projectDetails[i].is_folder_flag == 1)
                    {
                        GameObject folder = Instantiate(folderPrefab, scrollContent);
                        folder.name = projectDetails[i].folder_id;
                        folderObjectList.Add(folder);
                        bothObjectList.Add(folder);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = folderObjectList.Count - 1;
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                        parentFolder = folderObjectList[0];

                        folderObjectList[count].transform.GetChild(0).GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }
                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }

                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;

                        }
                    }
                    else
                    {
                        GameObject file = Instantiate(filePrefab, scrollContent);
                        file.name = projectDetails[i].folder_id;
                        fileObjectList.Add(file);
                        bothObjectList.Add(file);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = fileObjectList.Count - 1;
                        parentFolder = fileObjectList[0];
                        fileObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        fileObjectList[count].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        //parentFolder = folderObjectList[0];

                        fileObjectList[count].transform.GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = fileObjectList[count].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }
                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }

                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        for (int j = 0; j < fileObjectList.Count; j++)
                        {
                            fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        ExpandAndCollapseButtonEnable();
                    }
                    

                }
            }

            for (int i = 0; i < projectDetails.Count; i++)
            {
                if (projectDetailsDummy[0].folder_id == projectDetails[i].parent_folder_id)
                {
                    if (projectDetails[i].is_folder_flag == 1)
                    {
                        GameObject folder = Instantiate(folderPrefab, parentFolder.transform);
                        folder.name = projectDetails[i].folder_id;
                        folderObjectList.Add(folder);
                        bothObjectList.Add(folder);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = folderObjectList.Count - 1;
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                        //parentFolder = folderObjectList[0];

                        folderObjectList[count].transform.GetChild(0).GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }
                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }

                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        for (int j = 0; j < fileObjectList.Count; j++)
                        {
                            fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                    }
                    else
                    {
                        GameObject folder = Instantiate(filePrefab, parentFolder.transform);
                        folder.name = projectDetails[i].folder_id;
                        fileObjectList.Add(folder);
                        bothObjectList.Add(folder);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = fileObjectList.Count - 1;
                        fileObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        fileObjectList[count].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        //parentFolder = folderObjectList[0];

                        fileObjectList[count].transform.GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = fileObjectList[count].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }
                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }

                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        for (int j = 0; j < fileObjectList.Count; j++)
                        {
                            fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                    }
                }
            }

            ProjectFolderTreeViewCall();
        }

        public void ProjectFolderTreeViewCall()
        {
            //print("folderCount main :" + folderObjectList.Count);
            int count = folderObjectList.Count;
            if (count != folderCount)
            {
                if (folderObjectList.Count > folderCount)
                    ProjectFolderTreeView(folderObjectList[folderCount], count);
                for (int i = folderCount; i < count; i++)
                {
                    
                }
            }
            else
            {
                print("No");
                ExpandAndCollapseButtonEnable();
            }

        }

        public void ProjectFolderTreeView(GameObject gameObject, int index)
        {
            folderCount = folderCount + 1;
            parentFolder = gameObject;


            for (int i = 0; i < projectDetails.Count; i++)
            {
                if (projectDetails[i].parent_folder_id == gameObject.name)
                {
                    if (projectDetails[i].is_folder_flag == 1)
                    {
                        GameObject folder = Instantiate(folderPrefab, parentFolder.transform);
                        folder.name = projectDetails[i].folder_id;
                        folderObjectList.Add(folder);
                        bothObjectList.Add(folder);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = folderObjectList.Count - 1;
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                        //parentFolder = folderObjectList[0];

                        folderObjectList[count].transform.GetChild(0).GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }
                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }

                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        for (int j = 0; j < fileObjectList.Count; j++)
                        {
                            fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                    }
                    else
                    {
                        GameObject folder = Instantiate(filePrefab, parentFolder.transform);
                        folder.name = projectDetails[i].folder_id;
                        fileObjectList.Add(folder);
                        bothObjectList.Add(folder);
                        projectDetailsDummy.Add(projectDetails[i]);
                        int count = fileObjectList.Count - 1;
                        fileObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                        fileObjectList[count].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        //parentFolder = folderObjectList[0];

                        fileObjectList[count].transform.GetChild(0).localPosition = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + parentMoveValue, 0);

                        float value = fileObjectList[count].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().preferredWidth;

                        //contentGridLayoutGroup.cellSize = new Vector2((projectDetails[i].folder_level * folderLevelAddValue) + finalValue + 400, (bothObjectList.Count) * 60);
                        float gridSizeX = (projectDetails[i].folder_level * folderLevelAddValue) + value + addGridSizeX;
                        if (gridSizeX < defaultGridSizeX)
                        {
                            gridSizeX = defaultGridSizeX;
                        }

                        if (gridSizeX > contentGridLayoutGroup.cellSize.x)
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(gridSizeX, (bothObjectList.Count) * defaultGridSizeY);
                        }
                        else
                        {
                            contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (bothObjectList.Count) * defaultGridSizeY);
                        }


                        for (int j = 0; j < folderObjectList.Count; j++)
                        {
                            folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                        for (int j = 0; j < fileObjectList.Count; j++)
                        {
                            fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                            fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                        }
                    }
                }
            }


            if (folderCount == index - 1)
            {

                if (projectDetailsDummy.Count <= projectDetails.Count)
                {
                    //ProjectFolderTreeViewCall();
                }
            }
            if (projectDetailsDummy.Count == projectDetails.Count)
            {
                ExpandAndCollapseButtonEnable();
            }
            else
            {
                ProjectFolderTreeViewCall();
            }
            //print("projectFoldersDummy.Count :" + projectDetailsDummy.Count);
            //print("projectFoldersOriginal.Count :" + projectDetails.Count);

        }

        public void ExpandAndCollapseButtonEnable()
        {
            print("button action");
            for (int i = 0; i < folderObjectList.Count; i++)
            {
                int count = i;
                folderObjectList[i].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnFolderButtonClicked(count, folderObjectList[count]));
                if (folderObjectList[i].transform.childCount > 1)
                {
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false);
                   

                }
                else
                {
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < fileObjectList.Count; i++)
            {
                int count = i;
                fileObjectList[i].transform.GetComponent<Button>().onClick.AddListener(() => OnFileButtonClicked(count, fileObjectList[count]));
                fileObjectList[i].transform.GetComponent<Image>().color = normalColor;
            }

        }

        public void OnFileButtonClicked(int count,GameObject gameObject)
        {
            selectPdfFlag = false;
            //print("file button"+ gameObject.name);
            loadingScreen.SetActive(true);
            for (int i = 0; i < projectDetails.Count; i++)
            {
                if (gameObject.name == projectDetails[i].folder_id)
                {
                    documentNameText.text = RemoveSpecialCharacters(projectDetails[i].folder_name);
                    selectText.SetActive(false);
                    //browser.Url = "https://www.revolutiongamesindia.com/";
                    //browser.Url = "https://qa.plannotate3.com/";
                    //byte[] bytesToEncode = File.ReadAllBytes("C:/Users/sreer/Downloads/a039793ef63fabc46549c0499acff6a3.jpg");
                    //byte[] bytesToEncode = File.ReadAllBytes("C:/Users/sreer/Downloads/a039793ef63fabc46549c0499acff6a3.jpg");
                    //byte[] decBytes1 = Encoding.UTF8.GetBytes(s1);
                    //string encodedText = Convert.ToBase64String(bytesToEncode);
                    //print(encodedText);
                    //byte[] sPDFDecoded = Convert.FromBase64String(encodedText);
                    //BinaryWriter writer = new BinaryWriter(File.Open(@"c:/Users/sreer/Downloads/pdf9.jpg", FileMode.CreateNew));
                    //writer.Write(sPDFDecoded);

                    /*string path = uploadFileScreen.webFileLoadPath;
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo directory = new DirectoryInfo(path);
                        foreach (FileInfo file in directory.GetFiles())
                        {
                            if (file.Name == projectDetails[i].folder_id+".pdf")
                            {
                                //print(file.FullName);
                                pdfPath = file.FullName;
                                selectPdfFlag = true;
                                break;
                            }

                        }
                    }

                    if (selectPdfFlag == true)
                    {
                        print("pdf");
                        browser.Url = "file:///" + uploadFileScreen.webFileLoadPath+"/dist/index.html?folder_id=" + projectDetails[i].folder_id + "&project_id=" + projectDetails[i].project_id + "&folder_path=" + pdfPath + "/";
                    }
                    else
                    {
                        print("image");
                        browser.Url = "file:///" + uploadFileScreen.webFileLoadPath + "/dist/index.html?folder_id=" + projectDetails[i].folder_id + "&project_id=" + projectDetails[i].project_id + "/";
                    }*/

                    dummyViewOject.SetActive(true);
                    webView.SetActive(true);
                    //browser.Url = "file:///" + uploadFileScreen.webFileLoadPath + "/dist/index.html?folder_id=" + projectDetails[i].folder_id + "&project_id=" + projectDetails[i].project_id + "&folder_path=" + "\"" + uploadFileScreen.webFileLoadPath+"/" + "\"";
                    browser.Url = "file:///" + uploadFileScreen.file_directory_Path + "dist/index.html?folder_id=" + projectDetails[i].folder_id + "&project_id=" + projectDetails[i].project_id + "&folder_path=" + "\"" + uploadFileScreen.webFileLoadPath+"/" + "\"";
                    //browser.Url = "file:///" + uploadFileScreen.webLoadPath + "/index.html?folder_id=" + projectDetails[i].folder_id + "&project_id=" + projectDetails[i].project_id + "&folder_path=" + "\"" + uploadFileScreen.webFileLoadPath+"/" + "\"";

                

                    print(browser.Url);
                    print("t");
                    webView.SetActive(true);
                    StartCoroutine(DisableDummyObject());
                    //GetLayerDetails(projectDetails[i].folder_id);
                }
            }

            for (int i = 0; i < fileObjectList.Count; i++)
            {
                if(i==count)
                    fileObjectList[i].transform.GetComponent<Image>().color = selectColor;
                else
                    fileObjectList[i].transform.GetComponent<Image>().color = normalColor;

            }
        }

        public string RemoveSpecialCharacters(string oldString)
        {
            oldString = oldString.Replace('â', '\\');
            oldString = oldString.Replace('ê', '\'');
            oldString = oldString.Replace('ô', '\"');
            oldString = oldString.Replace('Â', '\\');
            oldString = oldString.Replace('Ê', '\'');
            oldString = oldString.Replace('Ô', '\"');
            return oldString;
        }

        public void GetLayerDetails(string folderId)
        {
            string jsonPath = uploadFileScreen.webFileLoadPath + "/" + folderId + "-layer.json";
            if (!File.Exists(jsonPath))
                return;
            
            string jsonStr = File.ReadAllText(jsonPath); // using System;
            //string jsonStr = File.ReadAllText(webFileLoadPath+ "index.p3.json"); // using System;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(jsonStr);
            var response_body = jsonObject["response_body"].Value<JObject>();
            print(response_body);
            var annotationList = response_body["layer_datas"].Value<JArray>();
            DataClass.Instance().layerDatas = annotationList.ToObject<List<DataClass.LayerDatas>>();
            for (int i = 0; i < DataClass.Instance().layerDatas.Count; i++)
            {
                DataClass.Instance().layerDatas[i].layerData.associated_pages = annotationList[i]["layer_data"]["associated_pages"].ToObject<List<DataClass.AnnotationPages>>();
                DataClass.Instance().layerDatas[i].layerData.annotations = annotationList[i]["layer_data"]["annotations"].ToObject<List<DataClass.Annotations>>();

            }

            print(DataClass.Instance().layerDatas[0].layerData.annotations.Count);
            //print(str);
        }

        IEnumerator DisableDummyObject()
        {
            yield return new WaitForSeconds(1f);
            dummyViewOject.SetActive(false);
        }
        public void OnFolderButtonClicked(int count,GameObject gameObject)
        {
            //print("folder button" + gameObject.name);
            if (!folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.activeSelf)
            {
                if (folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    CollapseButtonClicked(count, gameObject);
                }
                else
                {
                    ExpandButtonClikced(count, gameObject);
                }

                int childCount = 0;
                for (int i = 0; i < bothObjectList.Count; i++)
                {
                    if (bothObjectList[i].activeInHierarchy)
                    {
                        childCount += 1;
                    }
                }

                contentGridLayoutGroup.cellSize = new Vector2(contentGridLayoutGroup.cellSize.x, (childCount * 60));

                gridCellSizeList.Clear();

                for (int i = 0; i < folderObjectList.Count; i++)
                {
                    if (folderObjectList[i].activeInHierarchy)
                    {
                        float value = folderObjectList[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
                        float gridSizeX = (folderObjectList[i].transform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>().anchoredPosition.x - parentMoveValue) + value + addGridSizeX;
                        gridCellSizeList.Add(gridSizeX);
                        //print(gridSizeX);
                    }
                }

                for (int i = 0; i < fileObjectList.Count; i++)
                {
                    if (fileObjectList[i].activeInHierarchy)
                    {
                        float value = fileObjectList[i].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
                        float gridSizeX = (fileObjectList[i].transform.GetChild(0).transform.transform.GetComponent<RectTransform>().anchoredPosition.x - parentMoveValue) + value + addGridSizeX;
                        gridCellSizeList.Add(gridSizeX);
                        //print(gridSizeX);
                    }
                }

                //print(Mathf.Max(grildCellSizeList.ToArray()));
                if (Mathf.Max(gridCellSizeList.ToArray()) > defaultGridSizeX)
                {
                    contentGridLayoutGroup.cellSize = new Vector2(Mathf.Max(gridCellSizeList.ToArray()), contentGridLayoutGroup.cellSize.y);
                }
                else
                {
                    contentGridLayoutGroup.cellSize = new Vector2(defaultGridSizeX, contentGridLayoutGroup.cellSize.y);
                }
                

                for (int j = 0; j < folderObjectList.Count; j++)
                {
                    folderObjectList[j].transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                    folderObjectList[j].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;

                }
                for (int j = 0; j < fileObjectList.Count; j++)
                {
                    fileObjectList[j].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(contentGridLayoutGroup.cellSize.x, 1);
                    fileObjectList[j].transform.GetComponent<LayoutElement>().preferredWidth = contentGridLayoutGroup.cellSize.x;
                }

                //print(childCount);
            }
        }
        
        public void ExpandButtonClikced(int count,GameObject gameObject)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }

            folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
            folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
        }

        public void CollapseButtonClicked(int count, GameObject gameObject)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
           
            folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
            folderObjectList[count].transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
        }

        public void AnnotationClickedFromWeb()
        {
            print(browser.annotationClicked);
            if (browser.annotationClicked)
            {
                webView.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWebViewHeight - annotationScreenHeight);
                annotationMediaPreviewScreen.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWebViewHeight - annotationScreenHeight);
                annotationPhotosScreen.SetActive(true);
                GetAnnotationMedia(browser.annotationId, browser.layerId);
            }
            else
            {
                webView.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWebViewHeight);
                annotationPhotosScreen.SetActive(false);
            }
        }

        public void GetAnnotationMedia(string annotaionId, string layerId)
        {
            annotationMediaList.Clear();
            for (int i = 0; i < annotationMediaContent.childCount; i++)
            {
                Destroy(annotationMediaContent.GetChild(i).gameObject);
            }

            for (int i = 0; i < DataClass.Instance().layerDatas.Count; i++)
            {
                if (DataClass.Instance().layerDatas[i].layer_id == layerId)
                {
                    for (int j = 0; j < DataClass.Instance().layerDatas[i].layerData.annotations.Count; j++)
                    {
                        if (DataClass.Instance().layerDatas[i].layerData.annotations[j].annotation_id == annotaionId)
                        {
                            if (DataClass.Instance().layerDatas[i].layerData.annotations[j].annotation_media.Count > 1)
                            {
                                noMediaFound.SetActive(false);
                                for (int k = 0; k < DataClass.Instance().layerDatas[i].layerData.annotations[j].annotation_media.Count; k++)
                                {
                                    if (DataClass.Instance().layerDatas[i].layerData.annotations[j].annotation_media[k].is_removed == false)
                                    {
                                        GameObject media = Instantiate(annotationMediaPrefab, annotationMediaContent);
                                        annotationMediaList.Add(media);
                                        
                                        string path = uploadFileScreen.webFileLoadPath + "/" + DataClass.Instance().layerDatas[i].layerData.annotations[j].annotation_media[k].annotation_media_id;

                                        string file_path = "";
                                        if (File.Exists(path + ".png"))
                                            file_path = path + ".png";
                                        else if (File.Exists(path + ".jpg"))
                                            file_path = path + ".jpg";
                                        else if (File.Exists(path + ".heic"))
                                            file_path = path + ".heic";
                                        print(file_path);
                                        annotationMediaList[annotationMediaList.Count - 1].transform.GetComponent<Image>().sprite = GetSpriteUsingLocalPath(file_path);
                                        print(annotationMediaList.Count - 1);
                                        int count = annotationMediaList.Count - 1;
                                        annotationMediaList[annotationMediaList.Count - 1].transform.GetComponent<Button>().onClick.AddListener(() => OnAnnotationMediaButtonCliked(count));
                                    }
                                }
                            }
                            else
                            {
                                noMediaFound.SetActive(true);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void LoadDummyImages()
        {
     
            GameObject media = Instantiate(annotationMediaPrefab, annotationMediaContent);
            annotationMediaList.Add(media);
            annotationMediaList[annotationMediaList.Count-1].transform.GetComponent<Button>().onClick.AddListener(() => OnAnnotationMediaButtonCliked(annotationMediaList.Count - 1));
            string path = "C:/Users/sreer/Downloads/IMG_1907";
            string file_path = "";
            if (File.Exists(path + ".png"))
                file_path = path + ".png";
            else if (File.Exists(path + ".jpg"))
                file_path = path + ".jpg";
            else if (File.Exists(path + ".heic"))
                file_path = path + ".heic";


            print(file_path);
            annotationMediaList[annotationMediaList.Count - 1].transform.GetComponent<Image>().sprite = GetSpriteUsingLocalPath(file_path);
            
        }

        public Sprite GetSpriteUsingLocalPath(string path)
        {
            //print(path);

            Texture2D texture2D = null;
            byte[] imageData;
            Sprite tempSprite = null;
            imageData = File.ReadAllBytes(path);
            texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(imageData);
            if (texture2D.LoadImage(imageData))
            {

                tempSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                return tempSprite;
            }
            return tempSprite;
        }

        public void OnAnnotationMediaButtonCliked(int count)
        {
            print("media clicked" + count);
            print(annotationMediaList[count].transform.GetComponent<Image>().sprite.texture.width);
            print(annotationMediaList[count].transform.GetComponent<Image>().sprite.texture.height);

            annotationMediaPreviewScreen.SetActive(true);

            float defaultWidth = annotationMediaPreviewScreen.transform.GetComponent<RectTransform>().rect.width;
            float defaultHeight = annotationMediaPreviewScreen.transform.GetComponent<RectTransform>().rect.height;
            annotationMediaPreviewScreen.transform.GetChild(0).transform.GetComponent<Image>().sprite = annotationMediaList[count].transform.GetComponent<Image>().sprite;
            annotationMediaPreviewScreen.transform.GetChild(0).transform.GetComponent<Image>().SetNativeSize();
            float originalWidth = annotationMediaPreviewScreen.transform.GetChild(0).transform.GetComponent<RectTransform>().rect.width;
            float originalHeight = annotationMediaPreviewScreen.transform.GetChild(0).transform.GetComponent<RectTransform>().rect.height;
            float widthFinal=0;
            float heightFinal=0;
            if (originalWidth > defaultWidth)
            {
                widthFinal = originalWidth - (originalWidth - defaultWidth);
                float percentage = (widthFinal / originalWidth) * 100;

                heightFinal = (percentage / 100) * originalHeight;

                if (heightFinal > defaultHeight)
                {
                    float heightFinal1 = heightFinal - (heightFinal - defaultHeight);
                    float percentage1 = (heightFinal1 / heightFinal) * 100;
                    heightFinal = heightFinal - (heightFinal - defaultHeight);

                    widthFinal = (percentage1 / 100) * widthFinal;
                }

            }
            else if (originalHeight > defaultHeight)
            {
                heightFinal = originalHeight - (originalHeight - defaultHeight);
                float percentage = (heightFinal / originalHeight) * 100;

                widthFinal = (percentage / 100) * originalWidth;

                if (widthFinal > defaultWidth)
                {
                    float widthFinal1 = widthFinal - (widthFinal - defaultWidth);
                    float percentage1 = (widthFinal1 / widthFinal) * 100;
                    widthFinal = widthFinal - (widthFinal - defaultWidth);

                    heightFinal = (percentage1 / 100) * heightFinal;
                }

            }
            annotationMediaPreviewScreen.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(widthFinal, heightFinal);



        }

        #endregion

    }
}
