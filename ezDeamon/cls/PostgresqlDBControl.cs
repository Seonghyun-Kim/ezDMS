using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezDeamon.cls;
using Npgsql;
namespace ezDeamon
{
    public class PostgresqlDBControl
    {
        private NpgsqlConnection DbConn = null;   //OLEDB 연결자
        private string sConnecion = "";             //OLEDB 연결 문자열
        private string sProvider = "";           //OLEDB 공급자

        #region "clsOleDb 로딩부"

        /// <summary>
        /// clsOleDb 로딩(기본)
        /// </summary>
        public PostgresqlDBControl()
        {
        }

        /// <summary>
        /// clsOleDb 로딩(MS-SQL OLEDB연결 생성)
        /// </summary>
        /// <param name="sProvider">DB 공급자 (ex:ORACLE, MS-SQL)</param>
        /// <param name="sDataSource">Server Host (ip 주소 및 host 명)</param>
        /// <param name="sUser">DB 연결자 (사용자)</param>
        /// <param name="sPwd">DB 연결자 암호</param>
        /// <param name="sDefaultDB">MS-SQL용 DB 명 (ex:Master, Temp 등)</param>
        public PostgresqlDBControl(string sProvider, string sDataSource, string sUser, string sPwd, string sDefaultDB)
        {
            CreateConnectionStr(sProvider, sDataSource, sUser, sPwd, sDefaultDB);
        }

        public PostgresqlDBControl(string sProvider, string sDataSource, string sUser, string sPwd, string sDefaultDB, string sPort)
        {
            CreateConnectionStr(sProvider, sDataSource, sUser, sPwd, sDefaultDB, sPort);
        }

        /// <summary>
        /// clsOleDb 로딩(ORACLE OLEDB연결 생성)
        /// </summary>
        /// <param name="sProvider">DB 공급자 (ex:ORACLE, MS-SQL)</param>
        /// <param name="sDataSource">Server Host (ip 주소 및 host 명)</param>
        /// <param name="sUser">DB 연결자 (사용자)</param>
        /// <param name="sPwd">DB 연결자 암호</param>
        public PostgresqlDBControl(string sProvider, string sDataSource, string sUser, string sPwd)
        {
            CreateConnectionStr(sProvider, sDataSource, sUser, sPwd, "");
        }

        /// <summary>
        /// clsOleDb 로딩(연결문자열로 OLEDB연결 생성)
        /// </summary>
        /// <param name="sConnectionStr">DB연결 문자열</param>
        public PostgresqlDBControl(string sConnectionStr)
        {
            sConnecion = sConnectionStr;

            if (sConnectionStr.IndexOf("Oracle") > -1)
            {
                sProvider = "ORACLE";
            }
            else if (sConnectionStr.IndexOf("SQLOLEDB") > -1)
            {
                sProvider = "MS-SQL";
            }
            else if (sConnectionStr.IndexOf(".mdb") > -1)
            {
                sProvider = "ACCESS";
            }
        }

        ~PostgresqlDBControl()
        {
            DBDisconnect();
        }

        /// <summary>
        /// DB 연결 STRING 만들기
        /// </summary>
        /// <param name="sProvider">ORACLE, MS-SQL</param>
        /// <param name="sDataSource">Server Host (ip 주소 및 host 명)</param>
        /// <param name="sUser">DB 연결자 (사용자)</param>
        /// <param name="sPwd">DB 연결자 암호</param>
        /// <param name="sDefaultDB">MS-SQL용 DB 명 (ex:Master, Pop 등)</param>
        public void CreateConnectionStr(string sProvider, string sDataSource, string sUser, string sPwd, string sDefaultDB, string sPort = "5432")
        {
            sConnecion = "";
            string strPooling = "Enlist=true;Pooling=true;Min Pool Size=10;Max Pool Size=100;Decr Pool Size=2;Incr Pool Size=5;Validate Connection=true;Promotable Transaction=Local";
            switch (sProvider)
            {
                case "ORACLE":
                    sConnecion = string.Format("Persist Security Info=True;"
                                                + "Data Source={0};Provider=OraOLEDB.Oracle.1;"
                                                + "Password={1};User ID={2};"
                                                , sDataSource
                                                , sPwd
                                                , sUser) + strPooling;
                    break;
                case "MS-SQL":
                    sConnecion = string.Format("Persist Security Info=True;"
                                                + "Data Source={0};Provider=SQLOLEDB.1;"
                                                + "Password={1};User ID={2};"
                                                + "Initial Catalog={3};"
                                                , sDataSource
                                                , sPwd
                                                , sUser
                                                , sDefaultDB);

                    break;
                case "ACCESS":
                    sConnecion = string.Format("Persist Security Info=True;"
                                                + "Data Source={0};Provider=Microsoft.Jet.OLEDB.4.0;"
                                                + "Password={1};User ID={2};"
                                                , sDataSource
                                                , sPwd
                                                , sUser);
                    break;
                case "POSTGRESQL":
                    sConnecion = string.Format("host={0};Port={1};Database={2};username={3};password={4};timeout=1024;"
                                                , sDataSource
                                                , sPort
                                                , sDefaultDB
                                                , sUser
                                                , sPwd
                                                );

                    break;
                default:
                    sConnecion = "";
                    break;

            }

            this.sProvider = sProvider;
        }

        #endregion

        #region "clsOleDb 속성부 (읽기전용)"

        /// <summary>
        /// 현재 설정된 Ole Db Connection연결 문자열 반환.
        /// </summary>
        public string OLEDBCONNECTION_STRING
        {
            get
            {
                return sConnecion;
            }
        }
        /// <summary>
        /// 현재 설정된 Ole Db 공급자구분.
        /// </summary>
        public string PROVIDER
        {
            get
            {
                return sProvider;
            }
        }
        /// <summary>
        /// DB연결 상태 반환.
        /// </summary>
        public bool IsDBConnected
        {
            get
            {
                if (DbConn == null)
                {
                    return false;
                }

                if (DbConn.State == System.Data.ConnectionState.Closed)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region "데이터베이스 연결 처리부"

        /// <summary>
        /// 연결지연시간이 없는 데이터베이스 연결
        /// </summary>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>DB 연결 성공여부</returns>
        public bool DBConnect()
        {
            return DBConnect(0);
        }

        /// <summary>
        /// 연결지연시간이 있는 데이터베이스 연결
        /// </summary>
        /// <param name="iDelaySecond">연결지연시간(초)</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>DB 연결 성공여부</returns>
        public bool DBConnect(int iDelaySecond)
        {
            if (sConnecion == "")
            {
                throw new Exception("DB 연결 문자열 설정이 되어 있지 않습니다.!");
            }

            if (IsDBConnected)
            {
                return true;
            }

            try
            {
                if (DbConn == null)
                {
                    if (iDelaySecond != 0) System.Threading.Thread.Sleep(iDelaySecond * 1000);

                    DbConn = new NpgsqlConnection();
                    DbConn.ConnectionString = sConnecion;
                }

                if (DbConn.State == System.Data.ConnectionState.Closed)
                {
                    DbConn.Open();
                }
                return true;
            }

            catch (Exception ex)
            {
                DbConn = null;
                throw ex;
            }
        }
        /// <summary>
        /// 데이터베이스 연결종료(연결종료 에러발생 시 강제종료)
        /// </summary>
        /// <param name="sMsg">리턴:메세지</param>
        public void DBDisconnect()
        {
            if (DbConn == null)
            {
                return;
            }

            try
            {
                if (IsDBConnected)
                {
                    DbConn.Close();
                }
                return;
            }

            catch (Exception ex)
            {
                DbConn = null;
                throw ex;
            }

        }

        #endregion

        #region "데이터베이스 트랜젝션 처리부"

        /// <summary>
        /// 트랜젝션 개체생성
        /// </summary>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>트랜젝션 개체</returns>
        public NpgsqlTransaction DBBeginTrans()
        {
            if (!IsDBConnected)
            {
                if (!DBConnect())
                {
                    throw new Exception("DB 연결이 되지 않았습니다.");
                }
            }

            try
            {
                return DbConn.BeginTransaction();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 트랜젝션 롤백
        /// </summary>
        /// <param name="DbTrans">롤백할 트랜젝션</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>트랜젝션 롤백 성공여부(1:성공, 0:개체없음, -1:실패)</returns>
        public int DBRollBack(NpgsqlTransaction dbTrans)
        {
            if (dbTrans == null)
            {
                throw new Exception("롤백할 Transaction 개체가 존재 하지 않습니다.!");
            }

            try
            {
                dbTrans.Rollback();
                return 1;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 트랜젝션 커밋
        /// </summary>
        /// <param name="DbTrans">커밋할 트랜젝션</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>트랜젝션 커밋 성공여부(1:성공, 0:개체없음, -1:실패)</returns>
        public int DBCommit(NpgsqlTransaction dbTrans)
        {
            if (dbTrans == null)
            {
                throw new Exception("커밋할 Transaction 개체가 존재 하지 않습니다.! ");
            }

            try
            {
                dbTrans.Commit();
                return 1;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "ExecuteNonQuery 관련 (프로시저 및 DML(I/U/D) 관련 쿼리 실행)"

        /// <summary>
        /// 트랜젝션이 없는 DB DML(INSERT, UPDATE, DELETE) QUERY 실행
        /// </summary>
        /// <param name="sQuery">실행할 (INSERT, UPDATE, DELETE) QUERY</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>적용된 Record 개수</returns>
        public int DBExecuteQuery(string sQuery)
        {
            if (sQuery.Trim().Length > 0)
                return DBExecuteQuery(null, sQuery);
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 트랜젝션이 있는 DB DML(INSERT, UPDATE, DELETE) QUERY 실행
        /// </summary>
        /// <param name="dbTrans">Transaction 개체 (null:Transaction없음)</param>
        /// <param name="sQuery">실행할 (INSERT, UPDATE, DELETE) QUERY</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>적용된 Record 개수 (-1:에러) </returns>
        public int DBExecuteQuery(NpgsqlTransaction dbTrans, string sQuery)
        {
            if (!IsDBConnected)
            {
                DBConnect();
            }

            NpgsqlCommand dbCommand = DbConn.CreateCommand(); ;

            try
            {
                dbCommand.CommandText = sQuery;

                if (dbTrans != null)
                {
                    dbCommand.Transaction = dbTrans;
                }

                int iRet = dbCommand.ExecuteNonQuery();

                return iRet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbCommand.Dispose();
            }
        }

        /// <summary>
        /// 트랜젝션이 없는 프로시저 실행
        /// </summary>
        /// <param name="sProcedureName">실행할 프로시저 명</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>프로시저 실행 성공여부(1:성공, 0:개체없음, -1:실패)</returns>
        public int DBExecuteProcedure(string sProcedure)
        {
            string[] sRtnPrarmValue = null;
            return DBExecuteProcedure(null, sProcedure, null, null, null, null, null, out sRtnPrarmValue);
        }

        /// <summary>
        /// 트랜젝션이 있는 프로시저 실행
        /// </summary>
        /// <param name="dbTrans">Transaction 개체 (null:Transaction없음)</param>
        /// <param name="sProcedure">실행할 프로시저 명</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>프로시저 실행 성공여부(-1:실패)</returns>
        public int DBExecuteProcedure(NpgsqlTransaction dbTrans, string sProcedure)
        {
            string[] sRtnPrarmValue = null;

            return DBExecuteProcedure(dbTrans, sProcedure, null, null, null, null, null, out sRtnPrarmValue);
        }

        /// <summary>
        /// 트랜젝션이 없는 프로시저 실행
        /// </summary>
        /// <param name="sProcedure">실행할 프로시저</param>
        /// <param name="bPrarmRtn">프로시저 input/output param 구분</param>
        /// <param name="sPrarmName">프로시저 parameter</param>
        /// <param name="sPrarmValue">프로시저 parameter value</param>
        /// <param name="sPrarmType">프로시저 parameter 데이타타입</param>
        /// <param name="sPrarmLen">프로시저 parameter 길이</param>
        /// <param name="sRtnPrarmValue">리턴:프로시저 parameter ourput value</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>프로시저 실행 성공여부(1:성공, 0:개체없음, -1:실패)</returns>
        public int DBExecuteProcedure(string sProcedure, bool[] bPrarmRtn, string[] sPrarmName, string[] sPrarmValue, NpgsqlTypes.NpgsqlDbType[] sPrarmType, int[] sPrarmLen, out string[] sRtnPrarmValue)
        {
           
            return DBExecuteProcedure(null, sProcedure, bPrarmRtn, sPrarmName, sPrarmValue, sPrarmType, sPrarmLen, out sRtnPrarmValue);
        }

        /// <summary>
        /// 트랜젝션이 있는 프로시저 실행
        /// </summary>
        /// <param name="dbTrans">Transaction 개체 (null:Transaction없음)</param>
        /// <param name="sProcedure">실행할 프로시저</param>
        /// <param name="bPrarmRtn">프로시저 input/output param 구분</param>
        /// <param name="sPrarmName">프로시저 parameter</param>
        /// <param name="sPrarmValue">프로시저 parameter value</param>
        /// <param name="sPrarmType">프로시저 parameter 데이타타입</param>
        /// <param name="sPrarmLen">프로시저 parameter 길이</param>
        /// <param name="sRtnPrarmValue">리턴:프로시저 parameter ourput value</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>프로시저 실행 성공여부(1:성공, 0:개체없음, -1:실패)</returns>
        public int DBExecuteProcedure(NpgsqlTransaction dbTrans, string sProcedure, bool[] bPrarmRtn, string[] sPrarmName, string[] sPrarmValue, NpgsqlTypes.NpgsqlDbType[] sPrarmType, int[] sPrarmLen, out string[] sRtnPrarmValue)
        {
            sRtnPrarmValue = null;

            if (!IsDBConnected)
            {
                DBConnect();
            }

            NpgsqlCommand dbCommand = DbConn.CreateCommand();

            try
            {
                int iRtnPrarmCnt = 0;

                dbCommand.CommandText = sProcedure;
                dbCommand.CommandType = CommandType.StoredProcedure;

                if (sPrarmName != null)
                {
                    for (int iPra = 0; iPra < sPrarmName.Length; iPra++)
                    {
                        dbCommand.Parameters.Add(sPrarmName[iPra], sPrarmType[iPra], sPrarmLen[iPra]);

                        if (bPrarmRtn[iPra] == true)
                        {
                            dbCommand.Parameters[sPrarmName[iPra]].Direction = ParameterDirection.ReturnValue;
                            iRtnPrarmCnt = iRtnPrarmCnt + 1;
                        }
                        else
                        {
                            dbCommand.Parameters[sPrarmName[iPra]].Value = sPrarmValue[iPra];
                        }
                    }
                }

                if (dbTrans != null)
                {
                    dbCommand.Transaction = dbTrans;
                }

                int iRet = dbCommand.ExecuteNonQuery();

                try
                {
                    sRtnPrarmValue = new string[iRtnPrarmCnt];
                    iRtnPrarmCnt = 0;

                    for (int iPraA = 0; iPraA < sPrarmName.Length; iPraA++)
                    {
                        if (bPrarmRtn[iPraA] == true)
                        {
                            sRtnPrarmValue[iRtnPrarmCnt] = dbCommand.Parameters[sPrarmName[iPraA]].Value.ToString();
                            iRtnPrarmCnt = iRtnPrarmCnt + 1;
                        }
                    }
                    return iRet;
                }
                catch
                {
                    return iRet;
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                dbCommand.Dispose();
            }
        }

        #endregion

        #region "DataSet 개체 생성(SELECT 쿼리)"
        /// <summary>
        /// 트랜젝션이 없는 DataSet 반환.
        /// </summary>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>Dataset 개체</returns>
        public DataSet NewDataSet(string sQuery)
        {
            return NewDataSet(null, sQuery);
        }

        /// <summary>
        /// 트랜젝션이 있는 DataSet 반환.
        /// </summary>
        /// <param name="dbTrans">Transaction 개체 (null:Transaction없음)</param>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>Dataset 개체</returns>
        public DataSet NewDataSet(NpgsqlTransaction dbTrans, string sQuery)
        {
            if (!IsDBConnected)
            {
                DBConnect();
            }

            DataSet OleDataSet = new DataSet();

            NpgsqlDataAdapter dbDataAdapter = new NpgsqlDataAdapter();

            NpgsqlCommand dbCommand = DbConn.CreateCommand();

            try
            {
                dbCommand.CommandText = sQuery;
                dbCommand.CommandType = CommandType.Text;

                if (dbTrans != null)
                {
                    dbCommand.Transaction = dbTrans;
                }

                dbDataAdapter.SelectCommand = dbCommand;
                dbDataAdapter.Fill(OleDataSet, "LOCAL");

                return OleDataSet;
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbCommand.Dispose();
            }
        }

        #endregion

        #region "OleDbDataReader 개체 생성(SELECT 쿼리) "

        /// <summary>
        /// 트랜젝션이 없는 OleDbDataReader 반환.
        /// </summary>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>OleDbDataReader 개체</returns>
        public NpgsqlDataReader NewOleDbDataReader(string sQuery)
        {
            int iRcdCount = 0;
            return NewOleDbDataReader(null, sQuery, false, out iRcdCount);
        }
        /// <summary>
        /// 트랜젝션이 없는 OleDbDataReader 와 레코드개수 반환.
        /// </summary>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>OleDbDataReader 개체</returns>
        public NpgsqlDataReader NewOleDbDataReader(string sQuery, out int iRcdCount)
        {
            iRcdCount = 0;
            return NewOleDbDataReader(null, sQuery, true, out iRcdCount);
        }
        /// <summary>
        /// 트랜젝션이 있는 OleDbDataReader 반환.
        /// </summary>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>OleDbDataReader 개체</returns>
        public NpgsqlDataReader NewOleDbDataReader(NpgsqlTransaction dbTrans, string sQuery)
        {
            int iRcdCount = 0;
            return NewOleDbDataReader(dbTrans, sQuery, false, out iRcdCount);
        }
        /// <summary>
        /// 트랜젝션이 있는 OleDbDataReader 와 레코드개수 반환 .
        /// </summary>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>OleDbDataReader 개체</returns>
        public NpgsqlDataReader NewOleDbDataReader(NpgsqlTransaction dbTrans, string sQuery, out int iRcdCount)
        {
            iRcdCount = 0;
            return NewOleDbDataReader(dbTrans, sQuery, true, out iRcdCount);
        }
        /// <summary>
        /// 트랜젝션이 있는 OleDbDataReader 반환.
        /// </summary>
        /// <param name="dbTrans">Transaction 개체 (null:Transaction없음)</param>
        /// <param name="sQuery">select Query</param>
        /// <param name="sMsg">리턴:메세지</param>
        /// <returns>OleDbDataReader 개체</returns>
        public NpgsqlDataReader NewOleDbDataReader(NpgsqlTransaction dbTrans, string sQuery, bool bLoadRcdCnt, out int iRcdCount)
        {
            iRcdCount = 0;

            if (!IsDBConnected)
            {
                DBConnect();
            }

            NpgsqlCommand dbCommand = DbConn.CreateCommand();

            try
            {
                dbCommand.CommandText = sQuery;
                dbCommand.CommandType = CommandType.Text;

                if (dbTrans != null)
                {
                    dbCommand.Transaction = dbTrans;
                }

                if (bLoadRcdCnt)
                {
                    DataSet OleDataSet = new DataSet();
                    NpgsqlDataAdapter dbDataAdapter = new NpgsqlDataAdapter();

                    dbDataAdapter.SelectCommand = dbCommand;
                    dbDataAdapter.Fill(OleDataSet, "LOCAL");

                    iRcdCount = OleDataSet.Tables[0].Rows.Count;
                    OleDataSet.Clear();
                    OleDataSet = null;
                }

                NpgsqlDataReader OleDataReader = dbCommand.ExecuteReader();

                return OleDataReader;
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                dbCommand.Dispose();
            }
        }

        #endregion
    }
}
