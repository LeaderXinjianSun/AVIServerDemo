﻿using AVIServerDemo.Model;
using HalconDotNet;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        private string imageSavePath;

        public string ImageSavePath
        {
            get { return imageSavePath; }
            set
            {
                imageSavePath = value;
                this.RaisePropertyChanged("ImageSavePath");
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
        private string serverIP;

        public string ServerIP
        {
            get { return serverIP; }
            set
            {
                serverIP = value;
                this.RaisePropertyChanged("ServerIP");
            }
        }
        private int serverPort;

        public int ServerPort
        {
            get { return serverPort; }
            set
            {
                serverPort = value;
                this.RaisePropertyChanged("ServerPort");
            }
        }

        #endregion
        #region 方法绑定
        public DelegateCommand AppLoadedEventCommand { get; set; }
        public DelegateCommand<object> MenuActionCommand { get; set; }
        public DelegateCommand<object> FolderBrowserDialogCommand { get; set; }
        public DelegateCommand ParameterSaveCommand { get; set; }
        #endregion
        #region 变量
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        DXH.Net.DXHTCPServer tcpServer;
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            #region 初始化参数
            Version = "20201103";
            MessageStr = "";
            HomePageVisibility = "Visible";
            ParameterPageVisibility = "Collapsed";
            WorkPath = Inifile.INIGetStringValue(iniParameterPath, "System", "WorkPath", "D:\\");
            ImageSavePath = Inifile.INIGetStringValue(iniParameterPath, "System", "ImageSavePath", "D:\\");
            CameraROIList = new ObservableCollection<ROI>();

            ServerIP = Inifile.INIGetStringValue(iniParameterPath, "Server", "IP", "192.168.0.11");
            ServerPort = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "Server", "PORT", "3000"));
            #endregion
            AppLoadedEventCommand = new DelegateCommand(new Action(this.AppLoadedEventCommandExecute));
            MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
            FolderBrowserDialogCommand = new DelegateCommand<object>(new Action<object>(this.FolderBrowserDialogCommandExecute));
            ParameterSaveCommand = new DelegateCommand(new Action(this.ParameterSaveCommandExecute));


        }

        private void ParameterSaveCommandExecute()
        {
            Inifile.INIWriteValue(iniParameterPath, "System", "WorkPath", WorkPath);
            Inifile.INIWriteValue(iniParameterPath, "System", "ImageSavePath", ImageSavePath);
            Inifile.INIWriteValue(iniParameterPath, "Server", "IP", ServerIP);
            Inifile.INIWriteValue(iniParameterPath, "Server", "PORT", ServerPort.ToString());
            AddMessage("保存参数");
        }

        private void FolderBrowserDialogCommandExecute(object obj)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    switch (obj.ToString())
                    {
                        case "0":
                            WorkPath = dialog.SelectedPath;
                            break;
                        case "1":
                            ImageSavePath = dialog.SelectedPath;
                            break;
                        default:
                            break;
                    }
                    
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
            tcpServer = new DXH.Net.DXHTCPServer();
            tcpServer.LocalIPAddress = Inifile.INIGetStringValue(iniParameterPath, "Server", "IP", "127.0.0.1");
            tcpServer.LocalIPPort = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "Server", "PORT", "11001"));
            tcpServer.SocketListChanged += TcpServer_SocketListChanged;
            tcpServer.ConnectStateChanged += TcpServer_ConnectStateChanged;
            tcpServer.Received += TcpServer_Received;
            tcpServer.StartTCPListen();
            Run();
        }

        private void TcpServer_Received(object sender, string e)
        {
            string restr = e.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
            AddMessage("接收:" + restr);
            Task.Run(()=> {
                try
                {
                    if (Directory.Exists(WorkPath))
                    {
                        DirectoryInfo folder = new DirectoryInfo(WorkPath);
                        int cout = 0;
                        FileInfo[] files = folder.GetFiles(restr);
                        while (!(files.Length > 0 && !IsFileLocked(files[0])) && cout < 10)
                        {
                            files = folder.GetFiles(restr);
                            cout++;
                            System.Threading.Thread.Sleep(200);
                        }
                        if (cout < 10)
                        {
                            string[] vs = files[0].Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            if (vs.Length == 3)
                            {
                                HObject img;
                                HOperatorSet.ReadImage(out img, files[0].FullName);
                                CameraIamge = new HImage(img);

                                
                                if (!Directory.Exists(Path.Combine(ImageSavePath, vs[1])))
                                {
                                    Directory.CreateDirectory(Path.Combine(ImageSavePath, vs[1]));
                                }
                                string[] vs1 = vs[2].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                if (!Directory.Exists(Path.Combine(ImageSavePath, vs[1], vs1[0])))
                                {
                                    Directory.CreateDirectory(Path.Combine(ImageSavePath, vs[1], vs1[0]));
                                }
                                File.Copy(files[0].FullName, Path.Combine(ImageSavePath, vs[1], vs1[0], files[0].Name), true);
                                string str = files[0].FullName;
                                File.Delete(str);

                                Mysql mysql = new Mysql();
                                if (mysql.Connect())
                                {
                                    string stm = "";
                                    switch (vs[1])
                                    {
                                        case "M1":
                                            stm = $"INSERT INTO aviproductdata (BoardID,PcsIndex,ResultItem1) VALUES ('{vs[0]}',{vs1[0]},'OK')";
                                            break;
                                        case "M2":
                                            stm = $"UPDATE aviproductdata SET ResultItem2 = 'OK' WHERE BoardID = '{vs[0]}' AND PcsIndex = {vs1[0]}";
                                            break;
                                        case "M3":
                                            stm = $"UPDATE aviproductdata SET ResultItem3 = 'OK' WHERE BoardID = '{vs[0]}' AND PcsIndex = {vs1[0]}";
                                            break;
                                        case "M4":
                                            stm = $"UPDATE aviproductdata SET ResultItem4 = 'OK' WHERE BoardID = '{vs[0]}' AND PcsIndex = {vs1[0]}";
                                            break;
                                        default:
                                            break;
                                    }
                                    
                                    int result = mysql.executeQuery(stm);
                                    if (result < 1)
                                    {
                                        AddMessage("请注意，数据库插入失败");
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
                            AddMessage($"获取文件{restr}超时");
                        }
                    }

                }
                catch (Exception ex)
                {
                    AddMessage(ex.Message);
                }
            });
        }

        private void TcpServer_ConnectStateChanged(object sender, string e)
        {
            AddMessage("TCP Server " + e);
        }

        private void TcpServer_SocketListChanged(object sender, bool e)
        {
            AddMessage($"客户端:{((IPEndPoint)(((Socket)sender).RemoteEndPoint)).Address}:{((IPEndPoint)(((Socket)sender).RemoteEndPoint)).Port} {(e ? "连接" : "断开")}");
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
