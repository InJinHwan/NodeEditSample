using BaseComponent;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestForm
{
    public partial class Form1 : Form
    {
        private BasePanel currentSelectedComponent = null;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            canvasPanel.ComponentSelected += CanvasPanel_ComponentSelected;

            //특정 컴포넌트 갯수 재한 예제
            canvasPanel.SetComponentLimit("ImagePlugin", 20);
            canvasPanel.SetComponentLimit("SamplePlugin", 20);
        }

        private void CanvasPanel_ComponentSelected(BasePanel obj)
        {
            if (obj != null)
            {
                foreach (Control ctrl in canvasPanel.Controls)
                {
                    if (ctrl is BasePanel bp)
                        bp.SetSelected(false);
                }

                if (obj is BasePanel selected)
                {
                    selected.SetSelected(true);

                    // 기존 이벤트 제거
                    if (currentSelectedComponent != null)
                    {
                        currentSelectedComponent.Move -= SelectedComponent_Moved;
                        currentSelectedComponent.SizeChanged -= SelectedComponent_Resized;
                    }


                    // 새 선택된 컴포넌트 등록 + Move 이벤트 연결
                    currentSelectedComponent = selected;
                    currentSelectedComponent.Move += SelectedComponent_Moved;
                    currentSelectedComponent.SizeChanged += SelectedComponent_Resized;

                    // 기준 위치: CanvasPanel 내에서의 컴포넌트 위치
                    UpdatePropertyGridPosition(selected);

                    // 적용
                    propertyGrid.BringToFront();
                    propertyGrid.SelectedObject = obj;
                    propertyGrid.Visible = canvasPanel.isPropertyGridVisible;
                }
            }
            else
            {
                // 선택 해제 시 숨김 + 이벤트 해제
                if (currentSelectedComponent != null)
                {
                    currentSelectedComponent.Move -= SelectedComponent_Moved;
                    currentSelectedComponent.SizeChanged -= SelectedComponent_Resized;
                }

                // 선택 해제 시 숨김
                currentSelectedComponent = null;
                propertyGrid.Visible = false;
                propertyGrid.SelectedObject = null;
            }
        }


        private void SelectedComponent_Resized(object sender, EventArgs e)
        {
            if (sender is BasePanel panel && propertyGrid.Visible)
            {
                UpdatePropertyGridPosition(panel);
            }
        }

        private void SelectedComponent_Moved(object sender, EventArgs e)
        {
            if (propertyGrid.Visible && sender is BasePanel panel)
            {
                UpdatePropertyGridPosition(panel);
            }
        }

        private void UpdatePropertyGridPosition(BasePanel selected)
        {
            if (!canvasPanel.isPropertyGridVisible) return;

            int padding = 6;

            Size gridSize = propertyGrid.PreferredSize;
            Size panelSize = canvasPanel.ClientSize;

            Point compPos = canvasPanel.PointToClient(selected.PointToScreen(Point.Empty));
            int compRight = compPos.X + selected.Width;
            int compTop = compPos.Y;

            int desiredX = compRight + padding;
            int desiredY = compTop;

            // 우측 경계
            if (desiredX + gridSize.Width > panelSize.Width)
                desiredX = panelSize.Width - gridSize.Width - padding;

            if (desiredX < padding)
                desiredX = padding;

            // 아래쪽 경계
            if (desiredY + gridSize.Height > panelSize.Height)
                desiredY = panelSize.Height - gridSize.Height - padding;

            if (desiredY < padding)
                desiredY = padding;

            propertyGrid.Location = new Point(desiredX, desiredY);
        }
    }
}