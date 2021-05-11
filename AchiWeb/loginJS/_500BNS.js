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
function GetDrawData()
{
    var GUID = $('#GUID').html();//活动GUID
    var acction = $.cookie('Acction');
    var url = 'http://' + $('#API').html() + '/getItem/GetDrawMsg.aspx?GUID=' + GUID;
    $.ajax({
        //提交方式
        //方法所在路径
        url: url,
        type: 'get',
        //接收返回值的方法
        success: function (result) {
            if (result != null) {
                var js = $.parseJSON(result);
                var js1 = $.parseJSON(js.GralData);
                if (js.GUID != null && js.GUID != "") {
                    $('#DrawDiv').html('<label>' + js.DrawName + '</label>');
                    $('#DrawTimer').html('<label>活动时间 ' + js.Start_Timer.replace("T", " ") + ' - ' + js.Due_Timer.replace("T", " ") + '</label>');
                    for (var i = 0; i < js1.GralData.length; i++) {
                        $('#fdText').html('<label> 花费' + js1.GralData[i].jf + '积分兑换 <br />' + js1.GralData[i].Item_Name + '</label>');
                        $('#divbtn').html('<button type="button" onclick="gral(\'' + js1.GralData[i].GUID + '\',\''+GUID+'\')" class="layui-btn layui-btn-lg layui-btn-norma layui-btn-warm btn">立即兑换</button>')
                        break;
                    }
                    $('#fd1div').html('<label>' + js.F_Msg + '</label>');
                    $('#fd2div').html('<label>' + js.Msg + '</label>');
                }
            } 
        },
        error: function (e) {
            console.log(e);
        }
    });
    $.ajax({
        //提交方式
        //方法所在路径
        url: "http://"+$('#API').html()+"/getItem/GetGralALL.aspx?GUID=" + GUID,
        type: 'get',
        //数据类型
        //contentType: 'application/x-www-form-urlencoded',
        //将数据转成JSon格式
        //接收返回值的方法
        success: function (result) {
            if (result != null) {
                var js = $.parseJSON(result);
                $('#_1dc').html('<label>' + js[0].Number + '钻石</label>');
                $('#_1lc').html('<label>' + js[0].DoubleNumber + '钻石</label>');
                $('#_1dc_btn').html('<a href="javascript:void(0);" onclick="GetItem(\'' + js[0].GUID + '\',\'' + GUID+'\',\'1\')" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>');
                $('#_1lc_btn').html('<a href="javascript:void(0);" onclick="GetItem(\'' + js[0].GUID + '\',\'' + GUID +'\',\'11\')" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>');

                $('#_2dc').html('<label>' + js[1].Number + '钻石</label>');
                $('#_2lc').html('<label>' + js[1].DoubleNumber + '钻石</label>');
                $('#_2dc_btn').html('<a href="javascript:void(0);" onclick="GetItem(\'' + js[1].GUID + '\',\'' + GUID +'\',\'1\')" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>');
                $('#_2lc_btn').html('<a href="javascript:void(0);" onclick="GetItem(\'' + js[1].GUID + '\',\'' + GUID +'\',\'11\')" style="font-size:40px">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</a>');

            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    if (acction != null) {
        $.ajax({
            //提交方式
            //方法所在路径
            url: "http://" + $('#API').html() +"/getItem/GetUserMsg.aspx?DrawGUID=" + GUID + "&Acction=" + acction,
            type: 'get',
            //数据类型
            //contentType: 'application/x-www-form-urlencoded',
            //将数据转成JSon格式
            //接收返回值的方法
            success: function (result) {
                if (result != null) {
                    var js = $.parseJSON(result);
                    $.cookie('#Balance', js.Balance, { expires: 1, path: '/', secure: false });
                    $('#bal').html('<label>' + js.Balance + '</label>');
                    $('#cz').html('<label><a href="javascript:void(0);" onclick="cz(\'' + GUID + '\')" style="color:#FFB800">充值</a></label>');
                    $('#hd').html('<label><a href="javascript:void(0);" onclick="hd(\'' + GUID + '\')" style="color:#fff">我的物品</a></label>');
                    $('#wdjf').html('<label>我的积分' + js.quota + '</label>');
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    
}

function gral(GUID,DrawGUID)
{
    layui.use('layer', function () {
        var layer = layui.layer;
        var Acction = $.cookie('Acction');
        var data = '{"GUID":"' + GUID + '","DrawGUID":"' + DrawGUID + '","Acction":"' + Acction + '"}';
        $.ajax({
            //提交方式
            //方法所在路径
            url: "http://" + $('#API').html() +"/getItem/Gral.aspx",
            type: 'POST',
            data: data,
            //数据类型
            //contentType: 'application/x-www-form-urlencoded',
            //将数据转成JSon格式
            //接收返回值的方法
            success: function (result) {
                var js = $.parseJSON(result);
                
                layer.open({
                    title: '兑换结果',
                    type: 1,
                    area: ["260px", "220px"],
                    content: '<br /><br /><br /><div style="text-align:center;color:#000000"><label>' + js.msg +'</label></div>',
                    btnAlign: 'c', btn: '好的',
                    yes: function () {
                        layer.closeAll();
                    }
                });
                
            },
            error: function (e) {
                layer.msg('Error'+e);
            }
        });

        
    });
    
}

function cz(GUID)
{
    var Acction = $.cookie('Acction');
    var GUID = $('#GUID').html();//活动GUID
    if (Acction != null) {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.open({
                title: '[剑灵]-充值钻石',
                type: 2,
                area: ["500px", "500px"],
                content: ['/UserLogin/UserPay.aspx?Acction=' + Acction+'&GUID='+GUID+'&API='+$('#API').html(), 'no']
            });
        });
    }
}

function hd(GUID)
{
    var acction = $.cookie('Acction');
    var GUID = $('#GUID').html();//活动GUID
    if (acction != null) {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.open({
                title: '获得的物品',
                type: 2,
                area: ["500px", "500px"],
                content: ['/UserLogin/UserGetItem.aspx?Acction=' + acction + '&GUID=' + GUID + '&API=' + $('#API').html(), 'no']
            });
        });
    }
}

function GetItem(GUID, DrawGUID, Number,cout)
{
    layui.use('layer', function () {
        var layer = layui.layer;
        var Acction = $.cookie('Acction');
        var data = '{"GUID":"' + GUID + '","DrawGUID":"' + DrawGUID + '","Acction":"' + Acction + '","Number":"' + Number+'"}';
        $.ajax({
            //提交方式
            //方法所在路径
            url: "http://" + $('#API').html() +"/getItem/RandomItem.aspx",
            type: 'POST',
            data: data,
            //数据类型
            //contentType: 'application/x-www-form-urlencoded',
            //将数据转成JSon格式
            //接收返回值的方法
            success: function (result) {
                var js = $.parseJSON(result);
                if (js.Code==0) {
                    layer.open({
                        title: '获得以下物品',
                        content: '<br /><div style="text-align:center;color:#000000"><label>' + js.Msg + '</label></div>',
                    });
                } else {
                    layer.msg(js.Msg);
                }
                

            },
            error: function (e) {
                layer.msg('Error' + e);
            }
        });


    });
}
