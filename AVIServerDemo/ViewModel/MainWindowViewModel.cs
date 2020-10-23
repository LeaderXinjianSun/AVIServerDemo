using AVIServerDemo.Model;
using HalconDotNet;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewROI;

namespace AVIServerDemo.ViewModel
{
    class MainWindowViewModel : NotificationObject
    {
        #region 属性绑定
        private string version;

        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                this.RaisePropertyChanged("Version");
            }
        }
        private string messageStr;

        public string MessageStr
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
                this.RaisePropertyChanged("MessageStr");
            }
        }
        private string homePageVisibility;

        public string HomePageVisibility
        {
            get { return homePageVisibility; }
            set
            {
                homePageVisibility = value;
                this.RaisePropertyChanged("HomePageVisibility");
            }
        }
        private string parameterPageVisibility;

        public string ParameterPageVisibility
        {
            get { return parameterPageVisibility; }
            set
            {
                parameterPageVisibility = value;
                this.RaisePropertyChanged("ParameterPageVisibility");
            }
        }

        private bool statusDataBase;

        public bool StatusDataBase
        {
            get { return statusDataBase; }
            set
            {
                statusDataBase = value;
                this.RaisePropertyChanged("StatusDataBase");
            }
        }
        private string workPath;

        public string WorkPath
        {
            get { return workPath; }
            set
            {
                workPath = value;
                this.RaisePropertyChanged("WorkPath");
            }
        }
        private HImage cameraIamge;

        public HImage CameraIamge
        {
            get { return cameraIamge; }
            set
            {
                cameraIamge = value;
                this.RaisePropertyChanged("CameraIamge");
            }
        }
        private bool cameraRepaint;

        public bool CameraRepaint
        {
            get { return cameraRepaint; }
            set
            {
                cameraRepaint = value;
                this.RaisePropertyChanged("CameraRepaint");
            }
        }
        private ObservableCollection<ROI> cameraROIList;

        public ObservableCollection<ROI> CameraROIList
        {
            get { return cameraROIList; }
            set
            {
                cameraROIList = value;
                this.RaisePropertyChanged("CameraROIList");
            }
        }
        private HObject cameraAppendHObject;

        public HObject CameraAppendHObject
        {
            get { return cameraAppendHObject; }
            set
            {
                cameraAppendHObject = value;
                this.RaisePropertyChanged("CameraAppendHObject");
            }
        }
        private Tuple<string, object> cameraGCStyle;

        public Tuple<string, object> CameraGCStyle
        {
            get { return cameraGCStyle; }
            set
            {
                cameraGCStyle = value;
                this.RaisePropertyChanged("CameraGCStyle");
            }
        }


        #endregion
        #region 方法绑定
        public DelegateCommand AppLoadedEventCommand { get; set; }
        public DelegateCommand<object> MenuActionCommand { get; set; }
        public DelegateCommand FolderBrowserDialogCommand { get; set; }
        public DelegateCommand ParameterSaveCommand { get; set; }
        #endregion
        #region 变量
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            #region 初始化参数
            Version = "20201023";
            MessageStr = "";
            HomePageVisibility = "Visible";
            ParameterPageVisibility = "Collapsed";
            WorkPath = Inifile.INIGetStringValue(iniParameterPath, "System", "WorkPath", "D:\\");
            CameraROIList = new ObservableCollection<ROI>();
            #endregion
            AppLoadedEventCommand = new DelegateCommand(new Action(this.AppLoadedEventCommandExecute));
            MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
            FolderBrowserDialogCommand = new DelegateCommand(new Action(this.FolderBrowserDialogCommandExecute));
            ParameterSaveCommand = new DelegateCommand(new Action(this.ParameterSaveCommandExecute));
        }

        private void ParameterSaveCommandExecute()
        {
            Inifile.INIWriteValue(iniParameterPath, "System", "WorkPath", WorkPath);
            AddMessage("保存参数");
        }

        private void FolderBrowserDialogCommandExecute()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WorkPath = dialog.SelectedPath;
                }
            }
        }

        private void MenuActionCommandExecute(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    HomePageVisibility = "Visible";
                    ParameterPageVisibility = "Collapsed";
                    break;
                case "1":
                    HomePageVisibility = "Collapsed";
                    ParameterPageVisibility = "Visible";
                    break;
                default:
                    break;
            }
        }

        private void AppLoadedEventCommandExecute()
        {
            AddMessage("软件加载完成");
            try
            {
                Mysql mysql = new Mysql();
                if (mysql.Connect())
                {
                    string stm = $"SELECT NOW()";
                    DataSet ds = mysql.Select(stm);
                    AddMessage($"数据库连接成功{ ds.Tables["table0"].Rows[0][0]}");
                    StatusDataBase = true;
                }
                else
                {
                    AddMessage("数据库未连接");
                    StatusDataBase = false;
                }
                mysql.DisConnect();
            }
            catch (Exception ex)
            {
                AddMessage($"数据库连接失败{ex.Message}");
                StatusDataBase = false;
            }
            Run();
        }
        #endregion
        #region 自定义函数
        private void AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            MessageStr += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        private async void Run()
        {
            while (true)
            {
                try
                {
                    if (Directory.Exists(WorkPath))
                    {
                        DirectoryInfo folder = new DirectoryInfo(WorkPath);
                        FileInfo[] files = folder.GetFiles("*.bmp");
                        if (files.Length > 0 && !IsFileLocked(files[0]))
                        {
                            string[] vs = files[0].Name.Split(new string[] { "_"}, StringSplitOptions.RemoveEmptyEntries);
                            if (vs.Length == 2)
                            {
                                string[] vs1 = vs[1].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                if (!Directory.Exists(Path.Combine(WorkPath, vs1[0])))
                                {
                                    Directory.CreateDirectory(Path.Combine(WorkPath, vs1[0]));
                                }
                                File.Copy(files[0].FullName, Path.Combine(WorkPath, vs1[0], files[0].Name),true);
                                string str = files[0].FullName;
                                File.Delete(str);

                                HObject img;
                                HOperatorSet.ReadImage(out img, Path.Combine(WorkPath, vs1[0], files[0].Name));
                                CameraIamge = new HImage(img);

                                Mysql mysql = new Mysql();
                                if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                                {                                    
                                    for (int i = 0; i < 16; i++)
                                    {
                                        string stm = $"INSERT INTO aviproductdata (BoardID,PcsIndex,ResultItem1) VALUES ('{vs1[0]}',{i},'OK')";
                                        int result = mysql.executeQuery(stm);
                                        if (result < 1)
                                        {
                                            AddMessage("请注意，数据库插入失败");
                                        }
                                    }
                                    StatusDataBase = true;
                                }
                                else
                                {
                                    AddMessage("数据库未连接");
                                    StatusDataBase = false;
                                }
                                mysql.DisConnect();


                                AddMessage($"{str}文件处理完成");
                            }
                            else
                            {
                                string str = files[0].FullName;
                                File.Delete(str);
                                AddMessage($"{str}文件名非法，已删除");
                            }
                        }
                        else
                        {
                            await Task.Delay(500);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    AddMessage(ex.Message);
                }
                await Task.Delay(100);
            }
        }
        protected bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
        #endregion
    }
}
