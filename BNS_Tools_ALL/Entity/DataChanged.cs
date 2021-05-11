using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools_ALL.Entity
{
    public class DataChanged
    {
        private string str { get; set; }
        public string ServerIP { get; set; }
        /// <summary>
        /// 主窗体
        /// </summary>
        private Form _frm;

        /// <summary>
        /// 构造函数 用来获取主窗体
        /// </summary>
        /// <param name="f"></param>
        public DataChanged(Form f) 
        {
            _frm = f;
        }

        /// <summary>
        /// Sql连接状态
        /// </summary>
        public bool IsConnection = false;

        /// <summary>
        /// 回调连接状态事件
        /// </summary>
        public Action<bool,string,string,string> ConnStatus;

        /// <summary>
        /// 回调操作数据库影响行数
        /// </summary>
        public Action<int> IDU_Count;

        /// <summary>
        /// 回调返回数据库查询结果
        /// </summary>
        public Action<SqlDataReader> SDR_Count;
        
        /// <summary>
        /// 对数据库进行连接
        /// </summary>
        /// <param name="ServerIP">服务器IP 必须是IP 若是域名请修改</param>
        /// <param name="Acction">数据库账号</param>
        /// <param name="Password">数据库密码</param>
        public void Connection(string ServerIP, string Acction,string Password) 
        {
            str = $"server={ServerIP};database=master;uid={Acction};pwd={Password}";
            this.ServerIP = ServerIP;
            if (ServerIP.Contains(","))
            {
                this.ServerIP = ServerIP.Substring(0, ServerIP.IndexOf(","));
            }
            Task.Run(() =>
            {
                try
                {
                    SqlConnection conn = new SqlConnection(str);
                    conn.Open();
                    IsConnection = true;
                    conn.Close();
                    ConnStatus?.Invoke(true,ServerIP,Acction,Password);
                    
                }
                catch (Exception ex)
                {
                    IsConnection = false;
                    ConnStatus?.Invoke(false,null,null,null);
                    _frm.Invoke(new Action(delegate
                    {
                        MessageBox.Show($"连接数据库时发生异常 异常内容：{ex.Message}");
                    }));
                }
            });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close() 
        {
            //this.IsConnection = false;
        }

        /// <summary>
        /// 进行数据操作
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <returns></returns>
        public void SQL_IDU(string sql) 
        {
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand com = new SqlCommand(sql, conn);
                int count = com.ExecuteNonQuery();
                IDU_Count?.Invoke(count);
            }
            catch (Exception ex)
            {
                _frm.Invoke(new Action(delegate
                {
                    MessageBox.Show($"操作数据时发生异常 异常内容：{ex.Message}");
                }));
            }
        }

        /// <summary>
        /// 进行数据查询
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        public void SQLRead(string sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                SDR_Count?.Invoke(sdr);
            }
            catch (Exception ex)
            {
                _frm.Invoke(new Action(delegate 
                {
                    MessageBox.Show($"查询数据时发生异常 异常内容：{ex.Message}");
                }));
            }

        }

        /// <summary>
        /// 进行数据操作 返回数据
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <returns></returns>
        public int _SQL_IDU(string sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                int x = cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                return x;

            }
            catch (Exception ex)
            {
                _frm.Invoke(new Action(delegate
                {
                    MessageBox.Show($"操作数据时发生异常 异常内容：{ex.Message}");
                }));
            }
            return 0;
        }

        /// <summary>
        /// 进行数据查询 返回数据
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        public SqlDataReader _SQLRead(string sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                _frm.Invoke(new Action(delegate
                {
                    MessageBox.Show($"查询数据时发生异常 异常内容：{ex.Message}");
                }));
            }
            return null;
        }

        /// <summary>
        /// 进行数据查询 返回数据
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        public object _SQLScalar(string sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                object x = cmd.ExecuteScalar();
                conn.Close();
                conn.Dispose();
                return x;
            }
            catch (Exception ex)
            {
                _frm.Invoke(new Action(delegate
                {
                    MessageBox.Show($"查询数据时发生异常 异常内容：{ex.Message}");
                }));
            }
            return null;
        }
    }
}
