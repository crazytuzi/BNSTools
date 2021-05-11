<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Achi.aspx.cs" Inherits="AchiWeb.Achi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>[T1剑灵]肝帝在何方！黑龙幻化免费送！</title>
    <link href="css/T1css.css" rel="stylesheet" />
    <script src="loginJS/T1Login.js"></script>
    <script src="layui/layui.js"></script>
    <script src="layui/lay/jquery-3.3.1.min.js"></script>
    <script src="layui/jquery.cookie.js"></script>
    <link href="layui/css/layui.css" rel="stylesheet" />
    <script type="text/javascript">
        var $button = document.querySelector('.button');
        $button.addEventListener('click', function () {
            var duration = 0.3,
                delay = 0.08;
            TweenMax.to($button, duration, { scaleY: 1.6, ease: Expo.easeOut });
            TweenMax.to($button, duration, { scaleX: 1.2, scaleY: 1, ease: Back.easeOut, easeParams: [3], delay: delay });
            TweenMax.to($button, duration * 1.25, { scaleX: 1, scaleY: 1, ease: Back.easeOut, easeParams: [6], delay: delay * 3 });
        });
    </script>
</head>
<body>
        <div class="content">
            <h2 style="color:cornsilk">
                活动时间:2020.09.03 - 2020.11.30
            </h2>
            <div id ="login_div">
                <label>尊敬的灵芝,请</label><a href="javascript:void(0);" onclick="loginbtn()" style="color:royalblue">登录</a>
            </div>
            <div>
                <br /><br /><br /><br /><br /><br />
                <button type="button" onclick="getitem()" class="layui-btn layui-btn-lg layui-btn-normal layui-btn-radius">领取我的活动专属奖励</button>
            </div>
            <div>
                <label id="GUID" style="visibility:hidden">f4408262-a1be-4a33-a4b3-34d472a9ed13</label>
                <label id="API" style="visibility:hidden">www.scddzj.com:8012</label>
            <label><br /></label>
            </div>
        </div>
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
                parent.$('#login_div').html('<label>亲爱的灵芝&nbsp ' + acction + '  &nbsp [' + pcidName + '] &nbsp&nbsp <a href="javascript:void(0);" onclick="bindProjcet()" style="color:royalblue">更改绑定</a>&nbsp&nbsp <a href="#" onclick="logout()" style="color:royalblue">注销</a>');

            } else if (acction != null && pcid == null && password != null && jsonstr != null) {
                //未绑定角色
                parent.$('#login_div').html('<label>亲爱的灵芝 ' + acction + ' ,</label><a href="javascript:void(0);" onclick="bindProjcet()" style="color:royalblue">绑定角色</a> , <a href="#" onclick="logout()" style="color:royalblue">注销</a>');

            } else {
                //未登录 不处理
            }
            //layer.msg('Hello World');
        });

    </script> 
</body>
</html>
