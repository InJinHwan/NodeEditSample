using BaseComponent;
using System.Drawing;

namespace BasePlugins
{
    [iComponent("SAMPLE/Sample", "Resources\\cctv.png")]
    public partial class SamplePlugin : BasePanel
    {
        #region Input/output
        NodePort input1;
        NodePort input2;
        NodePort input3;
        NodePort input4;
        NodePort output1;
        NodePort output2;
        NodePort output3;
        NodePort output4;
        #endregion

        public SamplePlugin()
        {
            InitializeComponent();

            this.Title = "Sample";
            this.TitleBackColor = Color.FromArgb(200, 255, 255, 0);
            this.IsResize = true;

            input1 = AddInputPort("In1", PortDataType.Object, value => { TxCommand(input1, value); }, allowMultipleConnections: false, direction: PortDirection.LeftInput, tag: "IN1");//좌측 입력
            input2 = AddInputPort("In2", PortDataType.Object, value => { TxCommand(input2, value); }, allowMultipleConnections: false, direction: PortDirection.RightInput, tag: "IN2");//우측 입력
            input3 = AddInputPort("In3", PortDataType.Object, value => { TxCommand(input3, value); }, allowMultipleConnections: false, direction: PortDirection.TopInput, tag: "IN3");//상단 입력
            input4 = AddInputPort("In4", PortDataType.Object, value => { TxCommand(input4, value); }, allowMultipleConnections: false, direction: PortDirection.BottomInput, tag: "IN4");//하단단 입력
            output1 = AddOutputPort("Out1", PortDataType.Object, direction: PortDirection.LeftOutput); //좌측 출력
            output2 = AddOutputPort("Out2", PortDataType.Object, direction: PortDirection.RightOutput); //우측 출력            
            output3 = AddOutputPort("Out3", PortDataType.Object, direction: PortDirection.TopOutput); //상단 출력
            output4 = AddOutputPort("Out4", PortDataType.Object, direction: PortDirection.BottomOutput); //하단 출력
        }

        #region unser define function
        private void TxCommand(NodePort sender, object value)
        {
            switch (sender.Tag)
            {
                case "IN1": this.TransferData<string>(output1, (string)value); break;
                case "IN2": this.TransferData<string>(output2, (string)value); break;
                case "IN3": this.TransferData<string>(output3, (string)value); break;
                case "IN4": this.TransferData<string>(output4, (string)value); break;
            }


            this.Invalidate();
        }
        #endregion
    }
}
