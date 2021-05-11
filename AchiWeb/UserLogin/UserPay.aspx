<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPay.aspx.cs" Inherits="AchiWeb.UserLogin.UserPay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../layui/css/layui.css" rel="stylesheet" />
    <script src="../layui/layui.js"></script>
    <script src="../layui/lay/jquery-3.3.1.min.js"></script>
    <script src="../layui/jquery.cookie.js"></script>
    <title></title>
</head>
<body>
    <form class="layui-form">
        <br /><br /><br /><br />
        <div style="text-align:center">
            <input id ="r1" type="radio" name="Pay" title="50钻石&nbsp&nbsp&nbsp" checked="checked" />
            <input id ="r2" type="radio" name="Pay" title="100钻石&nbsp" />
            <br /><br />
            <input id ="r3" type="radio" name="Pay" title="500钻石&nbsp" />
            <input id ="r4" type="radio" name="Pay" title="1000钻石" />
        </div>
        <br />
        <div style="text-align:center">
            <input type="radio" name="type" checked="checked" />
            <img src="../img/types.jpg" style="width:260px;height:160px;" />
        </div>
        <br /><br />
        <div class="layui-form-item" style="text-align:center">
           <button type="button" onclick="login()" class="layui-btn layui-btn-lg layui-btn-normal">支付宝付款</button>
        </div>
    </form>
    <script>
        layui.use('form', function () {
            var form = layui.form;
        });
    </script>
    <script type="text/javascript">
        function getQueryVariable(variable) {
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (pair[0] == variable) { return pair[1]; }
            }
            return (false);
        }
        function login() {
            layui.use('layer', function () {
                var layer = layui.layer;
                var vals = 0;
                if (r1.checked) {
                    vals = 50;
                } else if (r2.checked) {
                    vals = 100;
                } else if (r3.checked) {
                    vals = 500;
                } else {
                    vals = 1000;
                }

                var Acction = getQueryVariable("Acction");
                var GUID = getQueryVariable("GUID");
                var API = getQueryVariable("API");
                var url = "http://" + API+"/Paycode_url.aspx?Acction=" + Acction + "&price=" + vals + "&param=" + GUID;
                //var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                window.location.href = url;
                //parent.$('#')
            });
        }
    </script>
</body>
</html>
