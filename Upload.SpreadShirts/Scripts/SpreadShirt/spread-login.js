﻿$(function () {
    Spread.Controls.CreateControls();
})

var Spread = {
    Controls: {
        CreateControls: function () {
            if (sessionStorage.initialFiles === undefined) {
                sessionStorage.initialFiles = "[]";
                sessionIndexUpload = 0;
            } else {
                sessionStorage.initialFiles = "[]";
                sessionIndexUpload = 0;
            }
            var initialFiles = JSON.parse(sessionStorage.initialFiles);

            keypressNumberOnly({ ctrlID: '#txtPrice' });

            $("#btnLogin").on('click', function (e) { Spread.Events.LoginServerSpread(e) });
            createKendoUpload({
                ctrlID: "fInputFile"
                , onSuccess: Spread.Events.ChangeLoadFile
            })
        },
        LoadAllShop: function (allData) {
            var html = '<div class="col-md-3">';
            html += '       <div class="displayShop">';
            html += '           <div class="row">';
            html += '               <div class="col-md-8"><span><b>ID:</b> <small>' + allData.ShopID + '</small></span></div>';
            html += '               <div class="col-md-4">';
            html += '                   <label class="switch">';
            html += '                       <input type="checkbox" class="select-item-shop" value="' + allData.ShopID + '">';
            html += '                       <span class="slider round"></span>';
            html += '                   </label>';
            html += '               </div>';
            html += '           </div>';
            html += '           <hr />';
            html += '           <h3 class="text-align-center">' + allData.ShopName + '</h3>';
            html += '       </div>';
            html += '</div>';

            $("#show-shop").append(html);
        },
        Filter: function () {
            var Image = "C:\\Users\\HoangLe\\Desktop\\Tool up spreadshirt\\Upload\\98.png";
            var Price = $("#txtPrice").val() == "" ? 0 : parseFloat($("#txtPrice").val()).toFixed(2);
            var Title = EncodeVietNamese($("#txtName").val());
            var Description = EncodeVietNamese($("#txtDescription").val());
            var Tag = EncodeVietNamese($("#txtTag").val());
            var Shop = EncodeVietNamese(Spread.Events.ShopServerSpread());

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
            var username = $("#txtUsername").val();// "hoangbap1595@gmail.com";
            var password = $("#txtPassword").val();//"Thienan@111";
            enableButton({ ctrlID: "btnLogin", disabled: true });
            getDataObject({
                url: "SpreadShirt/ExecuteLogin"
               , type: "POST"
               , filter: { username: username, password: password }
               , onSuccess: function (data) {
                   if (data.result == 1) {
                       alert(data);
                       Spread.Events.WriteLog({ data: "Login success!", work: 1 })
                       $("#modal-default").modal('toggle');
                       var htmUpload = '<button class="btn btn-success" style="width:150px;" id="btnUpload">Upload</button>';
                       $("#btnShowLogin").remove();
                       $("#action-button").append(htmUpload);
                       $("#btnUpload").on('click', function (e) { Spread.Events.UploadServerSpread(e) });
                       $("#show-shop").empty();
                       var shops = JSON.parse(data.shop);
                       for (var i = 0; i < shops.list.length; i++) {

                           var ShopID = shops.list[i].target.id;
                           var ShopName = shops.list[i].name;
                           if (ShopID != 93439)
                               Spread.Controls.LoadAllShop({ ShopID: ShopID, ShopName: ShopName });
                       }
                   } else {
                       alert(data);
                       enableButton({ ctrlID: "btnLogin", disabled: false });
                   }
               }
                  , onError: function (error) {
                      alert(error);
                  }
            })
        },
        UploadServerSpread: function (e) {
            var filter = Spread.Controls.Filter();
            var dataImage = JSON.parse(sessionStorage.initialFiles);
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
                alert({ title: "Message", icon: "warning", message: "Please enter field tag more than 3 keyword (Ex: tag1, tag2, tag3)" });
                return
            }
            if (filter.Price > 20 || filter.Price < 0) {
                alert({ title: "Message", icon: "warning", message: "Please enter price between $00.00 and $20.00" });
                return
            }
            if (dataImage.length < 1) {
                alert({ title: "Message", icon: "warning", message: "Please choose image design !" });
                return
            }

            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
            Spread.Events.ProgressUpload({ filter: filter });
        },
        ProgressUpload: function (allData) {
            var dataImage = JSON.parse(sessionStorage.initialFiles);
            if (sessionIndexUpload >= dataImage.length)
                return;
            var imageName = dataImage[sessionIndexUpload].name;
            Spread.Events.WriteLog({ data: "Uploading: " + imageName, work: 3 })
            var filter = allData.filter;
            filter.Image = imageName;
            filter.Title = filter.Title.replace("$name", imageName.split('.')[0])
            filter.Description = filter.Description.replace("$name", imageName.split('.')[0])
            enableButton({ ctrlID: "btnUpload", disabled: true });
            getDataObject({
                url: "SpreadShirt/UploadProgress"
               , type: "POST"
               , filter: filter
               , onSuccess: function (data) {
                   Spread.Events.WriteLog({ data: data.data, work: 1 })
                   sessionIndexUpload++; //Upload next Image
                   Spread.Events.ProgressUpload({ filter: filter });
                   enableButton({ ctrlID: "btnUpload", disabled: false });
               }
                  , onError: function (error) {
                      //console.log(error);
                      Spread.Events.WriteLog({ data: error, work: 2 })
                      sessionIndexUpload++; //Upload next Image
                      Spread.Events.ProgressUpload({ filter: filter });
                      enableButton({ ctrlID: "btnUpload", disabled: false });
                  }
            })
        },
        ChangeLoadFile: function (e) {
            var currentInitialFiles = JSON.parse(sessionStorage.initialFiles);
            for (var i = 0; i < e.files.length; i++) {
                var current = {
                    name: e.files[i].name,
                    extension: e.files[i].extension,
                    size: e.files[i].size
                }

                if (e.operation == "upload") {
                    currentInitialFiles.push(current);
                } else {
                    var indexOfFile = currentInitialFiles.indexOf(current);
                    currentInitialFiles.splice(indexOfFile, 1);
                }
            }
            sessionStorage.initialFiles = JSON.stringify(currentInitialFiles);
        },
        ShopServerSpread: function () {
            var items = [];
            var values = "";
            $("input.select-item-shop:checked:checked").each(function (index, item) {
                items[index] = item.value;
            });
            if (items.length < 1) {
                values = "";
            } else {
                values = items.join(',');
                console.log(values);
            }
            return values;
        },
        WriteLog: function (allData) {
            var log = $("#show-progress");
            var date = new Date();
            var d_Time = '[' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds() + ']';
            var d_Work = parseInt(allData.work);

            var data = '<li>' + d_Time + ' <span>' + allData.data + '</span>.............................@work</li>';
            switch (d_Work) {
                case 1:
                    data = data.replace("@work", "Done");
                    break;
                case 2:
                    data = data.replace("@work", "Faild");
                    break;
                case 3:
                    data = data.replace("@work", "InProgress");
                    break;
            }

            log.prepend(data);
        }
    }
}
