$(
    function () {

        $("#accordConfig").accordion({ heightStyle: "content" });

        $(".Agent").click(function () {

            $(this).parent().find(".CounterpartySettings").slideToggle();
            $(this).parent().find(".CounterpartySettingsIntro").accordion({ heightStyle: "content" });
     
        });
        $(".ChangeSettings").click(function () {

            var counterpartyId = $(this).parent().parent().parent().data("id");
            var analytics = new Array();
            $(this).parent().parent().find("input[name='AnalyticTypes']:checked").each(function () {
                analytics.push($(this).val());
            });

            var performers = new Array();
            $(this).parent().parent().find("input[name='Manager']:checked").each(function () {
                performers.push($(this).val());
            });

            var sources = new Array();
            $(this).parent().parent().find("input[name='Source']:checked").each(function () {
                sources.push($(this).val());
            });
            
            var model = { counterpartyId: counterpartyId, analytics: analytics, performers: performers, sources: sources };
            $.ajax({
                url: "/Config/ChangeSettingsCounterparty",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                success: function (result) {
                    alert(result);
                },
                error: function (result) {
                    alert(result.responseText);
                }
            });
                
        });


        $(".ObjMeta").delegate(".DelDesk", "click", function () {
            var res = confirm("Увага! Після видалення атрибуту,всі записи пов'язані з цим атрибутом будуть видалені!");
            if (!res) {
                return false;
            }
            $(this).parent().remove();
        });

        $("#AddAtrr").click(function () {
            var newAtrr = $("input[name='NewAtrr']").val();
            $(".ObjMeta").append(`
                               <tr>
                                <td><input type='text' value='${newAtrr}' name='Desk' data-id='${0}' /></td><td class='DelDesk'><i class='fas fa-times'></i></td>
                                </tr>
                             `);
        });

        $("#saveDesk").click(
            function () {
                var DeskIds = new Array();
                var DeskNames = new Array();
                var objId = $("#ObjId").val();

                $("input[name='Desk']").each(
                    function () {
                        DeskIds.push($(this).data('id'));
                        DeskNames.push($(this).val());
                    }
                );
        
                $.ajax({
                    url: "/Config/EditDesk", type: "POST", traditional: true, dataType: "json", data: { Obj: objId, DeskIds: DeskIds, DeskNames: DeskNames }
                });
             
                location.reload();
            }
        );

        $(".PropsArr").delegate(".DelProp", "click", function () {
            var id = $(this).parent().data("id");
            var del = $(this).parent();
            var conf = confirm("Увага! Ви видаляєте реквізит.");
            if (!conf) {
                return false;
            }
            $.post("/Config/DelProp", { Id: id }).done(function () {
               del.remove();
            });           
        });

        $(".SourceArr").delegate(".DelSource", "click", function () {
            var id = $(this).parent().data("id");
            var del = $(this).parent();
            var conf = confirm("Увага! Ви видаляєте джерело.");
            if (!conf) {
                return false;
            }
            $.post("/Config/DelSource", { Id: id }).done(function () {
                del.remove();
            }); 
        });
        $(".BranchArr").delegate(".DelBranch", "click", function () {
            var Id = $(this).parent().data('id');
            var del = $(this).parent();
            $.post("/Config/DelBranch", { Id:Id}).done(
                function () {
                  
                    del.remove();
                }
            );
        });
        $("#AddBranch").click(
            function () {
                var SourceId = $("#SourceId :selected").val();
                var BranchName = $(".BranchName").val();
                var SourceName = $('#SourceId :selected').text();
                $.post("/Config/AddBranch", { SourceId: SourceId, BranchName: BranchName }).done(function (data) {
                    $(".BranchArr").append(`
                    <div class="Branch row" data-id="${data}">
                <div class="col-md-5">${SourceName}</div>
                <div class="col-md-5">${BranchName}</div>
                <div class="col-md-2 DelBranch">
                    <i class="fas fa-times"></i>
                </div>
            </div>
                    `);
                });
            }
        );
        $("#AddSource").click(
            function () {
                var Content = $(".SourceName").val();
                if (Content == "" || Content == null) {
                    alert("Заповніть поле назви джерела");
                    return false;
                }
                $.post("/Config/AddSource", { SourceName: Content }).done(function (data) {
                    $(".SourceArr").append(`
                        <div class="Source row" data-id="${data}">
                    <div class="col-md-10">${Content}</div>
                    <div class="col-md-2 DelSource">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                    `);
                });
            }
        );

        $("#AddProp").click(
            function () {
                var Content = $(".PropName").val();
                if (Content == "" || Content == null) {
                    alert("Заповніть поле тексту реквізиту");
                    return false;
                }
                $.post("/Config/AddProp", { text: Content }).done(
                    function (data) {
                        $(".PropsArr").append(`
                    <div class="Props row" data-id="${data}" >
                    <div class="col-md-10">${Content}</div>
                    <div class="col-md-2 DelProp">
                            <i class="fas fa-times"></i>
                    </div>
                </div>`);
                    }
                );
            }
        );
        $("#AddMeta").click(
            function () {
                var Content = $(".MetaName").val();
                if (Content == "" || Content == null) {
                    alert("Заповніть поле метою оцінки");
                    return false;
                }
                $.post("/Config/AddMeta", { Meta: Content }).done(
                    function (data) {
                        $(".MetasArr").append(`
                    <div class="Meta row" data-id="${data}" >
                    <div class="col-md-10">${Content}</div>
                    <div class="col-md-2 DelMeta">
                            <i class="fas fa-times"></i>
                    </div>
                </div>`);
                    }
                );
            }
        );

        $(".MetasArr").delegate(".DelMeta", "click", function () {
            var id = $(this).parent().data("id");
            var del = $(this).parent();
            var conf = confirm("Увага! Ви видаляєте мету оцінки.");
            if (!conf) {
                return false;
            }
            $.post("/Config/DelMeta", { Id: id }).done(function () {
                del.remove();
            });
        });

        $(".Objects").delegate(".ObjName", "click",
            function () {
                $(".ObjMeta").empty();
                var ObjName = $(this).text();
                var ObjId = $(this).parent().data("id");
                $.get("/Config/GetDesk", { Id: ObjId }).done(
                    function (data) {
                        var ObjDesk = data;
                        
                        $("#ObjName").text(ObjName);
                        $("#ObjId").val(ObjId);
                        $(".ObjMeta").empty();
                        for (var i = 0; i < ObjDesk.length; i++) {
                            $(".ObjMeta").append(`
                               <tr>
                                <td><input type='text' name='Desk' value='${ObjDesk[i].Name}' data-id='${ObjDesk[i].Id}' /></td><td class='DelDesk'><i class='fas fa-times'></i></td>
                                </tr>
                             `);
                        }
                        $("#DeskObj").dialog();
                    }
                );
            }
        );
        $(".Objects").delegate(".DelObj", "click", function () {
            var ObjId = $(this).parent().data("id");
            var removeObj = $(this).parent().parent();
            var res = confirm("Увага! Після видалення об'єкту,всі записи пов'язані з цим типом будуть видалені!");
            if (!res) {
                return false;
            }
            $.ajax({
                type: "POST",
                url: "/Config/DelObjectType",
                data: { Id:ObjId },
                success: function (data) {
                    removeObj.remove();
                }
            });           
        });

        $("#AddObject").click(
            function () {
                var name = $(".ObjectName").val();
                $.ajax({
                    type: "POST",
                    url: "/Config/AddObjectType",
                    data: { Name:name },
                    success: function (data) {
                        $(".Objects").append(`
                        <div class="container">
                    <div class="row Obj" data-id="${data}">
                        <div class="ObjName col-md-10">${name}</div>
                        <div class="col-md-2 DelObj">
                            <i class="fas fa-times"></i>
                        </div>
                    </div>
                </div>
                        `);
                    }
                });
            }
        );
    }
);