function OpenPanel() {
    this.panelparentnode;//默认添加到body下
    this.contain;
    this.hand;
    this.bLoading = true;
    this.width;
    this.height;
    this.title;
    this.closeurl;

    this.CloseEditPanel = function () {
        document.body.removeChild(document.getElementById("newEditPageDiv"));
        document.body.removeChild(document.getElementById("newCoverPageDiv"));
    };

    this.EditPageHtml = function (html) {

        $("#partialpage").html(html);
    }

    this.OpenPanel = function () {
        var panelParentNode = document.body; //document.getElementById(this.panelparentnode);
        var newEditPageDiv = document.createElement("div");
        newEditPageDiv.setAttribute("id", "newEditPageDiv");
        newEditPageDiv.setAttribute("class", "EditPage");
        var newCoverPageDiv = document.createElement("div");
        newCoverPageDiv.setAttribute("id", "newCoverPageDiv");
        newCoverPageDiv.setAttribute("class", "CoverPage");
        newEditPageDiv.innerHTML = "<p style=\"background-color: Orange; width: 100%; height: 30px; margin-top: 0;\" class=\"Draggalbe\">" +
        "<span class=\"Title\">" + this.title + "</span>" +
        "<span class=\"Close\" onclick=\"ClosePanel();\" style=\"float:right;\">" +
        "<img alt=\"close\" src=\"" + this.closeurl + "\" /></span>" +
        "</p><div id=\"partialpage\" class=\"PartialPage\"></div>";
        panelParentNode.appendChild(newEditPageDiv);
        panelParentNode.appendChild(newCoverPageDiv);

        //draggable
        $("#newEditPageDiv").draggable({ handle: this.hand == null ? "p" : "#" + this.hand,
            containment: this.contain == null ? "#frame_index_div" : "#" + this.contain,
            scroll: false,
            cursor: "move",
            opacity: 0.7,
            axis: false
        });
        //遮罩层
        var newCoverPageDiv = document.getElementById("newCoverPageDiv");
        var newEditPageDiv = document.getElementById("newEditPageDiv");
        newCoverPageDiv.style.width = "100%";
        newCoverPageDiv.style.height = document.getElementById("frame_index_div").offsetHeight + "px";
        //定义窗口
        newEditPageDiv.setAttribute("style", "width:" + this.width + "px;height:" + this.height + "px;position:absolute;");
        newEditPageDiv.style.display = newCoverPageDiv.style.display = "block";
        if (this.bLoading == true) {
            document.getElementById("partialpage").innerHTML = "Loading...";
        }
    }
}
function ClosePanel() {
    document.body.removeChild(document.getElementById("newEditPageDiv"));
    document.body.removeChild(document.getElementById("newCoverPageDiv"));
}

/*************************原始方法（暂时保留）******************************/
function CloseEditPanel(coverpage, editpage) {
    var coverpage = document.getElementById(coverpage);
    var editpage = document.getElementById(editpage);
    coverpage.style.display = "none";
    editpage.style.display = "none";
};
function OpenEditPanel(coverpage, editpage, partialpage, contain, hand, bLoading) {//遮罩层，编辑块，内容块，拖动范围，拖动区域
    //draggable
    $("#" + editpage).draggable({ handle: hand == null ? "p" : "#" + hand,
        containment: contain == null ? "body" : "#" + contain,
        scroll: false,
        cursor: "move",
        opacity: 0.7
    });
    var coverpage = document.getElementById(coverpage);
    var editpage = document.getElementById(editpage);
    //遮罩层
    coverpage.style.width = document.getElementById('body_frame').offsetWidth + "px";
    coverpage.style.height = document.getElementById('body_frame').offsetHeight + "px";
    //定义窗口
    editpage.style.marginTop = -75 + document.documentElement.scrollTop + "px";
    editpage.style.display = coverpage.style.display = "block";
    if (bLoading == true) {
        document.getElementById(partialpage).innerHTML = "Loading...";
    }
};