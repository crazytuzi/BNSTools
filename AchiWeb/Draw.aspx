<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Draw.aspx.cs" Inherits="AchiWeb.Draw" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>[500剑灵]抽奖</title>
    <script src="loginJS/_500BNS.js"></script>
    <link href="css/Draw.css" rel="stylesheet" />
    <script src="layui/layui.js"></script>
    <script src="layui/lay/jquery-3.3.1.min.js"></script>
    <script src="layui/jquery.cookie.js"></script>
    <link href="layui/css/layui.css" rel="stylesheet" />

</head>
<body>
    <form class="layui-form">
    <div>
            
            <div id ="login_div" class="font Logindiv ">
                <label>尊敬的灵芝,请<a href="javascript:void(0);" onclick="loginbtn()" style="color:royalblue">登录</a></label>
            </div>
            <div id="DrawDiv" class="font title ">
                <label></label>
            </div>

            <div id="DrawTimer" class="font Drawtimer ">
                    <label>活动时间:2020.09.03 - 2020.11.30</label>
            </div>
        
            <div id="fdText" class="font fdtxt ">
                <label></label>
            </div>
            <div id="divbtn">
                <button type="button" class="layui-btn layui-btn-lg layui-btn-norma layui-btn-warm btn">立即兑换</button>
            </div>
            <div id="fd1div" class="font wb1 ">
                <label></label>
            </div>
            <div id="fd2div" class="font wb2 ">
                <label></label>
            </div>
            <div id="bal" class="font ye ">
                <label>0</label>
            </div>
            <div id="cz" class="font yebtn ">
                <label><a href="javascript:void(0);"  style="color:#FFB800">充值</a></label>
            </div>
            <div id="hd" class="font hqjl ">
                <label><a href="javascript:void(0);" style="color:#fff">我的物品</a></label>
            </div>
            <div id="wdjf" class="myjf ">
                <label>我的积分0</label>
            </div>
            <div id="_1lc" class="lclab ">
                <label>0钻石</label>
            </div>
            <div id="_1lc_btn" class="lcbtn ">
                <a href="javascript:void(0);" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>
            </div>
            <div id="_1dc" class="dclab">
                <label>0钻石</label>
            </div>
            <div id="_1dc_btn" class="dcbtn ">
                <a href="javascript:void(0);" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>
            </div>
            <div id="_2lc" class="lclab2 ">
                <label>0钻石</label>
            </div>
            <div id="_2lc_btn" class="lcbtn2 ">
                <a href="javascript:void(0);" style="font-size:40px;">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>
            </div>
            <div id="_2dc" class="dclab2 ">
                <label>0钻石</label>
            </div>
            <div id="_2dc_btn" class="dcbtn2 ">
                <a href="javascript:void(0);" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>
            </div>
        <div class="tc ">
            <label id="GUID" style="visibility:hidden">d008f96c-8397-4ee8-ab33-c0ccdeae920c</label>
        <label id="API" style="visibility:hidden">175.9.218.175:8012</label>
            <label><br /></label>
        </div>
        </div></form>
        <script>
            //一般直接写在一个js文件中
            layui.use(['layer', 'form'], function () {
                var layer = layui.layer
                    , form = layui.form;
                var acction = $.cookie('Acction');
                var password = $.cookie('Password');
                var jsonstr = $.cookie('jsonstr');
                var pcid = $.cookie('PCID');
                var pcidName = $.cookie('PcidName');
                if (acction != null && pcid != null && password != null && jsonstr != null) {
                    //已绑定角色
                    parent.$('#login_div').html('<label>亲爱的灵芝&nbsp' + acction + '&nbsp [' + pcidName + '] &nbsp<a href="javascript:void(0);" onclick="bindProjcet()" style="color:royalblue">更改绑定</a>&nbsp<a href="#" onclick="logout()" style="color:royalblue">注销</a>');

                } else if (acction != null && pcid == null && password != null && jsonstr != null) {
                    //未绑定角色
                    parent.$('#login_div').html('<label>亲爱的灵芝 ' + acction + ' ,</label><a href="javascript:void(0);" onclick="bindProjcet()" style="color:royalblue">绑定角色</a> , <a href="#" onclick="logout()" style="color:royalblue">注销</a>');

                } else {
                    //未登录 不处理
                }
                GetDrawData();
                //layer.msg('Hello World');
            });
        </script> 
</body>
</html>
