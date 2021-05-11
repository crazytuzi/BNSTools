<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AchiWeb.UserLogin.Login" %>

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
    <form>
        <br /><br /><br /><br />
        <div class="layui-form-item">
            <label class="layui-form-label">账号</label>
            <div class="layui-input-inline">
              <input type="text" id="Acction" style="width:200px" lay-verify="title" autocomplete="off" placeholder="请输入账号" class="layui-input"/>
            </div>
            </div>
        <br />
        <div class="layui-form-item">
            <label class="layui-form-label">密码</label>
            <div class="layui-input-inline">
              <input type="password" id="Password" style="width:200px" lay-verify="title" autocomplete="off" placeholder="请输入密码" class="layui-input"/>
            </div>
            </div>
        <br />
        <div class="layui-form-item" style="text-align:center">
           <button type="button" onclick="login()" class="layui-btn layui-btn-lg layui-btn-normal">登录</button>
        </div>
    </form>
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
        function login()
        {
            layui.use('layer', function () {
                var layer = layui.layer;
                var Acc = $(" #Acction ").val();
                var pass = $(" #Password ").val();
                var API = getQueryVariable("API");
                var url = "http://" + API + "/User/Login.aspx?Acction=" + Acc + "&Password=" + pass;

                var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                $.ajax({
                    //提交方式
                    //方法所在路径
                    url: url,
                    type:'get',
                    //数据类型
                    //contentType: 'application/x-www-form-urlencoded',
                    //将数据转成JSon格式
                    //接收返回值的方法
                    success: function (result) {
                        if (result != null) {
                            var js = $.parseJSON(result);
                            if (js.Code == 0)
                            {
                                layer.msg(js.Msg);
                                $.cookie('Acction', Acc, { expires: 7, path: '/', secure: false });
                                $.cookie('Password', pass, { expires: 7, path: '/', secure: false  });
                                $.cookie('jsonstr', result, { expires: 7, path: '/', secure: false });
                                parent.$('#login_div').html('<label>尊敬的 ' + Acc + ' ,</label><a href="javascript:void(0);" onclick="bindProjcet()" style="color:royalblue">绑定角色</a> , <a href="#" onclick="logout()" style="color:royalblue">注销</a>');
                                setTimeout(function () {
                                    parent.layer.close(index); //再执行关闭  
                                }, 1500);
                                 
                                
                            } else
                            {
                                //登陆失败
                            }
                            
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            });
        }
    </script>
</body>
</html>
