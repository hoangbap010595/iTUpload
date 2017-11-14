var dfUrl;

$(document).ready(function () {
    dfUrl = BC.dfUrl;
    var d = R.fun.loadGrid();
    createGridDataTable(d);
});
var R = {
    k: 1,
    data: {},
    fun: {
        loadGrid: function () {
            var d = {
                ctrID: "data-table-category"
                , url: { url: dfUrl, spName: "Category_Get", jsonFilter: {a: "a", b: "b", c: "c"} }
                , refix: { key: 1, id: "Id" }
                , colDefs: [
                    { "sClass": "col-ident-id", "aTargets": [0]}
                    , { "aTargets": [1], "bVisible": false }
                    , { "sClass": "text-left", "aTargets": [2] }
                    , { "sClass": "col-bit", "aTargets": [4] }
                    , {
                        "sClass": "col-date", "aTargets": [5]
                    }
                    , { "sClass": "col-actions-1", "aTargets": [6] }
                ]
                , columns: [
                      { "sTitle": "STT", "mData": "Stt" }
                      , { "sTitle": "Id", "mData": "Id" }
                      , { "sTitle": "Name", "mData": "Name" }
                      , { "sTitle": "Name Clean", "mData": "NameClean" }
                      , { "sTitle": "Enable", "mData": "Enable" }
                      , { "sTitle": "Date Create", "mData": "DateCreate"}
                      , { "sTitle": "Actions", "mData": "Actions" }
                ]
                , fnFooterCallback: function (nFoot, aData, iStart, iEnd, aiDisplay) {
                    //nFoot.getElementsByTagName('th')[0].innerHTML = "Starting index is " + iStart;
                }
                ,fnRowCallback: function (nRow, aData, iDisplayIndex) {
                    var jsonDate = aData.DateCreate;
                    $('td:eq(4)', nRow).text(parseDate(jsonDate));
                    return nRow;
                }
            }
            return d;
        }
    },
    events: {
        trashAction: function (data) {
            alert({ data: data.data });
        },
        editAction: function (data) {
            alert({ data: data.data });
        }
    }
};
