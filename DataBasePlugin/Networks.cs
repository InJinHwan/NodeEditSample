using BaseComponent;
using System.ComponentModel;
using System.Drawing.Design;

namespace DataBasePlugin
{
    [iComponent("그룹명/메뉴이름", "Resources\\database.png")]
    public partial class Networks : BasePanel
    {
        #region input/output
        NodePort inputPort;
        NodePort outputPort;
        #endregion

        #region 속성
        private int _maxcount = 10;
        [Category("전용 설정")]
        [DisplayName("일반속성")]
        [Description("일반속성 예제")]
        public int MaxCount
        {
            get => _maxcount;
            set
            {
                _maxcount = value;
                Invalidate();
            }
        }

        private string filepath = string.Empty;
        [Category("전용 설정")]
        [DisplayName("파일선택")]
        [Description("파일선택 속성 예제.")]
        [Editor(typeof(FileSelectEditor), typeof(UITypeEditor))]
        public string ScriptPATH
        {
            get => filepath;
            set
            {
                filepath = value;
                Invalidate();
            }
        }
        #endregion

        #region private
        #endregion

        #region public
        #endregion

        #region 생성자
        public Networks()
        {
            InitializeComponent();

            this.Title = "Networks";
            this.TitleBackColor = ColorTemplate.DarkPastelBlue;
            this.IsResize = true;

            inputPort = AddInputPort("In", PortDataType.String, value => { TxFunction(inputPort, value); }, PortDirection.LeftInput, allowMultipleConnections: true, showLed: true);
            outputPort = AddOutputPort("Out", PortDataType.String, PortDirection.RightOutput, allowMultipleConnections: true);
        }
        #endregion

        #region Event
        private void TxFunction(NodePort sender, object value)
        {
            this.TransferData<string>(outputPort, (string)value);
        }
        #endregion

        #region private function
        #endregion

        #region public function
        #endregion
    }
}
