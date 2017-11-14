var dfUrl = "/Account/GetListUser";

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
                ctrID: "data-table-users"
                , url: { url: dfUrl }
                , refix: { key: 2, id: "Id" }
                , colDefs: [
                    { "sClass": "col-ident-id", "aTargets": [0] }
                    , { "aTargets": [1], "bVisible": false }
                    , { "sClass": "col-bit", "aTargets": [4] }
                    , { "sClass": "text-left", "aTargets": [5], "sWidth": "140px" }
                    , { "sClass": "col-bit", "aTargets": [6] }
                    , { "sClass": "col-actions-2", "aTargets": [7] }
                ]
                , columns: [
                      { "sTitle": "STT", "mData": "Stt" },
                      { "sTitle": "Id", "mData": "Id" },
                      { "sTitle": "Username", "mData": "UserName" },
                      { "sTitle": "Email", "mData": "Email" },
                      { "sTitle": "EmailActive", "mData": "EmailConfirmed" },
                      { "sTitle": "Phone", "mData": "PhoneNumber" },
                      { "sTitle": "PhoneActive", "mData": "PhoneNumberConfirmed" },
                      { "sTitle": "Actions", "mData": "Actions" }
                ]
                , fnRowCallback: function (nRow, aData, iDisplayIndex) {
                    if (aData.EmailConfirmed == true) {
                        $('td:eq(3)', nRow).html('<b>Ok</b>');
                    } else
                        $('td:eq(3)', nRow).html('');
                    if (aData.PhoneNumberConfirmed == true) {
                        $('td:eq(5)', nRow).html('<b>Ok</b>');
                    } else
                        $('td:eq(5)', nRow).html('');
                    return nRow;
                }
            }
            return d;
        }
    },
    events: {
        trashAction: function (data) {
            alert({ message: data.data });
        },
        editAction: function (data) {
            alert({ message: data.data, icon: "success" });
        },
        detailsAction: function (data) {
            alertConfirm({
                title: "Thông báo!"
                , message: data.data
                , icon: "info"
                , fnExecute: function () {
                    R.events.editAction(data.data);
                }
            })
        }
    }
};


