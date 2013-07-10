function RefIDSelector() {
    this.textID;
    this.valueID;
    this.errorEvent;
    this.refType;
    this.init = function (id, onError) {
        textID = id + "_txt";
        valueID = id + "_val";
        errorEvent = onError;
        $("#" + textID).blur(function () {
            if ($("#" + textID).val() != "") {
                $.post("/Resource/GetNoteGid", { refType: refType, code: $("#" + textID).val() },
                        function (data) {
                            var gid = data.toString();
                            if (gid == "00000000-0000-0000-0000-000000000000") {
                                if(errorEvent != "" && errorEvent != null)
                                    eval(errorEvent + "();");
                                $("#" + textID).val("");
                            } else {
                                $("#" + valueID).val(gid);
                            }
                        });
            }
        });
    }
    this.create = function (type) {
        refType = type;
        $("#" + textID).removeAttr("disabled");
    }
    this.destroy = function () {
        $("#" + textID).val("");
        $("#" + valueID).val("");
        $("#" + textID).attr("disabled", "disabled");
    }
}