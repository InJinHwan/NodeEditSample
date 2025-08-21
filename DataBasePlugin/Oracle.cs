using BaseComponent;
using System;
using System.ComponentModel;

namespace DataBasePlugin
{
    [iComponent("Data Base/Oracle", "Resources\\database.png")]
    public partial class Oracle : BasePanel
    {
        #region Input/Output
        NodePort outputDB;
        #endregion

        #region 속성
        private string _serverip = string.Empty;
        [Category("접속 설정")]
        [DisplayName("서비 IP")]
        [Description("Database의 IP를 설정합니다.")]
        public string DBIP
        {
            get { return _serverip; }
            set { _serverip = value; Invalidate(); }
        }

        private string _servicename = string.Empty;
        [Category("접속 설정")]
        [DisplayName("SID or 서비스이름")]
        [Description("SID나 서비스 이름을 설정합니다.")]
        public string SERVICENAME
        {
            get { return _servicename; }
            set { this._servicename = value; }
        }

        private string _userid = string.Empty;
        [Category("접속 설정")]
        [DisplayName("계정 ID")]
        [Description("접속할 사용자의 ID를 설정합니다.")]
        public string USERID
        {
            get { return _userid; }
            set { this._userid = value; }
        }

        private string _password = string.Empty;
        [Category("접속 설정")]
        [DisplayName("계정 Password")]
        [Description("접속할 사용자의 Password를 설정합니다.")]
        [PasswordPropertyText(true)] // ⭐ 추가
        public string USERPASSWORD
        {
            get { return _password; }
            set { this._password = value; }
        }

        private string _serverport = "1521";
        [Category("접속 설정")]
        [DisplayName("포트")]
        [Description("Database의 접속 포트를 설정합니다.")]
        public string DBPORT
        {
            get { return _serverport; }
            set { this._serverport = value; }
        }
        #endregion

        #region private
        private OracleDbHelper odh = null;
        #endregion

        #region 생성자
        public Oracle()
        {
            InitializeComponent();

            this.Title = "Oracle";
            this.TitleBackColor = ColorTemplate.DarkPastelGray;
            this.IsResize = false;//크기조절 금지

            outputDB = AddOutputPort("Handle", PortDataType.DataAccess);
            outputDB.OnPortConnected += OutputDB_OnPortConnected;

            this.btnConnect.Click += BtnConnect_Click;
        }

        #endregion

        #region event 함수
        private void OutputDB_OnPortConnected(NodePort obj)
        {
            if (odh != null)
                this.TransferData<OracleDbHelper>(outputDB, odh);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                try
                {
                    string connStr = $"User Id={USERID};Password={USERPASSWORD};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={DBIP})(PORT={DBPORT}))(CONNECT_DATA=(SERVICE_NAME={SERVICENAME})))";
                    odh = new OracleDbHelper(connStr);
                    odh.Connect();
                    logConsole.WriteLine("연결 완료");

                    this.TransferData<OracleDbHelper>(outputDB, odh);

                    btnConnect.Icon = Properties.Resources.connect2;
                    btnConnect.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    logConsole.WriteLine($"<color:red>연결 실패</color> {ex.Message}");
                }
            }
            else
            {
                if (odh != null && odh.IsConnected)
                {
                    odh.Disconnect();  // 연결 강제 종료
                    logConsole.WriteLine("연결 종료");
                    btnConnect.Icon = Properties.Resources.disconnect;
                    btnConnect.Text = "Connect";
                }
            }
        }
        #endregion
    }
}