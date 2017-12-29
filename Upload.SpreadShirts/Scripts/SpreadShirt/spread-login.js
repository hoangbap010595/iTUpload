$(function () {
    Spread.Controls.CreateControls();
})

var Spread = {
    Controls: {
        CreateControls: function () {
            sessionStorage.initialFiles = "[]";
            var sessionStorageData = [];
            var initialFiles = JSON.parse(sessionStorage.initialFiles);
            keypressNumberOnly({ ctrlID: '#txtPrice' });

            $("#btnLogin").on('click', function (e) { Spread.Events.LoginServerSpread(e) });
            $("#btnUpload").on('click', function (e) { Spread.Events.UploadServerSpread(e) });
            $("#btnUploadUsingFile").on('click', function (e) { Spread.Events.UploadServerSpreadUseFile(e) });
            createKendoUpload({
                ctrlID: "fInputFile"
                , onSuccess: Spread.Events.ChangeLoadFile
            })
            createKendoUpload({
                ctrlID: "fInputFileData"
                , allowedExt: ThemeConfig.uploadExcelExt
                , async: {
                    saveUrl: "upload/saveexcel",
                    removeUrl: "upload/removeexcel",
                    autoUpload: true
                }
                , onSuccess: Spread.Events.OnSuccessLoadFileData
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
                       var htmUpload = '<button class="k-button k-primary" style="width:150px;" id="btnUpload">Upload</button>';
                       var htmUploadFile = '<button class="k-button k-primary" style="width:150px;" id="btnUploadUsingFile">Upload</button>';
                       $(".btnShowLogin").remove();
                       $("#action-button").append(htmUpload);
                       $("#action-button-file").append(htmUploadFile);
                       $("#btnUpload").on('click', function (e) { Spread.Events.UploadServerSpread(e) });
                       $("#btnUploadUsingFile").on('click', function (e) { Spread.Events.UploadServerSpreadUseFile(e) });
                       $("#show-shop").empty();
                       var uID = data.UID;
                       $(".lblID").html(uID);
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
            $("#imgUploading").removeAttr("hidden");
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
            enableButton({ ctrlID: "btnUpload", disabled: true });
            Spread.Events.ProgressUpload({ sessionIndexUpload: 0 });
        },
        ProgressUpload: function (allData) {
            var dataImage = JSON.parse(sessionStorage.initialFiles);
            var index = allData.sessionIndexUpload;
            var filter = Spread.Controls.Filter();
            try { 
                if (index >= dataImage.length) {
                    $("#imgUploading").attr("hidden", "hidden");
                    sessionStorage.initialFiles = "[]";
                    enableButton({ ctrlID: "btnUpload", disabled: false });
                    return;
                }
                var imageName = dataImage[index].name;
                Spread.Events.WriteLog({ data: "Uploading: " + imageName, work: 3 })
                //Config filter
                filter.Image = imageName;
                filter.Title = filter.Title.replace("$name", imageName.split('.')[0])
                filter.Description = filter.Description.replace("$name", imageName.split('.')[0]);
                getDataObject({
                    url: "SpreadShirt/UploadProgress"
                   , type: "POST"
                   , filter: filter
                   , onSuccess: function (data) {
                       Spread.Events.WriteLog({ data: data.data, work: 1 })
                       index++; //Upload next Image
                       Spread.Events.ProgressUpload({ sessionIndexUpload: index });
                   }
                      , onError: function (error) {
                          //console.log(error);
                          Spread.Events.WriteLog({ data: error, work: 2 })
                          index++; //Upload next Image
                          Spread.Events.ProgressUpload({ sessionIndexUpload: index });
                      }
                })
            } catch (error) {
                Spread.Events.WriteLog({ data: error, work: 2 });
                index++; //Upload next Image
                Spread.Events.ProgressUpload({ sessionIndexUpload: index });
            }
        },
        UploadServerSpreadUseFile: function (e) {
            var dataUpload = sessionStorageData;
            if (dataUpload.length < 1) {
                alert({ title: "Message", icon: "warning", message: "Please choose file data import!" });
                return
            }
            $("#imgUploading").removeAttr("hidden");
            enableButton({ ctrlID: "btnUploadUsingFile", disabled: true });
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
            Spread.Events.ProgressUploadUseFile({ sessionIndexUpload: 0 });
        },
        ProgressUploadUseFile: function (allData) {
            var dataUpload = sessionStorageData;
            var index = allData.sessionIndexUpload;
            var filter = Spread.Controls.Filter();
            try {
                if (index >= dataUpload.length) {
                    $("#imgUploading").attr("hidden", "hidden");
                    enableButton({ ctrlID: "btnUploadUsingFile", disabled: false });
                    return;
                }
                var imageName = dataUpload[index].Image;
                var title = dataUpload[index].Title;
                var description = dataUpload[index].Description;
                var tag = dataUpload[index].Tag;
                var shop = dataUpload[index].Shop;
                var price = dataUpload[index].Price;
                Spread.Events.WriteLog({ data: "Uploading: " + imageName, work: 3 })
                //Config filter
                filter.Image = imageName;
                filter.Title = title;
                filter.Description = description;
                filter.Tag = tag;
                filter.Shop = shop;
                filter.Price = price;
                getDataObject({
                    url: "SpreadShirt/UploadProgress"
                   , type: "POST"
                   , filter: filter
                   , onSuccess: function (data) {
                       Spread.Events.WriteLog({ data: data.data, work: 1 })
                       index++; //Upload next Image
                       Spread.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
                   }
                      , onError: function (error) {
                          //console.log(error);
                          Spread.Events.WriteLog({ data: error, work: 2 })
                          index++; //Upload next Image
                          Spread.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
                      }
                })
            } catch (error) {
                Spread.Events.WriteLog({ data: error, work: 2 });
                index++; //Upload next Image
                Spread.Events.ProgressUploadUseFile({ sessionIndexUpload: index });
            }
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
            var d_Time = '[' + (parseInt(date.getHours()) < 0 ? '0' : '') + date.getHours() + ':' + (parseInt(date.getMinutes()) < 0 ? '0' : '') + date.getMinutes() + ':' + (parseInt(date.getSeconds()) < 0 ? '0' : '') + date.getSeconds() + ']';
            var d_Work = parseInt(allData.work);

            var data = '<li class="@cWork">' + d_Time + ' ' + allData.data + '.............................@work</li>';
            switch (d_Work) {
                case 1:
                    data = data.replace("@work", "Done").replace("@cWork", "in-done");
                    break;
                case 2:
                    data = data.replace("@work", "Faild").replace("@cWork", "in-error");
                    break;
                case 3:
                    data = data.replace("@work", "InProgress").replace("@cWork", "in-progress");
                    break;
            }

            log.prepend(data);
        },
        OnSuccessLoadFileData: function (e) {
            try {
                var fileName = Spread.Events.GetFileInfo(e);
                var upload = $("#fInputFileData").data("kendoUpload");
                var files = upload.getFiles();

                /* set up XMLHttpRequest */
                var parentFolder = "/Uploaded/FileUpload/";
                var url = parentFolder + fileName;
                var oReq = new XMLHttpRequest();
                oReq.open("GET", url, true);
                oReq.responseType = "arraybuffer";

                oReq.onload = function (e) {
                    var arraybuffer = oReq.response;

                    /* convert data to binary string */
                    var data = new Uint8Array(arraybuffer);
                    var arr = new Array();
                    for (var i = 0; i != data.length; ++i)
                        arr[i] = String.fromCharCode(data[i]);
                    var bstr = arr.join("");

                    /* Call XLSX */
                    var workbook = XLSX.read(bstr, { type: "binary" });

                    /* DO SOMETHING WITH workbook HERE */
                    var first_sheet_name = workbook.SheetNames[0];
                    /* Get worksheet */
                    var worksheet = workbook.Sheets[first_sheet_name];

                    var data = XLSX.utils.sheet_to_json(worksheet);
                    console.log(data);
                    sessionStorageData = data;
                    Spread.Events.WriteLog({ data: "Open file success! " + data.length + " record(s)", work: 1 })
                }

                oReq.send();
            } catch (error) {
                sessionStorageData = [];
            }
        },
        GetFileInfo: function (e) {
            return $.map(e.files, function (file) {
                var info = file.name;
                return info;
            }).join(", ");
        }
    }
}