using BaseComponent;

namespace DataBasePlugin
{
    [iComponent("그룹명/메뉴이름", "Resources\\database.png")]
    public partial class MsSql : BasePanel
    {
        #region Input/Output
        NodePort inputPort;
        NodePort outputPort;
        #endregion

        public MsSql()
        {
            InitializeComponent();

            this.Title = "MSSQL";
            this.TitleBackColor = ColorTemplate.DarkPastelBlue;
            this.IsResize = true;

            inputPort = AddInputPort("In", PortDataType.String, value => { TxFunction(inputPort, value); }, PortDirection.LeftInput, allowMultipleConnections: true, showLed: true);
            outputPort = AddOutputPort("Out", PortDataType.String, PortDirection.RightOutput, allowMultipleConnections: true);
        }

        private void TxFunction(NodePort sender, object value)
        {
            this.TransferData<string>(outputPort, (string)value);
        }
    }
}
