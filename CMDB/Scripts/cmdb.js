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
        //alert("val:" + val);
        //oldName=""
        //舊物件值
        var val2 = $(".divold input[name='" + oldName + "']").val();
        //alert("val2:" + val2);
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
                ImputElement +="<div><div class='form-group AttrInput' >" +
                                                        "<label class='control-label col-md-2'>" + opt.AttributeName + "</label> " +
                                             " <div class='col-md-10'>";
            }
            else {
                ImputElement = "<div><div class='form-group AttrInput' >" +
                                                        "<label class='control-label col-md-2'>" + opt.AttributeName + "</label> " +
                                             " <div class='col-md-10'>";
            }
          
            // alert(ImputElement);
            //一般文字輸入
            if (opt.AttributeTypeID == 1) {
                ImputElement = ImputElement + "<input type='text' id='" + opt.AttributeID + "' class='form-control AttrInputField'></input>";

                //判斷此屬性是否支援多值
                if (opt.AllowMutiValue) {
                    //加入新增與刪除按鈕
                    ImputElement = ImputElement + " <button type='button' class='add btn btn-primary'>新增</button> "+
                                                                                    "<button type='button' class='del btn btn-danger' disabled='disabled'>刪除</button>";
                }
            }
                //一般文字輸入(多行)
            else if (opt.AttributeTypeID == 2) {
                ImputElement = ImputElement + "<textarea  id='" + opt.AttributeID + "' class='form-control AttrInputField' rows='4' cols='50'></textarea >";
                //判斷此屬性是否支援多值
                if (opt.AllowMutiValue) {
                    //加入新增與刪除按鈕
                    ImputElement = ImputElement + " <button type='button' class='add btn btn-primary'>新增</button> "+
                                                                                    "<button type='button' class='del btn btn-danger' disabled='disabled'>刪除</button>";
                }
            }
                //日期輸入
            else if (opt.AttributeTypeID == 3) {
                ImputElement = ImputElement + "<input type='text' id='" + opt.AttributeID + "' class='form-control date-picker AttrInputField'></input>";

                //判斷此屬性是否支援多值
                if (opt.AllowMutiValue) {
                    //加入新增與刪除按鈕
                    ImputElement = ImputElement + " <button type='button' class='add btn btn-primary'>新增</button>"+
                                                                                    "<button type='button' class='del btn btn-danger' disabled='disabled'>刪除</button>";
                }
            }
                //下拉式選單輸入
            else if (opt.AttributeTypeID == 4) {
                ImputElement = ImputElement + "<select id='" + opt.AttributeID + "' class='form-control AttrInputField'>";
                var DropValues = opt.DropDownValues.split("#");
                //組合選項
                for (var x in DropValues) {
                    ImputElement = ImputElement + "<option  value='" + DropValues[x] + "' > " + DropValues[x] + "</option>";
                }
                ImputElement = ImputElement + "</select>";
                //判斷此屬性是否支援多值
                if (opt.AllowMutiValue) {
                    //加入新增與刪除按鈕
                    ImputElement = ImputElement + " <button type='button' class='add btn btn-primary'>新增</button>"+
                                                                                    "<button type='button' class='del btn btn-danger' disabled='disabled'>刪除</button>";
                }
            }
                //未知輸入
            else {

            }
            ImputElement = ImputElement + "</div></div></div>";
        });
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

//取得關係範本菜單
function getRelationshipProfileMenu(ProfileID) {
    $.getJSON("/Object/getRelationshipProfileMenu", 'ProfileID=' + ProfileID, function (data) {
        var menuElement = "";
        var contentElement = "";
        var searchdiv = "<br/><div class='row'> ";
        searchdiv += "      <div class='col-md-12' id='serachdiv'>";
        searchdiv += "          <div class='row form-group'>"
        searchdiv += "              <label  class='col-md-1 control-label'>物件名稱</label>"
        searchdiv += "              <input id='objectnameinput' type='text' Class = 'form-control' placeholder='請輸入物件名稱'  value=''/>";
        searchdiv += "          </div>";
        searchdiv += "          <div class='row form-group'>"
        searchdiv += "              <input type='button' class='col-md-offset-1 btn btn-primary' id='searchbtn' value='查詢' />"
        searchdiv += "         </div>";
        searchdiv += "    </div>";
        searchdiv += "</div>";

        $.each(data, function (i, opt) {
            if (menuElement != "" && contentElement!="") {
                menuElement = menuElement + "<li><a data-toggle='tab' href=#menu" + opt.RelationshipProfileID + ">" + opt.ProfileName + "</a></li>";
                contentElement = contentElement + "<div id=menu" + opt.RelationshipProfileID + " class='tab-pane fade'>" + searchdiv + "</div>"
            }
            else {
                menuElement = "<li><a data-toggle='tab' href=#menu" + opt.RelationshipProfileID + ">" + opt.ProfileName + "</a></li>";
                contentElement = "<div id=menu" + opt.RelationshipProfileID + " class='tab-pane fade'>" + searchdiv + "</div>"
            }
        });

        //將控制項加入表單
        $("#mainli").siblings().remove();
        $("#mainli").after(menuElement);
        $("#maindiv").append(contentElement);

        $("#maindiv").on('click', "#searchbtn", function () {
            var target = $("a[aria-expanded='true']").attr("href"); // activated tab
            //alert(target);
            var ProfileID = target.replace("#menu", "");
            var ObjectName = $("#objectnameinput").val();
            getObjects(target, ProfileID, ObjectName, searchdiv);
        });

        $(document).on('click', "#addbtn", function () {
            //alert("aaaa");
            var ObjectRelationshipTable = "<table class='table table-bordered' id='objectrelationshiptb'>";
            ObjectRelationshipTable += " <tr class='active'>";
            ObjectRelationshipTable += "<th>物件名稱</th>";
            ObjectRelationshipTable += "<th>移除</th>";
            ObjectRelationshipTable += "</tr>";
            var ObjectRelationsRows = "";

            $('.sourcetb').find('input[type="checkbox"]:checked').each(function () {
                //this is the current checkbox
                var target = $("a[aria-expanded='true']").attr("href"); // activated tab
                var RelationshipProfileID = target.replace("#menu", "");
                var CheckBoxID = this.id;
                var RelationshipObjectID = CheckBoxID.replace("check", "");
                var RelationshipObjectName = $("#" + CheckBoxID).parent().parent().parent().siblings().text();
                //alert(RelationshipObjectName);
               
                ObjectRelationsRows += "<tr class='datatr'>";
                ObjectRelationsRows += "        <td class='col-md-1'>" + RelationshipObjectName + "</td>";
                ObjectRelationsRows += "        <td class='col-md-1'>";
                ObjectRelationsRows += "                <input type='hidden' id=hidden" + RelationshipObjectID + " value=" + RelationshipObjectID + " class='RelationshipObjectID'>";
                ObjectRelationsRows += "                <input type='hidden' id=hidden" + RelationshipProfileID + " value=" + RelationshipProfileID + " class='RelationshipProfileID'>"
                ObjectRelationsRows += "                <input type='button' value='移除' class='btn btn-danger' id='removebtn'>";
                ObjectRelationsRows += "        </td>";
                ObjectRelationsRows += "</tr>";
            });

            if (ObjectRelationsRows.length > 0) {
                ObjectRelationshipTable += ObjectRelationsRows;
            }

            ObjectRelationshipTable += "</table>";
            //alert(ObjectRelationsRows);

            var len = $("#objectrelationshiptb").length;

            if (len > 0) {
                //alert("bbbb");
                $("#objectrelationshiptb .datatr").remove();
                $("#objectrelationshiptb").append(ObjectRelationsRows);
            }
            else {
                $(".ObjectRelationship").html(ObjectRelationshipTable);
            }
            alert("加入關係成功!!");
        });

        $(document).on('click', "#removebtn", function () {
            //alert("cccc");
            var RelationshipID = $(this).siblings().val();
            $("#check" + RelationshipID).prop("checked", false);
            $(this).parent().parent().remove();
        });

        $('a[data-toggle="tab"]').on('click', function (e) {
            var target = $(e.target).attr("href"); // activated tab
            var ProfileID = target.replace("#menu", "");
            var ObjectName = $("#objectnameinput").val();
            if ($(target).is(':empty')) {
                
            }
            else {
                if (target != "#home") {
                    getObjects(target, ProfileID, ObjectName, searchdiv);
                }
            }
        });
    });

    function getObjects(target, ProfileID, ObjectName, searchdiv) {
        $.ajax({
            type: "POST",
            url: "/Object/getObjectDatafromName",
            contextType: "text/html; charset=utf-8",
            data: { ProfileID: ProfileID, ObjectName: ObjectName },
            error: function (data) {
                alert("There was a problem");
            },
            success: function (data) {
                //取得套用特定Profile的物件清單，組合成Table
                var ObjectTable = "<div class='row'><table class='table table-bordered sourcetb' id='sourcetb'>";
                ObjectTable += " <tr class='active'>";
                ObjectTable += "<th>選擇</th>";
                ObjectTable += "<th>物件名稱</th>";
                ObjectTable += "<th>描述</th>";
                ObjectTable += "</tr>";

                var ObjectRows = "";

                $.each(data, function (i, opt) {
                    //組合Row
                    ObjectRows += "<tr>";
                    ObjectRows += "<td class='col-md-2'>" +
                                                        "<div class='checkbox'> " +
                                                                "<label><input type='checkbox' value='' id=check" + opt.ObjectID + ">選擇</label>" +
                                                        "</div>" +
                                                  "</td>";
                    ObjectRows += "<td class='col-md-4'>" + opt.ObjectName + "</td>";
                    ObjectRows += "<td class='col-md-6'>" + $.trim(opt.Description) + "</td>";
                    ObjectRows += "</tr>";
                });

                if (ObjectRows.length > 0) {
                    ObjectTable += ObjectRows;
                }

                ObjectTable += "</table></div>";

                ObjectTable += "<div class='row' ><input type='button' class='btn btn-primary' id='addbtn' value='建立關係' /></div>"

                //將Table加入到Tab
                $(target).html(searchdiv + ObjectTable);
            }
        });
    }
}
