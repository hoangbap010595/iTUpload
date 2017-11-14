var dfUrl = "/Account/GetListRoles";
$(document).ready(function () {
    var d = R.fun.loadGrid();
    createGridDataTable(d);
});
var R = {
    k: 1,
    data: {},
    fun: {
        loadGrid: function () {
            var d = {
                ctrID: "data-table-rules"
                , url: { url: dfUrl }
                , refix: { key: 1, id: "Id" }
                , colDefs: [
                   { "sName": "Stt", "sClass": "col-ident-id", "aTargets": [0] },
                    { "sClass": "col-actions-1", "aTargets": [3] }
                ]
                , columns: [
                       { "sName": "Stt", "sTitle": "STT", "mData": "Stt" },
                         { "sTitle": "ID", "mData": "Id" },
                         { "sTitle": "Name", "mData": "Name" },
                         { "sTitle": "Actions", "mData": "Actions" }
                ]
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



