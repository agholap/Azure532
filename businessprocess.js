// JavaScript source code
//var interval = null;
function setBPFTooltips() {
    //interval = setInterval(function () {
    var toolTips = [
        { "field": "parentcontactid", "text": "Tooltip for contact ID" },
        { "field": "parentaccountid", "text": "Tooltip for account ID" },
        { "field": "purchasetimeframe", "text": "Tooltip for purchasetime frame" }
    ];
    for (var i in toolTips) {
        Xrm.Page.getAttribute(toolTips[i].field).controls.foreach(function (a, b) { a.setLabel(toolTips[i].text)});
    }
    // }, 1000);
}