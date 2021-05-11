<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BindProject.aspx.cs" Inherits="AchiWeb.UserLogin.BindProject" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../layui/css/layui.css" rel="stylesheet" media="all" />
    <script src="../layui/layui.js"></script>
    <script src="../layui/lay/jquery-3.3.1.min.js"></script>
    <script src="../layui/jquery.cookie.js"></script>
    <title></title>
</head>
<body>
<form class="layui-form" action="" lay-filter="example">
    <br /><br /><br />
  <div class="layui-form-item">
    <label class="layui-form-label">选择框</label>
    <div class="layui-input-block" style="width:200px;">
      <select id="checkall" name="interest" type1="flow_select" lay-filter="checkProjcet">
      </select>
        <br /><br /><br /><br /><br /><br />
        <div class="layui-form-item" style="text-align:center">
            <button type="button" onclick="bind()" class="layui-btn layui-btn-normal layui-btn-radius">绑定角色</button>
        </div>
    </div>
  </div>
</form>
          
<script>
layui.use(['form'], function(){
  var form = layui.form
  ,layer = layui.layer
    var jsonstr = $.cookie('jsonstr');
    if (jsonstr != null) {
        var js = $.parseJSON(jsonstr);
        var userid = js.Data;
        var html = '';
        
        for (var i = 0; i < js.Data.Data.length; i++) {

            if (i == 0) 
                html = '<option value="' + js.Data.Data[i].PCID + '" selected="selected">' + js.Data.Data[i].Name + '</option>';
             else 
                html = '<option value="' + js.Data.Data[i].PCID + '">' + js.Data.Data[i].Name + '</option>';
            
            $('#checkall').prepend(html);
        }
        form.render('select');
    }
  
});
</script>
    <script type="text/javascript">
        function bind() {
            var themeid = $("[name='interest']").val()
            var themename = $("[name='interest']").children("[value=" + themeid + "]").text()
            $.cookie('PCID', themeid, { expires: 7, path: '/', secure: false });
            $.cookie('PcidName', themename, { expires: 7, path: '/', secure: false });
            var acction = $.cookie('Acction');
            layui.use('layer', function () {
                var layer = layui.layer;
                
                var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                layer.msg('绑定成功！');
                setTimeout(function () {
                    parent.$('#login_div').html('<label>亲爱的灵芝&nbsp ' + acction + '  &nbsp [' + themename + '] &nbsp&nbsp <a href="#" onclick="bindProjcet()" style="color:royalblue">更改绑定</a>&nbsp&nbsp<a href="javascript:void(0);" onclick="logout()" style="color:royalblue">注销</a>');
                    parent.layer.close(index); //再执行关闭  
                }, 1000);

                
            });
        }
    </script>
</body>
</html>
