using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Store_Tools
{
    public class DataChanged
    {
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
        /// 数据库连接
        /// </summary>
        public SqlConnection conn = new SqlConnection();

        /// <summary>
        /// 回调连接状态事件
        /// </summary>
        public Action<bool> ConnStatus;

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
           string sqlstr = $"server={ServerIP};database=GoodsDB;uid={Acction};pwd={Password}";
            Task.Run(() =>
            {
                try
                {
                    conn = new SqlConnection(sqlstr);
                    conn.Open();
                    IsConnection = true;
                    ConnStatus?.Invoke(true);
                }
                catch (Exception ex)
                {
                    IsConnection = false;
                    ConnStatus?.Invoke(false);
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
            conn.Close();
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
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
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
                SqlCommand com = new SqlCommand(sql, conn);
                return  com.ExecuteNonQuery();
               
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
                SqlCommand cmd = new SqlCommand(sql, conn);
                return cmd.ExecuteReader();
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
                SqlCommand cmd = new SqlCommand(sql, conn);
                return cmd.ExecuteScalar();
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
