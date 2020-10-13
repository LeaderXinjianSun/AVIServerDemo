using AVIServerDemo.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion
        #region 方法绑定
        public DelegateCommand AppLoadedEventCommand { get; set; }
        public DelegateCommand<object> MenuActionCommand { get; set; }
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            #region 初始化参数
            Version = "20201013";
            MessageStr = "";
            #endregion
            AppLoadedEventCommand = new DelegateCommand(new Action(this.AppLoadedEventCommandExecute));
            MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
        }

        private void MenuActionCommandExecute(object obj)
        {
            
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
                    mysql.DisConnect();
                    AddMessage($"数据库连接成功{ ds.Tables["table0"].Rows[0][0]}");
                    StatusDataBase = true;
                }
                else
                {
                    AddMessage("数据库未连接");
                }
                mysql.DisConnect();
            }
            catch (Exception ex)
            {
                AddMessage($"数据库连接失败{ex.Message}");
                StatusDataBase = false;
            }
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
        #endregion
    }
}
