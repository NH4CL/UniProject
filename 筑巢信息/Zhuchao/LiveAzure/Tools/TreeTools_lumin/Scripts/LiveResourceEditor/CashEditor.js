function initCurrencyName(id) {
    var editor = $("#" + id);
    var textboxes = editor.find("input[type=text]");
    textboxes.each(
        function (i) {
            showCurrencyName(editor, this);
        });
}
function showCurrencyName(editor ,textbox) {
    var objID = $(textbox).attr("id");
    var lbl = editor.find("label[for=" + objID + "]");
    var n = objID.indexOf("Cash");
    var head = objID.substring(0, n);
    var guid = $("#" + head + "Currency").val();
    $.post("/Resource/GetCurrencyName", { gid: guid },
        function (name) {
            lbl.text(name.toString());
        });
}