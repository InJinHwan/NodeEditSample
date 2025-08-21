using BaseComponent;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DataBasePlugin
{
    [iComponent("Data Base/Sql Editer", "Resources\\sql.png")]
    public partial class SqlEditer : BasePanel
    {
        #region enum
        private enum QueryType
        {
            Select,
            StoredProcedure,
            NonQuery,
            Unknown
        }
        #endregion

        #region private
        private OracleDbHelper odh = null;
        #endregion

        #region Input/Output
        NodePort inputPort;
        NodePort outputPort;
        #endregion

        #region 생성자
        public SqlEditer()
        {
            InitializeComponent();

            this.Title = "Sql editer";
            this.TitleBackColor = ColorTemplate.DarkPastelSteelBlue;

            inputPort = AddInputPort("Handle", PortDataType.DataAccess, value => { TxCommand(inputPort, value); }, allowMultipleConnections: false);
            outputPort = AddOutputPort("Result", PortDataType.DataTable);
        }
        #endregion

        #region 사용자 정의 함수

        /// <summary>
        /// 텍스트박스에서 현재 커서 위치 기준으로 SQL 블록을 추출
        /// 세미콜론(;)이 있으면 해당 블럭 전체,
        /// 없으면 줄 단위 쿼리 추출
        /// </summary>
        private string GetCurrentSqlBlock()
        {
            string text = editor.Text;
            int cursorPos = editor.SelectionStart;

            // 1. 세미콜론 기준 블록 검색
            var matches = Regex.Matches(text, @"(.*?;)", RegexOptions.Singleline);
            int offset = 0;

            foreach (Match match in matches)
            {
                int length = match.Length;
                int end = offset + length;

                if (cursorPos <= end)
                {
                    return match.Value.Trim();
                }

                offset = end;
            }

            // 2. 세미콜론이 없으면 현재 라인 추출
            int lineStart = text.LastIndexOf('\n', Math.Max(0, cursorPos - 1));
            int lineEnd = text.IndexOf('\n', cursorPos);

            if (lineStart == -1) lineStart = 0; else lineStart++;
            if (lineEnd == -1) lineEnd = text.Length;

            string currentLine = text.Substring(lineStart, lineEnd - lineStart).Trim();

            return currentLine;
        }

        /// <summary>
        /// 구문 종류 리턴
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private QueryType IsSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return QueryType.Unknown;

            // 공백 및 주석 제거
            sql = sql.TrimStart();
            sql = Regex.Replace(sql, @"^\s*(--.*\n|\s)*", "", RegexOptions.Multiline);
            string firstWord = sql.Split(' ', '\t', '\r', '\n').FirstOrDefault()?.ToUpperInvariant();

            if (string.IsNullOrEmpty(firstWord))
                return QueryType.Unknown;

            if (firstWord == "SELECT" || firstWord == "WITH")
                return QueryType.Select;

            if (firstWord == "EXEC" || firstWord == "CALL")
                return QueryType.StoredProcedure;

            // 기타 나머지는 INSERT, UPDATE, DELETE 등
            return QueryType.NonQuery;
        }

        /// <summary>
        /// 파라미터 이름 추출
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string> ExtractParameters(string sql)
        {
            var matches = Regex.Matches(sql, @"(:|{{OUT}})([A-Za-z0-9_]+)");
            var result = new List<string>();

            foreach (Match match in matches)
            {
                string paramName = match.Groups[2].Value;
                if (!result.Contains(paramName))
                    result.Add(paramName);
            }

            return result;
        }

        /// <summary>
        /// OUTPUT 파라미터 여부 판단
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private bool IsOutputParameter(string sql, string paramName)
        {
            return sql.Contains("{{OUT}}" + paramName);
        }

        //private List<string> ExtractParameters(string sql)
        //{
        //    return Regex.Matches(sql, @"(?<!\w)([@:][a-zA-Z_][a-zA-Z0-9_]*)")
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .Distinct()
        //        .ToList();
        //}



        /// <summary>
        /// 저장 프로시저 이름 추출
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string ExtractProcedureName(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return string.Empty;

            sql = sql.TrimStart();
            sql = Regex.Replace(sql, @"^\s*(--.*\n|\s)*", "", RegexOptions.Multiline); // 주석 제거

            // 대소문자 무시, EXEC 또는 CALL 다음의 식별자 추출
            Match match = Regex.Match(sql, @"^(EXEC|CALL)\s+([a-zA-Z0-9_.\[\]""`]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[2].Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 쿼리 실행 함수
        /// </summary>
        /// <param name="sql"></param>
        private void RunQuery(string sql)
        {
            switch (IsSelectQuery(sql))
            {
                case QueryType.Select:
                    RunSelectQuery(sql);
                    break;

                case QueryType.StoredProcedure:
                    RunStoredProcedure(sql);
                    break;

                case QueryType.NonQuery:
                    RunNonQuery(sql);
                    break;

                case QueryType.Unknown:
                    MessageBox.Show("알 수 없는 쿼리 형식입니다.");
                    break;
            }
        }

        /// <summary>
        /// SELECT 쿼리
        /// </summary>
        /// <param name="sql"></param>
        private void RunSelectQuery(string sql)
        {
            if (odh == null) return;

            // 파라미터 추출
            List<string> parameters = ExtractParameters(sql);
            Dictionary<string, object> paramValues = new Dictionary<string, object>();

            if (parameters.Any())
            {
                using (var dialog = new ParameterInputDialog(parameters))
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    paramValues = dialog.ParameterValues;
                }
            }

            // SELECT 쿼리 처리: 결과 조회형
            DataTable dtSelect = new DataTable();
            if (paramValues.Count > 0) //파라메타가 있다
            {
                List<IDataParameter> oracleParams = new List<IDataParameter>();

                foreach (var pair in paramValues)
                {
                    IDataParameter param = new OracleParameter(pair.Key, pair.Value ?? DBNull.Value);
                    oracleParams.Add(param);
                }
                dtSelect = odh.ExecuteSelect(sql, oracleParams.Count > 0 ? oracleParams : null);
            }
            else
            {
                dtSelect = odh.ExecuteSelect(sql);
            }
            if (dtSelect != null)
            {
                this.TransferData<DataTable>(outputPort, dtSelect);
            }
        }

        /// <summary>
        /// PROCEDURE 실행
        /// </summary>
        /// <param name="sql"></param>
        private void RunStoredProcedure(string sql)
        {
            //if (odh == null) return;

            //// 파라미터 추출
            //List<string> parameters = ExtractParameters(sql);
            //Dictionary<string, object> paramValues = new Dictionary<string, object>();

            //if (parameters.Any())
            //{
            //    using (var dialog = new ParameterInputDialog(parameters))
            //    {
            //        if (dialog.ShowDialog() != DialogResult.OK)
            //            return;

            //        paramValues = dialog.ParameterValues;
            //    }
            //}


            //if (paramValues.Count > 0) //파라메타가 있다
            //{
            //    List<OracleParameter> oracleParams = new List<OracleParameter>();

            //    foreach (var pair in paramValues)
            //    {
            //        var param = new OracleParameter(pair.Key, pair.Value ?? DBNull.Value);
            //        oracleParams.Add(param);
            //    }

            //    var result1 = odh.ExecuteStoredProcedure(ExtractProcedureName(sql), oracleParams.Count > 0 ? oracleParams : null);
            //}
            //else
            //{
            //    var result2 = odh.ExecuteStoredProcedure(ExtractProcedureName(sql));
            //}

            if (odh == null) return;

            // 전체 파라미터 추출
            List<string> allParams = ExtractParameters(sql);

            // IN 파라미터만 필터링
            List<string> inputParams = allParams
                .Where(p => !IsOutputParameter(sql, p))
                .ToList();

            Dictionary<string, object> inputValues = new Dictionary<string, object>();

            // 사용자 입력이 필요한 IN 파라미터가 있을 때만 팝업 실행
            if (inputParams.Any())
            {
                using (var dialog = new ParameterInputDialog(inputParams))
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    inputValues = dialog.ParameterValues;
                }
            }

            // OracleParameter 구성
            List<IDataParameter> oracleParams = new List<IDataParameter>();

            foreach (var paramName in allParams)
            {
                OracleParameter oracleParam;

                if (IsOutputParameter(sql, paramName))
                {
                    oracleParam = new OracleParameter(paramName, OracleDbType.Varchar2);
                    oracleParam.Direction = ParameterDirection.Output;
                    oracleParam.Size = 1000; // 필요 시 크기 조정
                }
                else
                {
                    object value = inputValues.ContainsKey(paramName) ? inputValues[paramName] : DBNull.Value;
                    oracleParam = new OracleParameter(paramName, value);
                    oracleParam.Direction = ParameterDirection.Input;
                }

                oracleParams.Add((IDataParameter)oracleParam);
            }

            // 실행
            var result = odh.ExecuteStoredProcedure(ExtractProcedureName(sql), oracleParams);

            // OUT 파라미터 결과 활용 예시
            foreach (var kvp in result)
            {
                Console.WriteLine($"[{kvp.Key}] => {kvp.Value}");
            }
        }

        /// <summary>
        /// DELETE/INSERT등 실행
        /// </summary>
        /// <param name="sql"></param>
        private void RunNonQuery(string sql)
        {
            if (odh == null) return;

            // 파라미터 추출
            List<string> parameters = ExtractParameters(sql);
            Dictionary<string, object> paramValues = new Dictionary<string, object>();

            if (parameters.Any())
            {
                using (var dialog = new ParameterInputDialog(parameters))
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    paramValues = dialog.ParameterValues;
                }
            }

            int result = 0;
            if (paramValues.Count > 0) //파라메타가 있다
            {
                List<IDataParameter> oracleParams = new List<IDataParameter>();

                foreach (var pair in paramValues)
                {
                    var param = new OracleParameter(pair.Key, pair.Value ?? DBNull.Value);
                    oracleParams.Add((IDataParameter)param);
                }
                result = odh.ExecuteNonQuery(sql, oracleParams.Count > 0 ? oracleParams : null);
            }
            else
            {
                result = odh.ExecuteNonQuery(sql);
            }
        }
        #endregion

        #region event 함수
        private void TxCommand(NodePort sender, object value)
        {
            odh = value as OracleDbHelper;
        }
        #endregion

        #region override
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                string sql = string.IsNullOrWhiteSpace(editor.SelectedText) ? GetCurrentSqlBlock() : editor.SelectedText.Trim();  // ✅ 선택 영역 우선
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    if (odh != null && odh.IsConnected)
                    {
                        // 끝에 세미콜론 있으면 제거
                        if (sql.EndsWith(";"))
                            sql = sql.TrimEnd(';').Trim();

                        RunQuery(sql);
                    }
                    else
                    {
                        MessageBox.Show("DataBase 연결되어있지 않습니다.", "연결정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
            }            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}
