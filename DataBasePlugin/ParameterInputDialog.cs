using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataBasePlugin
{
    public partial class ParameterInputDialog : Form
    {
        #region private
        private readonly Dictionary<string, TextBox> inputBoxes = new Dictionary<string, TextBox>();
        private readonly Button okButton;

        #endregion

        #region public
        public Dictionary<string, object> ParameterValues { get; } = new Dictionary<string, object>();
        #endregion

        public ParameterInputDialog(IEnumerable<string> parameterNames)
        {
            InitializeComponent();
            Text = "파라미터 입력";
            Width = 400;
            Height = 90 + parameterNames.Count() * 30;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 10;
            foreach (string name in parameterNames)
            {
                Label label = new Label { Text = name, Left = 10, Top = y + 3, Width = 100 };
                TextBox box = new TextBox { Left = 120, Top = y, Width = 240 };

                inputBoxes[name] = box;
                Controls.Add(label);
                Controls.Add(box);

                y += 30;
            }

            okButton = new Button { Text = "확인", Left = 280, Top = y, Width = 80, DialogResult = DialogResult.OK };
            AcceptButton = okButton;
            Controls.Add(okButton);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            foreach (var kv in inputBoxes)
            {
                ParameterValues[kv.Key] = kv.Value.Text;
            }
        }
    }
}
