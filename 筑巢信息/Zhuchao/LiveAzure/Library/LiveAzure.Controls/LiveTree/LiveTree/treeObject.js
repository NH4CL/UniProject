function treeObject() {

    var tree;
    var treesetting;
    var currentProgUrl;
    this.selectNode;

    this.createTree = function (treeId, loadUrl, asyncUrl, changeUrl, rmenuId, checkable) {
        //��״�ṹ�Ķ���
        var zTree1;

        //�����Ե��趨
        var setting;

        var bCheckable = checkable;
        var strAsyncUrl = asyncUrl;
        //var strAsyncParam = asyncParam;													//�첽��������Ҫ��ֱ��ָ��Ϊ�����Guid        
        var strRmenuId = rmenuId;
        var strChangeUrl = changeUrl;
        var strLoadUrl = loadUrl;
        var strTreeId = treeId;

        //�趨���ṹ�Ĳ���
        setting = {
            editable: false, 															    //������ɾ��
            edit_renameBtn: false, 															//�༭��ť�����ã�����Ҫ�û�ָ��
            edit_removeBtn: false, 															//ɾ����ť�����ã�����Ҫ�û�ָ��
            checkable: bCheckable, 															//�û�ָ���Ƿ���Ҫcheckbox����Ҫ�û�ָ��
            keepParent: true, 																//Ĭ��ֵ������Ҫ�û�ָ��
            keepLeaf: true, 																//Ĭ��ֵ������Ҫ�û�ָ��
            async: true, 																	//Ĭ��ֵ������Ҫ�û�ָ��
            asyncUrl: strAsyncUrl, 															//�û�ָ��url
            asyncParam: ["id"], 															//�û�ָ������
            callback: {
                rightClick: rightClick, 												    //�Ҽ��¼��Ƿ���ã�����editable�趨����Ҫ�û�ָ��ҳ���Ҽ��˵�id
                change: zTreeOnChange, 													//���checkableΪtrue������Ҫָ��url��
                click: zTreeOnClick
            }
        };

        //��ȡ���ڵ���Ϣ
        $.ajax({
            type: "GET",
            url: strLoadUrl,
            success:
				function (data) {
				    //����ȡ��json�ַ���ת��Ϊjson����
				    var testdata = jQuery.parseJSON(data);
				    //������״�ṹ
				    zTree1 = $("#" + strTreeId).zTree(setting, testdata);
				    //�趨ȫ�ֱ������������ڲ�������������
				    tree = zTree1;
				    treesetting = setting;
				}
        });

        //���Ҽ�
        $(document).ready(function () {
            //��ȡ�Ҽ��˵�
            var rMenu = document.getElementById(rmenuId);
            //���Ҽ��˵�������div�е��ʱ�����Ҽ��˵�����
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

        //�û���checkbox��״̬�ı�����
        function zTreeOnChange(event, treeId, treeNode) {
            $.ajax({
                type: "POST",
                url: strChangeUrl,
                data: { id: treeNode.id }
            });
        }

        //ָ�����ٲ�����ڵ㲻����չ��������Ϊ10
        function beforeAsync(treeId, treeNode) {
            if (treeNode.level > 10) {
                return false;
            }
            return true;
        }

        //������ڵ��¼�
        function zTreeOnClick(event, treeId, treeNode) {
            currentProgUrl = treeNode.progUrl;
            this.selectNode = treeNode;
            //to do 

        }

        //�һ����ɲ˵�
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

        //��ʾ�Ҽ��˵�
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
	
    //������ڵ�
    this.add_TreeNode = function (rmenuId) {
        document.getElementById(rmenuId).style.visibility = "hidden";
        document.getElementById("m_add").style.visibility = "hidden";
        document.getElementById("m_edit").style.visibility = "hidden";
        document.getElementById("m_delete").style.visibility = "hidden";

        this.selectNode = tree.getSelectedNode();
    }

    //�༭���ڵ�
    this.edit_TreeNode = function (rmenuId) {
        document.getElementById(rmenuId).style.visibility = "hidden";
        document.getElementById("m_add").style.visibility = "hidden";
        document.getElementById("m_edit").style.visibility = "hidden";
        document.getElementById("m_delete").style.visibility = "hidden";

        this.selectNode = tree.getSelectedNode();
    }

    //ɾ��ָ���Ľڵ�
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
                //ɾ���ɹ�֮�����¼������ڵ�
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