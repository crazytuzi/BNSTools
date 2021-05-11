
--使用前请先备份以下数据库
--[GameWarehouseDB] 礼品盒库
--[BlGame01] 角色属性道具库
--[VirtualCurrencyDb] 点券库
begin tran
            begin try
delete [BlGame01].dbo.[GemProperty] where depot != '1'; --除身上带的八卦 其他删除
delete [BlGame01].dbo.[AccessoryProperty] where depot != '1'; --除身上带的首饰 其他删除
delete [BlGame01].dbo.[WeaponProperty] where depot != '1'; --除身上带的武器、宝石、幻化 其他删除
delete [BlGame01].dbo.[CostumeProperty] where depot != '1'; --除身上穿的衣服 其他删除
--delete [BlGame01].dbo.[Closet]; --删除衣柜数据 已注释不执行
delete [BlGame01].dbo.[GroceryProperty]; -- 清空材料/消耗品
delete [BlGame01].dbo.[UserPost]; --清空邮件
delete [BlGame01].dbo.[UserPostAttachment]; --清空邮件物品
delete [GameWarehouseDB].dbo.[WarehouseItemArchive]; --清空礼品盒
delete [GameWarehouseDB].dbo.[warehouseitem]; --清空礼品盒
delete [GameWarehouseDB].dbo.[WarehouseGoods]; --清空礼品盒
delete [GameWarehouseDB].dbo.[WarehouseStatistics]; --清空礼品盒
delete [GameWarehouseDB].dbo.[ErrorLog]; --清空礼品盒报错日志
delete [GameWarehouseDB].dbo.[WarehouseGoodsArchive]; --清空礼品盒礼包
delete [VirtualCurrencyDb].dbo.[WithdrawalPerDeposit];--清空点券
delete [VirtualCurrencyDb].dbo.[Deposits];--清空点券
            end try
begin catch
   if(@@trancount > 0)
      rollback tran
end catch
if(@@trancount > 0)
commit tran