function treeObject() {

    var tree;
    var treesetting;
    var currentProgUrl;
    this.selectNode;

    this.createTree = function (treeId, loadUrl, asyncUrl, changeUrl, rmenuId, checkable) {
        //树状结构的对象
        var zTree1;

        //树属性的设定
        var setting;

        var bCheckable = checkable;
        var strAsyncUrl = asyncUrl;
        //var strAsyncParam = asyncParam;													//异步参数不需要，直接指定为对象的Guid        
        var strRmenuId = rmenuId;
        var strChangeUrl = changeUrl;
        var strLoadUrl = loadUrl;
        var strTreeId = treeId;

        //设定数结构的参数
        setting = {
            editable: false, 															    //树不可删除
            edit_renameBtn: false, 															//编辑按钮不可用，不需要用户指定
            edit_removeBtn: false, 															//删除按钮不可用，不需要用户指定
            checkable: bCheckable, 															//用户指定是否需要checkbox，需要用户指定
            keepParent: true, 																//默认值，不需要用户指定
            keepLeaf: true, 																//默认值，不需要用户指定
            async: true, 																	//默认值，不需要用户指定
            asyncUrl: strAsyncUrl, 															//用户指定url
            asyncParam: ["id"], 															//用户指定参数
            callback: {
                rightClick: rightClick, 												    //右键事件是否可用，根据editable设定，需要用户指定页面右键菜单id
                change: zTreeOnChange, 													//如果checkable为true，则需要指定url。
                click: zTreeOnClick
            }
        };

        //获取树节点信息
        $.ajax({
            type: "GET",
            url: strLoadUrl,
            success:
				function (data) {
				    //将获取的json字符串转换为json对象
				    var testdata = jQuery.parseJSON(data);
				    //生成树状结构
				    zTree1 = $("#" + strTreeId).zTree(setting, testdata);
				    //设定全局变量，给对象内部其它函数调用
				    tree = zTree1;
				    treesetting = setting;
				}
        });

        //绑定右键
        $(document).ready(function () {
            //获取右键菜单
            var rMenu = document.getElementById(rmenuId);
            //绑定右键菜单，当在div中点击时，将右键菜单隐藏
            $("div").bind("mousedown",
				function (event) {
				    if (!(event.target.id == rmenuId || $(event.target).parents("#" + rmenuId).length > 0)) {
				        rMenu.style.visibility = "hidden";
				        document.getElementById("m_add").style.visibility = "hidden";
				        document.getElementById("m_edit").style.visibility = "hidden";
				        document.getElementById("m_delete").style.visibility = "hidden";
				    }
				});
        });

        //用户将checkbox的状态改变后调用
        function zTreeOnChange(event, treeId, treeNode) {
            $.ajax({
                type: "POST",
                url: strChangeUrl,
                data: { id: treeNode.id }
            });
        }

        //指定多少层的树节点不能再展开，参数为10
        function beforeAsync(treeId, treeNode) {
            if (treeNode.level > 10) {
                return false;
            }
            return true;
        }

        //点击树节点事件
        function zTreeOnClick(event, treeId, treeNode) {
            currentProgUrl = treeNode.progUrl;
            this.selectNode = treeNode;
            //to do 

        }

        //右击生成菜单
        function rightClick(event, treeId, treeNode) {
            if (!treeNode && event.target.tagName.toLowerCase() != "button" && $(event.target).parents("a").length == 0) {
                tree.cancelSelectedNode();
                showRMenu("root", event.clientX, event.clientY);
            } else if (treeNode) {
                tree.selectNode(treeNode);
                if (treeNode.name == "root") {
                    showRMenu("root", event.clientX, event.clientY);
                }
                else {
                    showRMenu("node", event.clientX, event.clientY);
                }
            }
        }

        //显示右键菜单
        function showRMenu(type, x, y) {
            $("#" + strRmenuId + " ul").show();
            document.getElementById("m_add").style.visibility = "visible";
            document.getElementById("m_edit").style.visibility = "visible";
            document.getElementById("m_delete").style.visibility = "visible";
            if (type == "root") {
                document.getElementById("m_edit").style.visibility = "hidden";
                document.getElementById("m_delete").style.visibility = "hidden";
            }
            else {
                document.getElementById("m_edit").style.visibility = "visible";
                document.getElementById("m_delete").style.visibility = "visible";
            }
            $("#" + strRmenuId).css({ "top": y + "px", "left": x + "px", "visibility": "visible" });
        }

    };
	
    //添加树节点
    this.add_TreeNode = function (rmenuId) {
        document.getElementById(rmenuId).style.visibility = "hidden";
        document.getElementById("m_add").style.visibility = "hidden";
        document.getElementById("m_edit").style.visibility = "hidden";
        document.getElementById("m_delete").style.visibility = "hidden";

        this.selectNode = tree.getSelectedNode();
    }

    //编辑树节点
    this.edit_TreeNode = function (rmenuId) {
        document.getElementById(rmenuId).style.visibility = "hidden";
        document.getElementById("m_add").style.visibility = "hidden";
        document.getElementById("m_edit").style.visibility = "hidden";
        document.getElementById("m_delete").style.visibility = "hidden";

        this.selectNode = tree.getSelectedNode();
    }

    //删除指定的节点
    this.del_TreeNode = function (treeId, removeUrl, reloadUrl, rmenuId) {
        document.getElementById(rmenuId).style.visibility = "hidden";
        document.getElementById("m_add").style.visibility = "hidden";
        document.getElementById("m_edit").style.visibility = "hidden";
        document.getElementById("m_delete").style.visibility = "hidden";

        var treeNode = tree.getSelectedNode();
        this.selectNode = tree.getSelectedNode();
        var truthBeTold = window.confirm("\u786e\u5b9a\u8981\u5220\u9664" + treeNode.name + "?");
        if (truthBeTold) {
            $.ajax({
                type: "POST",
                url: removeUrl,
                data: { id: treeNode.id },
                success:
                //删除成功之后，重新加载树节点
						function (data) {
						    $.ajax({
						        type: "GET",
						        url: reloadUrl,
						        success:
								 function (data) {
								     var testdata = jQuery.parseJSON(data);
								     tree = $("#" + treeId).zTree(treesetting, testdata);
								 }
						    });
						}
            });

            return true;
        }
        else {
            return false;
        }
    }

}