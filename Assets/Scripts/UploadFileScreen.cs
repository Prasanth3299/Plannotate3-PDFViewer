using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;
using System;
using UnityEngine.Networking;
using UnityEngine.Video;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.SharpZipLib.Utils;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Threading;

namespace RevolutionGames
{

    public class UploadFileScreen : MonoBehaviour
    {
        #region Variables

        public GameObject uploadScreen;
        public GameObject installWindowMain;
        public GameObject smallInstallWindow;
        public GameObject bigInstallWindow;
        public GameObject documentViewScreen;
        public GameObject loadingScreen;
        public GameObject errorText;
        public Text loadingText;
        public GameObject highFileSize;
        public Text highFileSizeText;
        public GameObject sliderObject;
        public Slider loaderSlider;
        public Text percentageText;
        public GameObject unZipLoader;
        public GameObject initializeText;
        public GameObject messageText;
        public Slider smallInstallWindowSlider;
        public Slider bigInstallWindowSlider;
        public Text smallInstallPercentageSmall;
        public Text bigInstallPercentageSmall;

        private string versionNo = "1.2";
        private string m_currentPath = string.Empty;
        private string json_file_Path = string.Empty;
        public string file_directory_Path = string.Empty;
        private string _path = string.Empty;
        private string main_dirctory_path = "";
        string[] path;
        string writePath = "";
        string dataPath = "";
        [HideInInspector]
        public string webDataPath = "";

        [HideInInspector]
        public  string webFileLoadPath = "";
        [HideInInspector]
        public string webLoadPath = "";

        private bool p3Document=false;
        private int loaderValue = 0;
        private bool loadTextFlag = false;
        private bool highFileFlag = false;
        string zipeFileSize;
        private bool installStart;

        #endregion

        #region Built in Methods

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            Screen.fullScreen = false;
            errorText.SetActive(false);
            unZipLoader.SetActive(false);
            dataPath = Application.dataPath;
            CreateFolders();
            if (!Directory.Exists(file_directory_Path + "dist") || PlayerPrefs.GetInt("dist"+versionNo, 0) == 0)
            {
                Screen.SetResolution(500, 200, FullScreenMode.Windowed);
                Screen.fullScreen = false;
                //unZipLoader.SetActive(true);
                //initializeText.SetActive(true);
                //messageText.SetActive(true);
                installStart = true;
                smallInstallWindowSlider.value = 0;
                uploadScreen.SetActive(false);
                installWindowMain.SetActive(true);
                smallInstallWindow.SetActive(true);
                DoSomethingOffMainThreadUnZipDistFile();
            }
            else
            {
                installWindowMain.SetActive(false);
                smallInstallWindow.SetActive(false);
                uploadScreen.SetActive(true);
            }
        }

        private void OnEnable()
        {
            initializeText.SetActive(false);
            messageText.SetActive(false);
            errorText.SetActive(false);
            highFileSize.SetActive(false);
            sliderObject.SetActive(false);
        }
        private void Update()
        {
            if (loadTextFlag)
                UpdateLoaderText();
            if (highFileFlag)
                EnbleHighFileSize();
            if (installStart)
                InstallSliderUpdated();
        }
        #endregion

        #region Custom Methods

        public string MainDirectoryPath
        {
            get => main_dirctory_path;
        }


        public void InstallSliderUpdated()
        {
            if (smallInstallWindowSlider.value < 0.8f)
            {
                smallInstallWindowSlider.value += Time.deltaTime * 0.1f;
                bigInstallWindowSlider.value += Time.deltaTime * 0.1f;
            }
            else if (smallInstallWindowSlider.value < 0.9f)
            {
                smallInstallWindowSlider.value += Time.deltaTime * 0.01f;
                bigInstallWindowSlider.value += Time.deltaTime * 0.01f;
            }
            double value = Math.Round(((smallInstallWindowSlider.value / 1) * 100));
            smallInstallPercentageSmall.text = value.ToString() + "%";
            bigInstallPercentageSmall.text = value.ToString() + "%";

           if (Screen.width > 1000 && Screen.height > 1000)
            {
                bigInstallWindow.SetActive(true);
                smallInstallWindow.SetActive(false);
            }
            else
            {
                smallInstallWindow.SetActive(true);
                bigInstallWindow.SetActive(false);
            }
        }

        public void CreateFolders()
        {
            m_currentPath = Application.persistentDataPath;
            file_directory_Path = m_currentPath + "/Plannotate/";
            webDataPath = m_currentPath + "/WebData/";

            webLoadPath = webDataPath + "dist.zip";

            if (!Directory.Exists(file_directory_Path))
            {
                //if it doesn't, create it
                Directory.CreateDirectory(file_directory_Path);
            }
            if (!Directory.Exists(webDataPath))
            {
                //if it doesn't, create it
                Directory.CreateDirectory(webDataPath);
            }


        }

        IEnumerator RunOffMainThreadUnZipDistFile(Action toRun, Action callback)
        {
            bool done = false;
            new Thread(() => {
                toRun();
                done = true;
            }).Start();
            while (!done)
                yield return null;
            callback();
        }

        //This is the method you call to start it
        void DoSomethingOffMainThreadUnZipDistFile()
        {
            StartCoroutine(RunOffMainThreadUnZipDistFile(ToRunUnZipDistFile, OnFinishedUnZipDistFile));
        }

        //This is the method that does the work
        void ToRunUnZipDistFile()
        {
            //Do something slow here
            //print("start");
            UnZipWebdistFile();

        }

        //This is the method that's called when finished
        void OnFinishedUnZipDistFile()
        {
            //off main thread code finished, back on main thread now
            //print("end");
            //loadingScreen.SetActive(true);
            PlayerPrefs.SetInt("dist" + versionNo, 1);
            //initializeText.SetActive(false);
            //messageText.SetActive(false);
            //unZipLoader.SetActive(false);
            smallInstallWindowSlider.value = 1;
            bigInstallWindowSlider.value = 1;
            smallInstallPercentageSmall.text= 100 + "%";
            bigInstallPercentageSmall.text= 100 + "%";
            installStart = false;
            installWindowMain.SetActive(false);
            smallInstallWindow.SetActive(false);
            uploadScreen.SetActive(true);
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
            Screen.fullScreen = false;
        }

        public void UnZipWebdistFile()
        {
            try
            {
                if (Directory.Exists(file_directory_Path))
                {
                    DirectoryInfo directory = new DirectoryInfo(file_directory_Path);

                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    foreach (DirectoryInfo dir in directory.GetDirectories())
                    {
                        print(dir.Name);
                        dir.Delete(true);
                    }
                }

                if (File.Exists(file_directory_Path + "dist.zip"))
                    File.Delete(file_directory_Path + "dist.zip");

                File.Copy(dataPath + "/StreamingAssets/Plannotate/dist.zip", file_directory_Path + "dist.zip");
                System.IO.Compression.ZipFile.ExtractToDirectory(file_directory_Path + "dist.zip", file_directory_Path);
            }
            catch
            {
                
            }
            
        }


        static void CopyFolder(string path, string target)
        {
            // Create target directory
            Directory.CreateDirectory(target);

            // Copy all files
            foreach (string file in Directory.GetFiles(path))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)));

            // Recursively copy all subdirectories
            foreach (string directory in Directory.GetDirectories(path))
                CopyFolder(directory, Path.Combine(target, Path.GetFileName(directory)));
        }


        public void OnUploadFileButtonClicked()
        {
            p3Document = false;
            errorText.SetActive(false);
            loadTextFlag = false;
            highFileFlag = false;
            loaderValue = 0;
            loaderSlider.value = 0;
            
            //print("up");
            StartCoroutine(OpenFileBrowser());
            //OpenFileBrowser();

        }
        IEnumerator RunOffMainThread(Action toRun, Action callback)
        {
            bool done = false;
            new Thread(() => {
                toRun();
                done = true;
            }).Start();
            while (!done)
                yield return null;
            callback();
        }

        //This is the method you call to start it
        void DoSomethingOffMainThread()
        {
            StartCoroutine(RunOffMainThread(ToRun, OnFinished));
        }

        //This is the method that does the work
        void ToRun()
        {
            //Do something slow here
            //print("start");
            StartExtrackFile(path, writePath);

        }

        //This is the method that's called when finished
        void OnFinished()
        {
            //off main thread code finished, back on main thread now
            //print("end");
            //loadingScreen.SetActive(true);
            if (p3Document)
                FinalMoveDocumentScreen();
            else
                NotSelectP3File();



        }
        public IEnumerator OpenFileBrowser()
        {
            yield return new WaitForSeconds(0);
            var extensions = new[] { new ExtensionFilter("Files", "zip") };
            StandaloneFileBrowser.OpenFilePanelAsync("Select images", "", extensions, false, (string[] paths) => { WriteResult(paths, file_directory_Path); });
        }

        public void WriteResult(string[] paths, string write_Path)
        {
            if (paths.Length == 0 || paths[0] == "")
                return;

            
            path = paths;
            writePath = write_Path;
            loadingText.text = "Preparing Zip file...";
            loadTextFlag = true;
            StartCoroutine(ChangeLoadingText());
            loadingScreen.SetActive(true);
            print("copied");
            DoSomethingOffMainThread();
            /* loadingScreen.SetActive(true);
            StartExtrackFile(paths, write_Path);*/
        }

        public IEnumerator ChangeLoadingText()
        {
            yield return new WaitForSeconds(10);
            loadingText.text = "Unpacking the files......";
            yield return new WaitForSeconds(20);
            loadingText.text = "Finalising the files........";
        }

        public void UpdateLoaderText()
        {
            sliderObject.SetActive(true);
            if (loaderValue == 0)
            {
                loadingText.text = "Preparing Zip file...";
                loaderSlider.value = 0.1f;
            }

            else if (loaderValue == 1)
            {
                loadingText.text = "Copying selected zip file...";
                loaderSlider.value = 0.2f;
            }
            else if (loaderValue == 2)
            {
                if(loadingText.text != "Unpacking Zip file...")
                 loadingText.text = "Unpacking Zip file...";
                if(loaderSlider.value<0.8f)
                  loaderSlider.value += Time.deltaTime * 0.005f;
            }
            else if (loaderValue == 3)
            {
                print("d");
                loaderSlider.value = 0.85f;
                loadingText.text = "Validating zip file...";
            }
            else if (loaderValue == 4)
            {
                if (loaderSlider.value < 0.8f)
                    loaderSlider.value = 0.8f;

                loadingText.text = "Finalising zip file...";
                if (loaderSlider.value < 1f)
                    loaderSlider.value += Time.deltaTime * 0.005f;
                else
                    loaderSlider.value = 1f;
            }

            double value = Math.Round(((loaderSlider.value / 1) * 100));
            percentageText.text = value.ToString() + "%";

        }

        public void EnbleHighFileSize()
        {
            highFileFlag = false;
            highFileSizeText.text = "This report has large data size (" + zipeFileSize + "), so it will take longer time to process. \n Please don't close the window. Once operation is over, you will be redirected to listing page.";
            highFileSize.SetActive(true);
        }

        public void StartExtrackFile(string[] paths, string write_Path)
        {
            //yield return new WaitForSeconds(0);
            print("delete old start");
            DeleteOldFiles();
            print("delete old end");
            _path = "";
            foreach (var p in paths)
            {
                _path += p + "\n";
                if (!File.Exists(write_Path + Path.GetFileName(p)))
                {
                    print("file copy start");
                    loaderValue = 1;
                    //loadingText.text = "Copying selected zip file...";
                    var fileInfo = new System.IO.FileInfo(p);
                    float byteCount = fileInfo.Length;
                    float kbCount = byteCount / 1024;
                    float mbCount = kbCount / 1024;
                    mbCount = float.Parse(Math.Round(mbCount, 1).ToString());
                    float gbCount = mbCount / 1024;
                    gbCount = float.Parse(Math.Round(gbCount, 2).ToString());
                    zipeFileSize = mbCount.ToString()+" MB";
                    if (mbCount > 200)
                        highFileFlag = true;
                    if(gbCount>1)
                        zipeFileSize = gbCount.ToString() + " GB";


                    print(mbCount);
                    File.Copy(p, write_Path + Path.GetFileName(p));
                    print("file copy end");
                    print("main extrat start");
                    loaderValue = 2;
                    //loadingText.text = "Unpacking Zip file...";
                    ExtractFile(p, file_directory_Path);
                }
            }

            MoveDocumentViewScreen();
        }

        public void MoveDocumentViewScreen()
        {
            print("main extrat end");
            loaderValue = 3;
            if (CheckPlannotateFileAvailable())
            {
                /*errorText.SetActive(false);
                loadingScreen.SetActive(false);
                LoadProjectDetailsJson(); 
                this.gameObject.SetActive(false);
                documentViewScreen.SetActive(true);*/

                print(webFileLoadPath);
                loaderValue = 4;
                //CopyFolder(webDataPath+"/dist", webFileLoadPath+"/dist");
                //ExtractWebZipFile(webFileLoadPath + "/dist.zip", "", webFileLoadPath + "/");
                //UnZipWebFile(webFileLoadPath + "/dist.zip", webFileLoadPath + "/");
                
                p3Document = true;
                //Directory.Move(webFileLoadPath + "/dist1/dist", webFileLoadPath+"/dist");
                //FinalMoveDocumentScreen();
            }
            else
            {
                p3Document = false;
               
                //StartCoroutine(OnCloseButtonClicked());
            }
           
        }


        public void NotSelectP3File()
        {
            loadTextFlag = false;
            loadingScreen.SetActive(false);
            errorText.SetActive(true);
        }

        public void FinalMoveDocumentScreen()
        {
            print("comp");
            StartCoroutine(FinalSwitch());
        }

        IEnumerator FinalSwitch()
        {
            loaderSlider.value = 1;
            yield return new WaitForSeconds(0);
            loadTextFlag = false;
            errorText.SetActive(false);
            loadingScreen.SetActive(false);
            LoadProjectDetailsJson();
            this.gameObject.SetActive(false);
            documentViewScreen.SetActive(true);
        }


        public void DeleteOldFiles()
        {

            if (Directory.Exists(file_directory_Path))
            {

                string path = file_directory_Path;

                DirectoryInfo directory = new DirectoryInfo(path);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    print(dir.Name);
                    if(dir.Name!="dist")
                       dir.Delete(true);
                }

            }

           /* if (Directory.Exists(webFileLoadPath))
            {

                string path = webFileLoadPath;

                DirectoryInfo directory = new DirectoryInfo(path);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    //dir.Delete(true);
                }

            }*/

        }

        public void ExtractFile(string zipPath, string extractPath)
        {
            //(zipPath, "f1&trIk!2*$?6pri0lv5vi0rec_+br1nesONI*1_E#$d*@at8!nUc_id@i6rUqud", extractPath);
            UnZipMainFile(zipPath, extractPath);
            //ExtractZipFile(zipPath, "", extractPath);
            //ExtractZipFile(zipPath, "", extractPath);
            //System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            print("ends");
        }

        void UnZipMainFile(string zipPath1, string extractPath)
        {
            try
            {
                print(zipPath1);
                string zipFile = zipPath1;
                string targetDirectory = extractPath;
                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
                Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(zipFile);
                zip.Password = "f1&trIk!2*$?6pri0lv5vi0rec_+br1nesONI*1_E#$d*@at8!nUc_id@i6rUqud";
                zip.ExtractAll(targetDirectory, Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite);
            }
            catch
            {
                print("catch");
            }
            

            //Console.WriteLine("Zip file has been successfully extracted.");
            //Console.Read();
        }
        void UnZipWebFile(string zipPath, string extractPath)
        {
            try
            {
                print("web");
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch
            {
                print("catch");
            }
        }

        public void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
        {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    print(zipEntry.Name);
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[8096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    entryFileName = entryFileName.Replace(":", "_");
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch
            {
                print("catch");
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        public void ExtractWebZipFile(string archiveFilenameIn, string password, string outFolder)
        {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch
            {
                
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }


        public bool CheckPlannotateFileAvailable()
        {
            string path = file_directory_Path ;
            print(path);
            loaderValue = 3;
            //loadingText.text = "Validating zip file...";
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    print(dir.Name);
                    DirectoryInfo directory1 = new DirectoryInfo(path+"/"+ dir.Name);
                    print(directory1.FullName);
                    if (File.Exists(directory1.FullName + "/index.p3.json"))
                    {
                        main_dirctory_path = directory1.FullName;
                        json_file_Path = directory1.FullName + "/index.p3.json";
                        webFileLoadPath = directory1.FullName;
                        p3Document = true;
                    }
                    else
                    {
                        p3Document = false;
                    }
                    /*foreach (FileInfo file in directory1.GetFiles())
                    {
                        //print(file.FullName);
                        //print(file.Name);
                        if (file.Name == "index.p3.json")
                        {
                            main_dirctory_path = directory1.FullName;
                            json_file_Path = file.FullName;
                            webFileLoadPath = directory1.FullName;
                            p3Document = true;
                            //return (true);
                        }
                        else if (p3Document == false)
                        {
                            p3Document = false;
                        }
                        //File.Move(file.FullName, webFileLoadPath + file.Name);
                    }*/

                    if (p3Document == true)
                        return (p3Document);

/*                    foreach (DirectoryInfo dir1 in directory1.GetDirectories())
                    {
                        DirectoryInfo directory2 = new DirectoryInfo(path + "/" + dir.Name+"/"+dir1.Name);

                        foreach (FileInfo file in directory2.GetFiles())
                        {
                            //print(file);
                            if (file.Name == "index.p3.json")
                            {
                                main_dirctory_path = directory2.FullName;
                                json_file_Path = file.FullName;
                                webFileLoadPath = directory2.FullName;
                                p3Document = true;
                                //return (true);
                            }
                            else if (p3Document == false)
                            {
                                p3Document = false;
                            }
                            //File.Move(file.FullName, webFileLoadPath + file.Name);
                        }
                        return (p3Document);
                    }*/
                }

                return (false);
            }

            return (false);
        }

        public IEnumerator OnCloseButtonClicked()
        {
            yield return new WaitForSeconds(5);
            errorText.SetActive(false);
        }

        public void LoadProjectDetailsJson()
        {
            string jsonPath = json_file_Path;
            string jsonStr = File.ReadAllText(jsonPath); // using System;
            //string jsonStr = File.ReadAllText(webFileLoadPath+ "index.p3.json"); // using System;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(jsonStr);
            var str = jsonObject["response_body"].Value<JArray>();
            DataClass.Instance().projectDetails = str.ToObject<List<DataClass.ProjectDetails>>();

            print(DataClass.Instance().projectDetails.Count);
            //print(str);
        }

        #endregion

    }
}
