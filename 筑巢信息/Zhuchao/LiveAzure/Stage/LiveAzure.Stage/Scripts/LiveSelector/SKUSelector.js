function initSKUSelect(orgID, targetID) {
    var url = "/Resource/GetSKUIntelliSense";
    var initUrl = "/Resource/GetSKUName";
    var textboxID = targetID + "_txt";
    var textbox = $("#" + textboxID);
    var valueID = targetID + "_val";
    var hidden = $("#" + valueID);
    var menuID = targetID + "_menu";
    var menu = $("#" + menuID);
    var selectTemplate = $("#SkuSelectTemplate");
    this.doSearchSKU = function () {
        if (textbox.val() == "") {
            menu.fadeOut();
        }
        else {
            var searching = $("#SkuSearching");
            var newSearching = searching.clone(true);
            newSearching.appendTo(menu);
            newSearching.show();
            var menuOffset = textbox.offset();
            menu.css({ left: menuOffset.left + "px", top: menuOffset.top + textbox.outerHeight() + "px" });
            menu.show();
            $.ajax({
                type: "post",
                url: url,
                data: { orgID: orgID, input: textbox.val() },
                async: false,
                success: function (data) {
                    menu.empty();
                    if (data.length == 0) {
                        var noResult = $("#SkuNoResult").clone(true);
                        noResult.appendTo(menu);
                        noResult.show();
                    }
                    else {
                        $.each(data,
                                function (index, obj) {
                                    var newItem = selectTemplate.clone(true);
                                    newItem.removeAttr("id");
                                    var hidSkuID = newItem.find("#hidSkuID");
                                    var txtSkuName = newItem.find("#txtSkuName");
                                    var txtSkuCode = newItem.find("#txtSkuCode");
                                    hidSkuID.val(obj.Gid);
                                    txtSkuName.text(obj.Name);
                                    txtSkuCode.text(obj.Code);
                                    newItem.appendTo(menu);
                                    newItem.show();
                                });
                    }
                }
            });
        }
    }
    //初始化显示
    if (hidden.val() != "") {
        $.post(initUrl, { skuID: hidden.val() },
            function (data) {
                var name = data.toString();
                textbox.val(name);
            });
    }
    selectTemplate.click(function () {
        var hidSkuID = $(this).find("#hidSkuID");
        var txtSkuName = $(this).find("#txtSkuName");
        var txtSkuCode = $(this).find("#txtSkuCode");
        hidden.val(hidSkuID.val());
        textbox.val(txtSkuName.text());
        menu.fadeOut("fast");
    });
    textbox.keyup(function () {
        doSearchSKU();
    });

    textbox.change(function () {
        textbox.val("");
        hidden.val("");
    });
    //点击其他地方隐藏菜单
    $("body").bind("mousedown",
			function (event) {
			    if (!(event.target.id == textboxID || event.target.id == menuID || $(event.target).parents("#" + menuID).length > 0)) {
			        menu.fadeOut("fast");
			    }
			});
}