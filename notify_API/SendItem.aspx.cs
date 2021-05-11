using notify_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace notify_API
{
    public partial class SendItem : System.Web.UI.Page
    {
        public string return_result = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string Acction = Request.QueryString["Acction"];
                string ItemID = Request.QueryString["ItemID"];
                string Count = Request.QueryString["Count"];
                string Pwd = Request.QueryString["Pwd"];
                if (Pwd != null && Pwd == "BNSServerP")
                {
                    if (Acction == null || ItemID == null)
                    {
                        return_result = "Acction & Item is NULL";
                        return;
                    }
                    if (Count == null)
                    {
                        Count = "1";
                    }
                    string sqlstr = $"select [UserId] from [PlatformAcctDb].dbo.[Users] where [LoginName] = N'{Acction}'+ N'@ncsoft.com'";
                    string UserID = DBHelper.SelectScalar(sqlstr).ToString().ToUpper();
                    string Key = GetKey(ItemID);
                    string Epoch_str = Tools.Get("http://127.0.0.1:6605/apps-state", new Dictionary<string, string>());
                    Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<AppName>WarehouseSrv</AppName>"));
                    Epoch_str = Epoch_str.Substring(Epoch_str.IndexOf("<Epoch>") + 7);
                    string Epoch = Epoch_str.Substring(0, Epoch_str.IndexOf("</Epoch>"));


                    DataItems_Entity DataItems = GetID(Key);


                    if (DataItems != null)
                    {
                        if (Sends(UserID, DataItems.GoodsId, DataItems.ItemId, Key, Count, Epoch))

                            return_result = "Success";
                        else
                            return_result = "Error";
                    }
                    else
                        return_result = "DataItems is NULL";

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return_result = $"Try Error :\r\n{ex.Message}";
            }


        }

        /// <summary>
        /// GoodsID与ItemID处理
        /// </summary>
        /// <param name="ItemKey">道具ID</param>
        /// <returns></returns>
        private DataItems_Entity GetID(string ItemKey) 
        {
            string sqlstr = $@"
select DISTINCT a.[GoodsId],a.[ItemId],b.[GoodsName],c.[ItemName],a.[ItemQuantity],d.[BasicSalePrice],e.[GameItemKey] from [GoodsDb].dbo.[GoodsItems] as a 
left join [GoodsDb].dbo.[Goods] as b on a.[GoodsId] = b.[GoodsId ]
left join [GoodsDb].dbo.[Items] as c on a.[ItemId] = c.[ItemId]
left join [GoodsDb].dbo.[GoodsItemBasicPrices] as d on a.[GoodsId] = d.[GoodsId]
left join [GoodsDb].dbo.[GameItems] as e on a.[ItemId] = e.[ItemId]
where e.[GameItemKey] = '{ItemKey}' and a.[GoodsId] > 10000";
            DataItems_Entity Data = null;
            using (SqlDataReader sdr = DBHelper.SelectReader(sqlstr))
            {
                if (sdr.Read())
                {
                    Data = new DataItems_Entity()
                    {
                        GoodsId = Convert.ToInt32(sdr["GoodsId"]),
                        ItemId = Convert.ToInt32(sdr["ItemId"]),
                        GoodsName = sdr["GoodsName"].ToString(),
                        ItemName = sdr["ItemName"].ToString(),
                    };
                }
                sdr.Close();
            }
            if (Data != null)
            {
                if (Data.ItemName != "系统发送道具" || Data.GoodsName != "系统发送道具")
                {
                    Data = null;
                }
            }
            
            if (Data == null) // 无 开始新增
            {
                #region 获取最新ID
                string sql = $"select top 1 [GoodsId],[ItemId] from [GoodsDb].dbo.[GoodsItems] where GoodsId>=15000 and GoodsId<50000 order by GoodsId desc";
                int ID = Convert.ToInt32(DBHelper.SelectScalar(sql));
                ID++;//自增1

                #region ItemsID
                sql = $"Select [ItemId] from [GoodsDb].dbo.[Items] where [ItemId]='{ID}'";
                int ItemId = Convert.ToInt32(DBHelper.SelectScalar(sql) == DBNull.Value ? 0 : DBHelper.SelectScalar(sql));
                if (ItemId == 0)
                {
                    ItemId = ID;
                    sql = $@"
            begin tran
begin try

insert into [GoodsDb].dbo.[Items] (ItemId,ItemName,ItemAppGroupCode,ItemType,IsConsumable,BasicPrice,BasicCurrencyGroupId,Changed,ChangerAdminAccount,ItemDescription) values ({ItemId},N'系统发送道具','bnsgrnTH',3,0,0,69,GetDate(),'TestAdminAccount',N'系统发送道具');
insert into [GoodsDb].dbo.[GameItems] (ItemId,GameItemKey,GameItemData) values ({ItemId},'{ItemKey}','AAAAAAAAAAA=');
insert into [GoodsDb].dbo.[ItemDisplay] (ItemId,LanguageCode,ItemDisplayName,ItemDisplayDescription) values ({ItemId},11,N'系统发送道具',N'系统发送道具');
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
                    ";
                   int ItemInsertCount = DBHelper.IDU(sql);
                    if (ItemInsertCount < 3)
                    {
                        return null;
                    }
                }

                #endregion

                #region GoodsID
                sql = $"Select [GoodsId] from [GoodsDb].dbo.[Goods] where [GoodsId]='{ID}'";
                int GoodsId = Convert.ToInt32(DBHelper.SelectScalar(sql) == DBNull.Value ? 0 : DBHelper.SelectScalar(sql));
                if (GoodsId == 0)
                {
                    GoodsId = ID;
                    sql = $@"
            begin tran
begin try

insert into [GoodsDb].dbo.[Goods ](GoodsId,GoodsName,GoodsAppGroupCode,GoodsType,DeliveryType,SaleStatus,EffectiveFrom,EffectiveTo,SaleableQuantity,RefundUnitCode,IsRefundable,IsAvailableRecurringPayment,Changed,ChangerAdminAccount,GoodsDescription,GoodsData,GoodsPurchaseType,GoodsPurchaseCheckMask) VALUES ({GoodsId},N'系统发送道具','bnsgrnTH',3,1,2,'2017-09-05 00:00:00','2099-12-31 23:59:59',0,1,0,1,GETDATE(),'TestAdminAccount',N'系统发送道具','AAAAAAE=',1,0);

insert into [GoodsDb].dbo.[GoodsDisplay] (GoodsId,LanguageCode,GoodsDisplayName,GoodsDisplayDescription) values ({GoodsId},11,N'系统发送道具',N'系统发送道具');

insert into [GoodsDb].dbo.[GoodsBasicPrices] (GoodsId,CurrencyGroupId,BasicSalePrice,RefundFee) values ({GoodsId},71,0,0);

insert into [GoodsDb].dbo.[GoodsSalePricePolicies] (GoodsId,CurrencyGroupId,PricePolicyType,EffectiveFrom,EffectiveTo,SalePrice) values ({GoodsId},71,1,'2017-06-06 00:00:00','2099-12-31 23:59:59',9999);

insert into [GoodsDb].dbo.[GoodsChanges] (ChangeId,ChangeType,Registered,RegistrarAdminAccount,GoodsAppGroupCode,IsDisplayable) values ({GoodsId},2,GETDATE(),'TestAdminAccount','bnsgrnTH',1);

insert into [GoodsDb].dbo.[GoodsItems] (GoodsId,ItemId,ItemQuantity,ItemExpirationType,ItemData,DeliveryPriority) values ({GoodsId},{ItemId},1,0,'AAAAAAEA',1);

insert into [GoodsDb].dbo.[GoodsItemBasicPrices] (GoodsId,ItemId,CurrencyGroupId,BasicSalePrice) values ({GoodsId},{ItemId},71,9999)
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran
                    ";
                    int GoodsInsertCount = DBHelper.IDU(sql);

                    if (GoodsInsertCount < 7)
                    {
                        return null;
                    }
                }
                #endregion

                return new DataItems_Entity()
                {
                    GoodsId = GoodsId,
                    ItemId = ItemId
                };
                #endregion
            }
            else 
            {
                return Data;//有 直接返回
            }
        }

        /// <summary>
        /// 发送道具
        /// </summary>
        /// <param name="UserID">UserID</param>
        /// <param name="GoodsID">GoodsID</param>
        /// <param name="ItemID">ItemID</param>
        /// <param name="ItemKey">ItemKey</param>
        /// <param name="Epoch">Epoch</param>
        /// <returns></returns>
        private bool Sends(string UserID, int GoodsID,int ItemID,string ItemKey,string Count, string Epoch)
        {
            string key = HttpUtility.UrlEncode(ItemKey);
            string Get_Str = $"http://127.0.0.1:6605/spawned/WarehouseSrv.1.{Epoch}/warehouse/insert?UserId={UserID}&GameCode=bnsgrnTH&IsRefundable=0&IsRefundExpirationDate=1&RefundEffectiveUntil=2099-12-31T23:59:59Z&RequestId=1234&RequestCode=5&GoodsQuantity=1&GoodsId={GoodsID}&GameServerId=7801&CharacterId=0&CharacterName=&SenderDescription=&GiftMessage=&EffectiveUntil=&IsGifted=0&GoodsWarehouseData=&ItemId={ItemID}&GameItemId={key}&ItemQuantity={Count}&GameItemData=AAAAAAAAAAA%3D&FirstRequestFromCode=5";
            string _res_1 = Tools.Get(Get_Str, new Dictionary<string, string>());
            if (_res_1.Contains("</Reply>")) 
            {
//                Thread.Sleep(100);
//                string sqlstr = $@"update [GamewarehouseDB].dbo.[warehouseitem] set [ItemDataID] = {ItemID} where [ItemInstanceID] = 
//(select top 1 a.[ItemInstanceID] from[GameWarehouseDB].dbo.[warehouseitem] as a inner join[PlatformAcctDb].dbo.[Users] as b on a.[OwnerAccountID] = b.[UserId] where 1 = 1 and b.[UserId] = '{UserID}' order by[RegistrationTime] desc)";
//                DBHelper.IDU(sqlstr);
                return true;
            }
            else
                return false;
        }
        private string GetKey(string ItemID)
        {
            List<string> ls1 = new List<string>();
            ls1.Add("A");
            ls1.Add("B");
            ls1.Add("C");
            ls1.Add("D");
            ls1.Add("E");
            ls1.Add("F");
            ls1.Add("G");
            ls1.Add("H");
            ls1.Add("I");
            ls1.Add("J");
            ls1.Add("K");
            ls1.Add("L");
            ls1.Add("M");
            ls1.Add("N");
            ls1.Add("O");
            ls1.Add("P");
            ls1.Add("Q");
            ls1.Add("R");
            ls1.Add("S");
            ls1.Add("T");
            ls1.Add("U");
            ls1.Add("V");
            ls1.Add("W");
            ls1.Add("X");
            ls1.Add("Y");
            ls1.Add("Z");

            ls1.Add("a");
            ls1.Add("b");
            ls1.Add("c");
            ls1.Add("d");
            ls1.Add("e");
            ls1.Add("f");
            ls1.Add("g");
            ls1.Add("h");
            ls1.Add("i");
            ls1.Add("j");
            ls1.Add("k");
            ls1.Add("l");
            ls1.Add("m");
            ls1.Add("n");
            ls1.Add("o");
            ls1.Add("p");
            ls1.Add("q");
            ls1.Add("r");
            ls1.Add("s");
            ls1.Add("t");
            ls1.Add("u");
            ls1.Add("v");
            ls1.Add("w");
            ls1.Add("x");
            ls1.Add("y");
            ls1.Add("z");

            ls1.Add("0");
            ls1.Add("1");
            ls1.Add("2");
            ls1.Add("3");
            ls1.Add("4");
            ls1.Add("5");
            ls1.Add("6");
            ls1.Add("7");
            ls1.Add("8");
            ls1.Add("9");

            ls1.Add("+");
            ls1.Add("/");

            List<string> ends = new List<string>();
            int v = int.Parse(ItemID) * 16;
            while (v >= 64)
            {
                int idx = (int)((float)v % 64);
                ends.Add(ls1[idx]);
                v = (int)(Math.Floor((float)v / 64f));
            }
            ends.Add(ls1[v]);
            string _mids = string.Empty;
            for (int i = ends.Count - 1; i >= 0; i--)
            {
                _mids += ends[i];
            }
            string _Code = "AA" + _mids + "==";
            return _Code;
        }
    }
}