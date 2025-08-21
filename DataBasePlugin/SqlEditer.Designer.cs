
namespace DataBasePlugin
{
    partial class SqlEditer
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
            this.editor = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // editor
            // 
            this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editor.BackColor = System.Drawing.Color.Black;
            this.editor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editor.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.editor.Location = new System.Drawing.Point(10, 59);
            this.editor.Margin = new System.Windows.Forms.Padding(0);
            this.editor.Multiline = true;
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(241, 137);
            this.editor.TabIndex = 0;
            // 
            // SqlEditer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editor);
            this.MinHeightSize = 46;
            this.Name = "SqlEditer";
            this.Size = new System.Drawing.Size(262, 206);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox editor;
    }
}
