<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserGetItem.aspx.cs" Inherits="AchiWeb.UserLogin.UserGetItem" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
    </script>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../layui/css/layui.css" rel="stylesheet" />
    <script src="../layui/lay/jquery-3.3.1.min.js"></script>
    <script src="../layui/layui.js"></script>
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
    </script>
</head>
<body>
    <table class="layui-hide" id="test"></table>
    <script>
    layui.use('table', function(){
  var table = layui.table;
        var Acction = getQueryVariable("Acction");
        var GUID = getQueryVariable("GUID");
        var API = getQueryVariable("API");
        var url = 'http://'+API+'/getitem/GetRecord.aspx?Acction=' + Acction + '&GUID=' + GUID;
        table.render({
            elem: '#test',
            url: url, height: 440
            , cols: [[
                { field: 'index', width: 70, title: '序号' },
                { field: 'ItemName', width: 210, title: '物品名称'}
                , { field: 'ItemTimer', width: 210, title: '获取时间' }
            ]]
            , page: false
        });
        });
  
    </script>
</body>
</html>
