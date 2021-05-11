
function loginbtn() {
    layui.use('layer', function () {
        var layer = layui.layer;
        layer.open({
            title: '[剑灵]-账号登录',
            type: 2,
            area: ["360px", "330px"],
            content: ['/UserLogin/Login.aspx' + '?API=' + $('#API').html(), 'no']
        });
    });
}
function bindProjcet() {
    layui.use('layer', function () {
        var layer = layui.layer;
        layer.open({
            title: '[剑灵]角色绑定',
            type: 2,
            area: ["400px", "300px"],
            content: ['/UserLogin/BindProject.aspx', 'no']
        });
    });
}
function logout() {
    layui.use('layer', function () {
        var layer = layui.layer;
        $.cookie('Acction', '', { expires: -1, path: '/' });
        $.cookie('Password', '', { expires: -1, path: '/' });
        $.cookie('jsonstr', '', { expires: -1, path: '/' });
        $.cookie('PCID', '', { expires: -1, path: '/' });
        $.cookie('PcidName', '', { expires: -1, path: '/' });
        $('#login_div').html('<label>尊敬的灵芝,请</label><a href="javascript:void(0);" onclick="loginbtn()" style="color:royalblue">登录</a>');
        layer.msg("注销成功");
    });
}
function getitem() {
    layui.use('layer', function () {
        var layer = layui.layer;
        var acction = $.cookie('Acction');
        var pcid = $.cookie('PCID');
        if (acction == null) {

            loginbtn();
            layer.msg('请先登录账号哦！');
        } else if (pcid == null) {
            bindProjcet();
            layer.msg('请先绑定角色！~');
        } else if (acction != null && pcid != null) {

            var API = $('#API').html();
            var GUID = $('#GUID').html();
            var data = '{"Acction":"' + acction + '","PCID":"' + pcid + '","GUID":"' + GUID + '"}';
            var url = 'http://' + API+'/activity/Achievement.aspx';
            $.ajax({
                //提交方式
                //方法所在路径
                url: url,
                type: 'Post',
                data: data,
                //数据类型
                contentType: 'application/x-www-form-urlencoded',
                //将数据转成JSon格式
                //接收返回值的方法
                success: function (result) {
                    if (result != null) {
                        var str = '<br /><br /><br />';
                        if (result == 'Already') {//已经领取过
                            str += '您已经领取过了哦~';
                        } else if (result == 'Not activity') {//活动不符 未开始或已结束
                            str += '活动暂未开启或已结束！';
                        } else if (result == 'Unqualified') {//不满足要求
                            str += '您还没满足要求哦！';
                        } else if (result == 'OK') {
                            str += '领取成功 道具将在24小时内发送至您的礼品盒,请注意查收！';
                        } else {
                            str = '系统出现错误';
                        }
                        //str += '<br /><br />';
                        //layer.msg(str, {
                        //    time: 10000, //20s后自动关闭
                        //    btn: ['确定']
                        //});
                        layer.open({
                            title: '[T1] 领取结果',
                            type: 1,
                            area: ["260px", "220px"],
                            content: str,
                            btnAlign: 'c', btn: '好的',
                            yes: function () {
                                layer.closeAll();
                            }
                        });
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }

    });
}