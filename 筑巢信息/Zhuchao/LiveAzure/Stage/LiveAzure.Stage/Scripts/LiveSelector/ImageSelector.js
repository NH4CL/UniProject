function initImageSelector(menuID, rootURL, targetTextbox, targetHidden, targetImg) {
    var url = "/Resource/GetImageNames";
    var menu = $("#" + menuID);
    var template = menu.find("#Template");
    var list = menu.find("ul");
    var textBox = $("#" + targetTextbox);
    var hidden = $("#" + targetHidden);
    var selectedImg = $("#" + targetImg);
    textBox.attr("readonly", "readonly");   //将文本框置为只读
    $.post(url,{ rootURL: rootURL },
        function (data) {
            $.each(data, function (index, obj) {
                //复制新节点
                var newItem = template.clone(true);
                newItem.removeAttr("id");
                var Img = newItem.find("img");
                var Info = newItem.find("span");
                var ImgUrl = rootURL + "/" + obj;
                Img.attr("src", ImgUrl);
                Img.attr("title", obj);
                Img.attr("alt", obj);
                Info.text(obj);
                //绑定选择图片事件
                newItem.click(function () {
                    hidden.val(Img.attr("src"));
                    textBox.val(Info.text());
                    selectedImg.attr("src", hidden.val());
                    selectedImg.attr("title", Info.text());
                    selectedImg.attr("alt", Info.text());
                    menu.fadeOut("fast");
                });
                newItem.appendTo(list);
                newItem.fadeIn();
            });
            //绑定点击文本框事件
            textBox.click(function () {
                var menuOffset = textBox.offset();
                menu.css({ left: menuOffset.left + "px", top: menuOffset.top + textBox.outerHeight() + "px" }).slideDown();
            });
            //点击其他地方隐藏菜单
            $(function () {
                $("body").bind("mousedown",
			        function (event) {
			            if (!(event.target.id == targetTextbox || event.target.id == menuID || $(event.target).parents("#" + menuID).length > 0)) {
			                menu.fadeOut("fast");
			            }
			        });
            });
        });
    }