function regionSelectTreeObject() {
    this.createTree = function (treeId, targetTextbox, targetHidden, onClick) {
        //树状结构的对象
        var zTree1;
        //树属性的设定
        var setting;
        var strAsyncUrl = "/Region/TreeExpand"; 											//异步参数不需要，直接指定为对象的Guid        
        var strLoadUrl = "/Region/TreeLoad";
        var strTreeId = treeId;
        var textbox = $("#" + targetTextbox);
        var hidden = $("#" + targetHidden);

        //初始化显示
        if (hidden.val() != "") {
            $.post("/Resource/GetRegionAddress", { gid: hidden.val() },
            function (data) {
                var name = data.toString();
                textbox.val(name);
            });
        }

        //设定数结构的参数
        setting = {
            editable: false, 															    //树不可删除
            edit_renameBtn: false, 															//编辑按钮不可用，不需要用户指定
            edit_removeBtn: false, 															//删除按钮不可用，不需要用户指定
            checkable: false, 															    //用户指定是否需要checkbox，需要用户指定
            checkType: { "Y": "", "N": "" },                                                //树结构的勾选框不影响父，不影响子
            checkedCol: "nodeChecked",                                                      //自定义勾选框状态的字段名
            async: true, 																	//默认值，不需要用户指定
            asyncUrl: strAsyncUrl, 															//用户指定url
            asyncParam: ["id"], 															//用户指定参数
            callback: {
                click: zTreeOnClick
            }
        };

        //获取树节点信息
        $.ajax({
            type: "POST",
            url: strLoadUrl,
            success:
			    function (data) {
			        //将获取的json字符串转换为json对象
			        var testdata = jQuery.parseJSON(data);
			        //生成树状结构
			        zTree1 = $("#" + strTreeId).zTree(setting, testdata);
			    }
	    });

        //点击树节点事件
        function zTreeOnClick(event, treeId, treeNode) {
            if (treeNode.id != "00000000-0000-0000-0000-000000000000") {
                //将选中的城市赋值到对应INPUT中，只赋城市，以防网络延迟
                $("#" + targetTextbox).val(treeNode.name);
                $("#" + targetHidden).val(treeNode.id);
                $("#" + treeId).fadeOut("fast");
                $.post("/Resource/GetRegionAddress", { gid: treeNode.id },
                    function (data) {
                        var name = data.toString();
                        //获取完整地址，赋值INPUT
                        $("#" + targetTextbox).val(name);
                        if (onClick != undefined && onClick != "")
                            eval(onClick + "(treeNode.id, name);");
                    });
            }
        }
        //绑定TEXTBOX点击弹出地区选择框
        $("#" + targetTextbox).click(function () {
            var txtBox = $("#" + targetTextbox);
            var cityOffset = $("#" + targetTextbox).offset();
            $("#" + treeId).css({ left: cityOffset.left + "px", top: cityOffset.top + txtBox.outerHeight() + "px" }).slideDown();
        });
        //点击其他地方隐藏菜单
        $(function () {
            $("body").bind("mousedown",
			        function (event) {
			            if (!(event.target.id == targetTextbox || event.target.id == treeId || $(event.target).parents("#" + treeId).length > 0)) {
			                $("#" + treeId).fadeOut("fast");
			            }
			        });
        });
    };
}