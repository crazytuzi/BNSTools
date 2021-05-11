using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace notify_API.Models
{
    public static class DBsql
    {
        public static SqlSugarClient db = new SqlSugarClient(
new ConnectionConfig()
{
    ConnectionString = ConfigurationManager.ConnectionStrings["SQLDBConn"].ConnectionString + ConfigurationManager.ConnectionStrings["notify"].ConnectionString,
   DbType = SqlSugar.DbType.SqlServer,//设置数据库类型
           IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
           InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
       });
    }
}