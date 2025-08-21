using BaseComponent;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DataBasePlugin
{
    [iComponent("Data Base/Result grid", "Resources\\gridView.png")]
    public partial class ResultGrid : BasePanel
    {
        #region Input/Output
        NodePort inputResult;
        #endregion

        #region private
        private DataTable currentTable;
        #endregion

        #region 생성자
        public ResultGrid()
        {
            InitializeComponent();

            this.Title = "Result";
            this.TitleBackColor = ColorTemplate.DarkPastelGray;
            

            inputResult = AddInputPort("Result", PortDataType.DataTable, value => { TxCommand(inputResult, value); });


            // VirtualMode 설정
            dataGrid.VirtualMode = true;
            dataGrid.ReadOnly = true;
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.CellValueNeeded += dataGrid_CellValueNeeded;

            // 테마 적용
            ApplyDarkThemeToDataGrid(dataGrid);
            typeof(DataGridView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty, null, dataGrid, new object[] { true });
        }
        #endregion

        #region event 함수
        private void TxCommand(NodePort sender, object value)
        {
            if (sender.DataType == PortDataType.DataTable)
            {
                currentTable = value as DataTable;

                if (currentTable != null)
                {
                    dataGrid.SuspendLayout();

                    dataGrid.Columns.Clear();
                    foreach (DataColumn col in currentTable.Columns)
                    {
                        dataGrid.Columns.Add(col.ColumnName, col.ColumnName);
                    }

                    dataGrid.RowCount = currentTable.Rows.Count;

                    dataGrid.ResumeLayout();
                }
            }
        }

        private void dataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (currentTable != null &&
                e.RowIndex >= 0 && e.RowIndex < currentTable.Rows.Count &&
                e.ColumnIndex >= 0 && e.ColumnIndex < currentTable.Columns.Count)
            {
                e.Value = currentTable.Rows[e.RowIndex][e.ColumnIndex];
            }
        }
        #endregion

        #region 사용자정의 함수

        private void ApplyDarkThemeToDataGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(30, 30, 30);
            grid.ForeColor = Color.FromArgb(220, 220, 220);

            grid.DefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(220, 220, 220);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 153, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;

            grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            grid.RowHeadersDefaultCellStyle.ForeColor = Color.FromArgb(180, 180, 180);

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(240, 240, 240);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(64, 64, 64);
        }
        #endregion

    }
}
