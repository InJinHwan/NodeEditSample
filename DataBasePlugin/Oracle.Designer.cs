
namespace DataBasePlugin
{
    partial class Oracle
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.logConsole = new BaseComponent.ConsoleControl();
            this.btnConnect = new BaseComponent.ButtonControl();
            this.SuspendLayout();
            // 
            // logConsole
            // 
            this.logConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logConsole.BackColor = System.Drawing.Color.Transparent;
            this.logConsole.LFEFBOTTOM = true;
            this.logConsole.Location = new System.Drawing.Point(11, 67);
            this.logConsole.Margin = new System.Windows.Forms.Padding(0);
            this.logConsole.MAXLINES = 100;
            this.logConsole.Name = "logConsole";
            this.logConsole.RIGHTBOTTOM = true;
            this.logConsole.Size = new System.Drawing.Size(214, 91);
            this.logConsole.TabIndex = 1;
            this.logConsole.TIMESTEMP = true;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.DimGray;
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Icon = global::DataBasePlugin.Properties.Resources.disconnect;
            this.btnConnect.IconAlign = BaseComponent.ButtonControl.IconAlignment.Left;
            this.btnConnect.IconSize = new System.Drawing.Size(20, 20);
            this.btnConnect.IconSpacing = 2;
            this.btnConnect.Location = new System.Drawing.Point(11, 36);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(0);
            this.btnConnect.Multiline = false;
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.ShadowColor = System.Drawing.Color.DarkGray;
            this.btnConnect.ShadowOffset = 1;
            this.btnConnect.Size = new System.Drawing.Size(100, 27);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.TextAlign = BaseComponent.ButtonControl.TextAlignment.Left;
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // Oracle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.logConsole);
            this.MinHeightSize = 46;
            this.Name = "Oracle";
            this.Size = new System.Drawing.Size(237, 168);
            this.ResumeLayout(false);

        }

        #endregion
        private BaseComponent.ConsoleControl logConsole;
        private BaseComponent.ButtonControl btnConnect;
    }
}
