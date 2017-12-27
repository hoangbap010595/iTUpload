$(function () {
    Spread.Controls.CreateControls();
})

var Spread = {
    Controls: {
        CreateControls: function () {
            keypressNumberOnly({ ctrlID: '#txtPrice' });

            $("#login").on('click', function (e) { Spread.Events.LoginServerSpread(e) });
            $("#upload").on('click', function (e) { Spread.Events.UploadServerSpread(e) });

        },
        Filter: function () {
            var Image = "C:\\Users\\HoangLe\\Desktop\\Tool up spreadshirt\\Upload\\98.png";
            var Price = $("#txtPrice").val() == "" ? 0 : parseFloat($("#txtPrice").val()).toFixed(2);
            var Title = EncodeVietNamese($("#txtName").val());
            var Description = EncodeVietNamese($("#txtDescription").val());
            var Tag = EncodeVietNamese($("#txtTag").val());
            var Shop = EncodeVietNamese($("#txtShop").val());

            var d = {
                Image: Image
                   , Price: Price
                   , Title: Title
                   , Description: Description
                   , Tag: Tag
                   , Shop: Shop
            }
            return d;
        }
    },
    Events: {
        LoginServerSpread: function (e) {
            enableButton({ ctrlID: "login", disabled: true });
            getDataObject({
                url: "SpreadShirt/ExecuteLogin"
               , type: "POST"
               , filter: { username: "hoangbap1595@gmail.com", password: "Thienan@111" }
               , onSuccess: function (data) {
                   if (data.result == 1) {
                       alert(data);
                   } else {
                       alert(data);
                       enableButton({ ctrlID: "login", disabled: false });
                   }
               }
                  , onError: function (error) {
                      alert(error);
                  }
            })
        },
        UploadServerSpread: function (e) {
            var filter = Spread.Controls.Filter();
            if (filter.Title == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field name!" });
                return
            }
            if (filter.Description == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field description!" });
                return
            }
            if (filter.Tag == "") {
                alert({ title: "Message", icon: "warning", message: "Please enter field tag!" });
                return
            } else if (filter.Tag.split(',').length < 3) {
                alert({ title: "Message", icon: "warning", message: "Please enter field tag more than 3 keyword (Ex: tag1,tag2,tag3)" });
                return
            }

            //enableButton({ ctrlID: "upload", disabled: true });
            //getDataObject({
            //    url: "SpreadShirt/UploadProgress"
            //   , type: "POST"
            //   , filter: filter
            //   , onSuccess: function (data) {
            //       if (data.result == 1) {
            //           //data.fnExecute = function () { R.events.clickRefresh() };
            //           //alertOK(data);
            //           console.log(data.data);
            //       } else {
            //           alert(data.data);
            //           enableButton({ ctrlID: "upload", disabled: false });
            //       }
            //   }
            //      , onError: function (error) {
            //          alert(error);
            //      }
            //})
        },
        ChangeLoadFile: function () {
            var files = $('#fInputFile').prop("files");
            var names = $.map(files, function (val) { return val.name; });
            $.each(names, function (i, name) {
                console.log(name);
                console.log(files[i]);
            });

            var a = $('#fInputFile').val();
            console.log(a);
        }
    }
}
