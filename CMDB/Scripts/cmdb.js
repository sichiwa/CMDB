//比較新舊物件
function compareReviewData() {
    $(".divnew input").each(function (i, elm) {
        var newName = $(elm).attr("name");
        var oldName = "o" + $(elm).attr("name");

        //alert("newName:" + newName);
        //alert("oldName:" + oldName);

        if (newName == "Creator") {
            oldName = "oUpadter";
        }
        else if (newName == "CreateTime") {
            oldName = "oUpadteTime";
        }

        //新物件值
        var val = $(elm).val();
        //舊物件值
        var val2 = $(".divold input[name=" + oldName + "]").val();

        //比較後如果不一致則加入紅色CSS
        if (val != val2) {
            if (newName != "Type") {
                //alert("oldName:" + oldName);
                $(elm).addClass("diff");
            }
        }
    });
}

//取得屬性選單
function getAttributeDropList(AttributeID) {
    $.getJSON("/Attribute/getAttributeList", function (data) {
        $("#AttributeDrop option").remove();
        $.each(data, function (i, opt) {
            //alert("aaaa");
            var ImputElement = "";
            if (AttributeID != "" && opt.AttributeID == AttributeID) {
                ImputElement = "<option selected class=" + opt.AttributeTypeName + "></option>";
                $(ImputElement).val(opt.AttributeID).text(opt.AttributeName).appendTo($("#AttributeDrop"));
            }
            else {
                ImputElement = "<option class=" + opt.AttributeTypeName + "></option>";
                $(ImputElement).val(opt.AttributeID).text(opt.AttributeName).appendTo($("#AttributeDrop"));
            }
        });
    });
}

//取得屬性輸入控制項
function getAttributeInputList(ProfileID) {
    //alert("aaaa");
    //從Server端取回資料
    $.getJSON("/Object/getAttributeInputList", 'ProfileID=' + ProfileID, function (data) {
        $(".AttrInput").remove();
        var ImputElement = "";
        $.each(data, function (i, opt) {
            //alert("bbbb");
            if (ImputElement != "") {
                ImputElement = ImputElement + "<div class='form-group AttrInput'>" +
                                             "<label class='control-label col-md-2'>" + opt.AttributeName + "</label> " +
                                             " <div class='col-md-10'>";
            }
            else {
                ImputElement = "<div class='form-group AttrInput'>" +
                                             "<label class='control-label col-md-2'>" + opt.AttributeName + "</label> " +
                                             " <div class='col-md-10'>";
            }
          
           // alert(ImputElement);
            //一般文字輸入
            if (opt.AttributeTypeID == 1) {
                ImputElement = ImputElement + "<input type='text' id='" + opt.AttributeID + "' class='form-control AttrInputField'></input>";
            }
                //一般文字輸入(多行)
            else if (opt.AttributeTypeID == 2) {
                ImputElement = ImputElement + "<textarea  id='" + opt.AttributeID + "' class='form-control AttrInputField' rows='4' cols='50'></textarea >";
            }
                //日期輸入
            else if (opt.AttributeTypeID == 3) {
                ImputElement = ImputElement + "<input type='text' id='" + opt.AttributeID + "' class='form-control date-picker AttrInputField'></input>";
            }
                //下拉式選單輸入
            else if (opt.AttributeTypeID == 4) {
                //alert(opt.DropDownValues);
                ImputElement = ImputElement + "<select id='" + opt.AttributeID + "' class='form-control AttrInputField'>";
                var DropValues = opt.DropDownValues.split("#");
                //alert(DropValues[x]);
                //組合選項
                for (var x in DropValues) {
                    ImputElement = ImputElement + "<option  value='" + DropValues[x] + "' > " + DropValues[x] + "</option>";
                }
                ImputElement = ImputElement + "</select>";
            }
                //未知輸入
            else {

            }

            ImputElement = ImputElement + "</div></div>";
            //alert(ImputElement);
        });
        // alert(ImputElement);
        //將控制項加入表單
        $(".Attr").after(ImputElement);
        $(".date-picker").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "yy-mm-dd"
        });
        //$(".date-picker").datepicker("setDate", new Date());
    });
   
 
}
