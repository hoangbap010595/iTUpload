$(function () {

    $("#login").bind('click', function () {
        enableButton({ ctrlID: "login", disabled: true });
        getDataObjectAjax({
            url: "SpreadShirt/ExecuteLogin"
           , method: "POST"
           , filter: { username: "lchoang1995@gmail.com", password: "Thienan@111" }
           , onSuccess: function (data) {
               if (data.result == 1) {
                   data.fnExecute = function () { R.events.clickRefresh() };
                   alertOK(data);
               } else {
                   alert(data);
                   enableButton({ ctrlID: "login", disabled: false });
               }
           }
              , onError: function (error) {
                  alert(error);
              }
        })
    })
})


