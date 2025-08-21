using BaseComponent;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace BasePlugins
{
    [iComponent("SAMPLE/Image", "Resources\\image.png")]
    public partial class ImagePlugin : BasePanel
    {
        #region Input/Output
        NodePort inputData;
        NodePort outputData;
        #endregion

        #region 속성
        private string _sourcePath = "";
        [Category("전용 설정")]
        [DisplayName("이미지 Path")]
        [Description("표시할 이미지를 설정합니다.")]
        [Editor(typeof(FileSelectEditor), typeof(UITypeEditor))]
        public string SOURCEPATH
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                try
                {
                    if (System.IO.File.Exists(_sourcePath))
                    {
                        _loadedImage = Image.FromFile(_sourcePath);
                    }
                    else
                    {
                        _loadedImage = null;
                    }
                }
                catch
                {
                    _loadedImage = null;
                }

                this.Invalidate(); // 다시 그리기 요청
            }
        }
        #endregion

        #region private
        private Image _loadedImage = null;
        #endregion

        #region 생성자
        public ImagePlugin()
        {
            InitializeComponent();

            this.Title = "Image";
            this.TitleBackColor = ColorTemplate.PastelBlueGreen;
            this.IsResize = true;

            inputData = AddInputPort("Image Path", PortDataType.String, value => { TxCommand(inputData, value); }, allowMultipleConnections: false);
            outputData = AddOutputPort("Out Image", PortDataType.String);
        }
        #endregion

        #region 사용자정의 함수
        private void TxCommand(NodePort sender, object value)
        {
            this.SOURCEPATH = (string)value;

            this.TransferData<string>(outputData, (string)value); //output 전송

            this.Invalidate();
        }
        #endregion

        #region override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_loadedImage != null && !_loadedImage.Size.IsEmpty)
            {

                e.Graphics.DrawImage(_loadedImage, this.GetClientRect(offTop: 32));
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, "이미지 없음", this.Font, this.GetClientRect(offTop: 32), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
        #endregion
    }
}
