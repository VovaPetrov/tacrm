$(document).ready(
    function () {
        $("#delOrder").click(
            function () {
                var ans = confirm("Бажаєте видалити замовлення?");
                if (!ans) {
                    return false;
                }
            }
        );

        $(".deleteFile").click(
            function () {
                var Id = $(this).parent().find("a").data("id");
                var file = $(this).parent();
                $.post("/Order/DeleteFile", { FileId: Id })
                    .done(
                    function () {
                            file.remove();
                        });
            });
  
        $("#uploadFile").click(function () {
            event.preventDefault();
            var orderId = $("input[name='Order']").val();
            var formdata = new FormData(); //FormData object
            var fileInput = $("input[name='fileToOrder'")[0];
            //Iterating through each files selected in fileInput
            for (i = 0; i < fileInput.files.length; i++) {
                //Appending each file to FormData object
                formdata.append(fileInput.files[i].name, fileInput.files[i]);
            }
            //Creating an XMLHttpRequest and sending
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/Order/UploadFile?Order=' + orderId);
            xhr.send(formdata);
            $("#preLoader").dialog();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    $("#preLoader").dialog("close");
                    var data = JSON.parse(this.response);
                    $(".files").append(`
            <div class="row file">
                <div class="col-md-1">
                    <i class="fas fa-file"></i>
                </div>
                <div class="col-md-4">
                    <a href="http://drive.google.com/file/d/${data.Id}" data-id='${data.Id}' target="_blank">${data.Name}</a>
                </div>
                <div class="col-md-7 deleteFile pull-right">
                    <i class="fas fa-trash"></i>
                </div>
            </div>`);
                }
            }
        });

        $("#sourceSelect").change(
            function () {
                var SourceId = $("#sourceSelect option:selected").val();
                $.get("/Order/GetBranches", { SourceId: SourceId }).done(
                    function (data) {
                        $("#branchSelect").empty();
                        for (var i = 0; i < data.length; i++) {
                            $("#branchSelect").append("<option value='" + data[i].Id + "'>" + data[i].BranchName + "</option>");
                        }
                    }
                );
            });

        $(".delPrice").click(
            function () {
                $(this).parent().parent().remove();
                var totalD = 0;
                $(".partPrice").each(
                    function () {
                        totalD += (+$(this).text().split(',')[0]);
                    }
                );
                $("input[name='TotalPrice']").val(totalD);
            }
        );

        $("#addPrice").click(
            function () {
                var appointment = $("input[name='Appointment']").val();
                var price = $("input[name='ObjPrice']").val();
                $("#partsPrice").append(
                    `<tr>
 <td> <input type="hidden" value="0" class="PayId" /></td>
                        <td class='appointment'><input type="text" value="${appointment}" /></td>
                        <td class='partPrice'>${price}</td>
                         <td>
                                    <input type="date" name="payDate" class="payDate form-control" /> </td>
                                <td>

                                    <input type="checkbox" class="isPaid" name="isPaid" />
                                </td>
                        <td>  <span class='delPrice' > <i class='fas fa-times'></i></span></td>
                    </tr>`);
                var total = 0;

                $(".partPrice").each(
                    function () {
                        total += (+$(this).text().split(',')[0]);
                    }
                );
                $("input[name='TotalPrice']").val(total);
                $(".delPrice").click(
                    function () {
                        $(this).parent().parent().remove();
                        var totalD = 0;
                        $(".partPrice").each(
                            function () {
                                totalD += (+$(this).text().split(',')[0]);
                            }
                        );
                        $("input[name='TotalPrice']").val(totalD);
                    }
                );
            });

        $("#orderTable tr").click(
            function () {
                var OrderId = $(this).find(".OrderId").text();
                location.href = "/Order/Get?Id=" + OrderId;
            });

        $("#addOrder").click(
            function () {
                $("#preLoader .ui-dialog-titlebar-close").hide();
                $("#preLoader").dialog({
                    create: function (event, ui) {
                        $(event.target).parent().css('position', 'fixed');
                    },
                    resizeStop: function (event, ui) {
                        var position = [(Math.floor(ui.position.left) - $(window).scrollLeft()),
                        (Math.floor(ui.position.top) - $(window).scrollTop())];
                        $(event.target).parent().css('position', 'fixed');
                        $(dlg).dialog('option', 'position', position);
                    },
                    closeOnEscape: false,
                    open: function (event, ui) {
                        $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
                    }
                });

                var MetaId = $("#AddOrderForm").find("#metaSelect option:selected").val();
                var StatusId = $("#AddOrderForm").find("#statusSelect option:selected").val();
                var SourceId = $("#AddOrderForm").find("#sourceSelect option:selected").val();
                var BranchId = $("#AddOrderForm").find("#branchSelect option:selected").val();
                var UsersId = new Array();
                $("#AddOrderForm").find("input[name='UserId']").each(
                    function () {
                        if ($(this).is(':checked')) {
                            UsersId.push($(this).val());
                        }
                    }
                );
                if (UsersId.length==0) {
                    alert("Виберіть хоча б одного виконавця");
                    $("#preLoader").dialog("close");
                    return false;
                }

                var SignatoriesId = new Array();
                $("#AddOrderForm").find("input[name='SignatoryId']").each(
                    function () {
                        if ($(this).is(':checked')) {
                            SignatoriesId.push($(this).val());
                        }
                    }
                );



                var CounterpartyId = $("#AddOrderForm").find("#counterpartySelect option:selected").val();
                //Client
                var ClientName = $("#AddOrderForm").find("input[name='ClientName']").val();
                var IPN = $("#AddOrderForm").find("input[name='IPN']").val();
                var Tel = $("#AddOrderForm").find("input[name='Tel']").val();
                var Email = $("#AddOrderForm").find("input[name='Email']").val();
                var Reckv = $("#AddOrderForm").find("input[name='Reck']").val();
                //Owner
                var OwnerName = $("#AddOrderForm").find("input[name='OwnerName']").val();
                var OwnerIPN = $("#AddOrderForm").find("input[name='OwnerIPN']").val();
                var OwnerTel = $("#AddOrderForm").find("input[name='OwnerTel']").val();
                var OwnerEmail = $("#AddOrderForm").find("input[name='OwnerEmail']").val();
                var OwnerReckv = $("#AddOrderForm").find("input[name='OwnerReck']").val();

                var CountDays = $("#AddOrderForm").find("input[name='CountDays']").val();
                var Inspector = $("#AddOrderForm").find("input[name='Inspector']").val();
                var InspectionDate = $("#AddOrderForm").find("input[name='InspectionDate']").val();
                var InspectionPrice = $("#AddOrderForm").find("input[name='InspectionPrice']").val();
                var Comments = $("#AddOrderForm").find("textarea[name='Comments']").val();
                var ReckvId = $("#AddOrderForm").find("select[name='Props'] option:selected").val();
                var dateOfDocument = $("#AddOrderForm").find("input[name='DateOfDocument']").val();
                var DateOfTransfer = $("#AddOrderForm").find("input[name='DateOfTransfer']").val();
                var CommentsOfTransfer = $("#AddOrderForm").find("input[name='CommentsOfTransfer']").val();
                var DateTakeVerification = $("#AddOrderForm").find("input[name='DateTakeVerification']").val();
                var DateDirectVerification = $("#AddOrderForm").find("input[name='DateDirectVerification']").val();
                var DateEndVerification = $("#AddOrderForm").find("input[name='DateEndVerification']").val();
                var CommentsVerification = $("#AddOrderForm").find("input[name='CommentsVerification']").val();
                var DateOfExpert = $("#AddOrderForm").find("input[name='DateOfExpert']").val();

                var ObjListArray = new Array();
                var objId = $(".objId");
                if (objId.length == 0) {
                    alert("Додайте хоча б 1 об'єкт для оцінки");
                    $("#preLoader").dialog("close");
                    return false;
                }

                if (ClientName == null || ClientName == "") {
                    alert("Введіть ПІБ замовника");
                    $("#preLoader").dialog("close");
                    return false;
                }
                if (Tel == null || Tel == "") {
                    alert("Введіть номер телефону замовника");
                    $("#preLoader").dialog("close");
                    return false;
                }

                var PriceArr = new Array();
                var AppoArr = new Array();
                var PayDateArr = new Array();
                var IsPaidArr = new Array();
               
                var Appointment = $("#partsPrice").find(".appointment input").each(
                    function () {
                        AppoArr.push($(this).val());
                      
                        
                    }
                );
                var PartPrice = $("#partsPrice").find(".partPrice").each(
                    function () {
                
                        PriceArr.push($(this).text());
                    }
                );
                var PayDates = $("#partsPrice").find(".payDate").each(
                    function () {
                        PayDateArr.push($(this).val());
                    }
                );
                var IsPaided = $("#partsPrice").find(".isPaid").each(
                    function () {
                        IsPaidArr.push($(this).prop('checked'));
                    }
                );


                var orderId = "";
                $.ajax({
                    type: "POST",
                    url: "/Order/Add",              
                   success:
                        function (data) {
                                orderId = data;
                                
                                for (var i = 0; i < objId.length; i++) {

                                    $.ajax({
                                        type: "POST", url: "/Order/CreateObjList", data: { OrderId: data, ObjId: $(objId[i]).val() }, success:
                                            function (data) {
                                                ObjListArray.push(data);
                                            }, async: false
                                    });

                                }
                                var ObjData = $(".objData");
                                for (var i = 0; i < ObjListArray.length; i++) {

                                    ObjData.eq(i).find("input").each(
                                        function () {
                                            var DeskId = $(this).data("id");
                                            var Value = $(this).val();
                                            $.ajax({ type: "POST", url: "/Order/ObjectValues", data: { ObjListId: ObjListArray[i], ObjectDeskId: DeskId, Value: Value }, async: false });
                                        }
                                    );
                                }
                            alert("Замовлення збережено");
                            location.href = "/Order/Get?id=" + orderId;
                        },
                    data: {
                        MetaId: MetaId, CountDays: CountDays, StatusId: StatusId, SourceId: SourceId, BranchId: BranchId,
                        UsersId: UsersId, CounterpartyId: CounterpartyId ,ClientName: ClientName, Tel: Tel, IPN: IPN, Email: Email, Reckv: Reckv, OwnerName: OwnerName, OwnerTel: OwnerTel, OwnerIPN: OwnerIPN, OwnerEmail: OwnerEmail, OwnerReckv: OwnerReckv,
                        ReckvId: ReckvId, Comments: Comments, DateOfDocument: dateOfDocument,
                        PayingInfo: { PriceArr: PriceArr, AppoArr: AppoArr, IsPaidArr: IsPaidArr, PayDateArr: PayDateArr },
                        Inspector: Inspector, InspectionDate: InspectionDate, InspectionPrice: InspectionPrice, CommentsOfTransfer: CommentsOfTransfer, DateOfTransfer: DateOfTransfer,
                        DateTakeVerification: DateTakeVerification, DateDirectVerification: DateDirectVerification, DateEndVerification: DateEndVerification, CommentsVerification: CommentsVerification, DateOfExpert: DateOfExpert, SignatoriesId: SignatoriesId
                    }
                });


            });

        $("#saveOrder").click(
            function () {
                $("#preLoader .ui-dialog-titlebar-close").hide();
                $("#preLoader").dialog({
                    create: function (event, ui) {
                        $(event.target).parent().css('position', 'fixed');
                    },
                    resizeStop: function (event, ui) {
                        var position = [(Math.floor(ui.position.left) - $(window).scrollLeft()),
                        (Math.floor(ui.position.top) - $(window).scrollTop())];
                        $(event.target).parent().css('position', 'fixed');
                        $(dlg).dialog('option', 'position', position);
                    },
                    closeOnEscape: false,
                    open: function (event, ui) {
                        $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
                    }
                });

                var MetaId = $("#EditOrder").find("#metaSelect option:selected").val();
                var StatusId = $("#EditOrder").find("#statusSelect option:selected").val();
                var SourceId = $("#EditOrder").find("#sourceSelect option:selected").val();
                var BranchId = $("#EditOrder").find("#branchSelect option:selected").val();
                var UsersId = new Array();
                $("#EditOrder").find("input[name='UserId']").each(
                    function () {
                        if ($(this).is(':checked')) {
                            UsersId.push($(this).val());
                        }
                    }
                );
                if (UsersId.length == 0) {
                    alert("Виберіть хоча б одного виконавця");
                    $("#preLoader").dialog("close");
                    return false;
                }

                var SignatoriesId = new Array();
                $("#EditOrder").find("input[name='SignatoryId']").each(
                    function () {
                        if ($(this).is(':checked')) {
                            SignatoriesId.push($(this).val());
                        }
                    }
                );

                var ClientName = $("#EditOrder").find("input[name='ClientName']").val();
                var IPN = $("#EditOrder").find("input[name='IPN']").val();
                var Tel = $("#EditOrder").find("input[name='Tel']").val();
                var Email = $("#EditOrder").find("input[name='Email']").val();
                var Reckv = $("#EditOrder").find("input[name='Reck']").val();
                var CountDays = $("#EditOrder").find("input[name='CountDays']").val();

                //Owner
                var OwnerName = $("#EditOrder").find("input[name='OwnerName']").val();
                var OwnerIPN = $("#EditOrder").find("input[name='OwnerIPN']").val();
                var OwnerTel = $("#EditOrder").find("input[name='OwnerTel']").val();
                var OwnerEmail = $("#EditOrder").find("input[name='OwnerEmail']").val();
                var OwnerReckv = $("#EditOrder").find("input[name='OwnerReck']").val();

                var Inspector = $("#EditOrder").find("input[name='Inspector']").val();
                var InspectionDate = $("#EditOrder").find("input[name='InspectionDate']").val();
                var InspectionPrice = $("#EditOrder").find("input[name='InspectionPrice']").val();
                var InspectionIsPaid = $("#EditOrder").find("input[name='oplataOverwatch']:checked").val();

                var Comments = $("#EditOrder").find("textarea[name='Comments']").val();
                var ReckvId = $("#EditOrder").find("select[name='Props'] option:selected").val();
                var IsPaid = $("#EditOrder").find("input[name='oplata']:checked").val();

       
                var dateOfDocument = $("#EditOrder").find("input[name='DateOfDocument']").last().val();
                var DateOfTransfer = $("#EditOrder").find("input[name='DateOfTransfer']").val();
                var CommentsOfTransfer = $("#EditOrder").find("input[name='CommentsOfTransfer']").val();
                var DateTakeVerification = $("#EditOrder").find("input[name='DateTakeVerification']").val();
                var DateDirectVerification = $("#EditOrder").find("input[name='DateDirectVerification']").val();
                var DateEndVerification = $("#EditOrder").find("input[name='DateEndVerification']").val();
                var CommentsVerification = $("#EditOrder").find("input[name='CommentsVerification']").val();
                var DateOfExpert = $("#EditOrder").find("input[name='DateOfExpert']").val();

                var СounterpartyId = $("#EditOrder").find("#counterpartySelect option:selected").val();
   

                if (ClientName == null || ClientName == "") {
                    alert("Введіть ПІБ замовника");
                    $("#preLoader").dialog("close");
                    return false;
                }
                if (Tel == null || Tel == "") {
                    alert("Введіть номер телефону замовника");
                    $("#preLoader").dialog("close");
                    return false;
                }

                var PriceArr = new Array();
                var AppoArr = new Array();
                var PayDateArr = new Array();
                var IsPaidArr = new Array();
                var IdArr = new Array();

                $("#partsPrice").find(".PayId").each(
                    function () {
                        IdArr.push($(this).val());
                    }
                );

                var Appointment = $("#partsPrice").find(".appointment input").each(
                    function () {
                        AppoArr.push($(this).val());
                    }
                );
                var PartPrice = $("#partsPrice").find(".partPrice").each(
                    function () {
                        PriceArr.push($(this).text());
                    }
                );
                ////
                var PayDates = $("#partsPrice").find(".payDate").each(
                    function () {
                        PayDateArr.push($(this).val());
                    }
                );
                var IsPaided = $("#partsPrice").find(".isPaid").each(
                    function () {
                        IsPaidArr.push($(this).prop('checked'));
                    }
                );
               


                $.ajax({
                    type: "POST", url: "/Order/Edit",
                    success:
                        function (data) {
                            var ObjListArray = new Array();
                            var objId = $(".objId");
                            for (var i = 0; i < objId.length; i++) {

                                $.ajax({
                                    type: "POST", url: "/Order/CreateObjList", data: { OrderId: data, ObjId: $(objId[i]).val() }, success:
                                        function (data) {
                                            ObjListArray.push(data);
                                        }, async: false
                                });

                            }
                            var ObjData = $(".objData");
                            for (var i = 0; i < ObjListArray.length; i++) {

                                ObjData.eq(i).find("input").each(
                                    function () {
                                        var DeskId = $(this).data("id");
                                        var Value = $(this).val();
                                        $.ajax({ type: "POST", url: "/Order/ObjectValues", data: { ObjListId: ObjListArray[i], ObjectDeskId: DeskId, Value: Value }, async: false });
                                    }
                                );
                            }
                            alert("Замовлення збережено");
                            location.href = "/Order/Index";
                        },
                    data: {
                        OrderId: $("input[name='Order']").val(),
                        MetaId: MetaId, StatusId: StatusId, SourceId: SourceId, BranchId: BranchId,
                        UsersId: UsersId, CounterpartyId: СounterpartyId, ClientName: ClientName, Tel: Tel, IPN: IPN, Email: Email, Reckv: Reckv,
                        ReckvId: ReckvId, Comments: Comments, CountDays: CountDays,
                         DateOfDocument: dateOfDocument,
                        OwnerName: OwnerName, OwnerTel: OwnerTel, OwnerIPN: OwnerIPN, OwnerEmail: OwnerEmail, OwnerReckv: OwnerReckv,
                        Inspector: Inspector, PaidOverWatch: InspectionIsPaid, InspectionDate: InspectionDate, InspectionPrice: InspectionPrice, IsPaid: IsPaid,
                        PayingInfo: {IdArr:IdArr, PriceArr: PriceArr, AppoArr: AppoArr, IsPaidArr: IsPaidArr, PayDateArr: PayDateArr },
                        CommentsOfTransfer: CommentsOfTransfer, DateOfTransfer: DateOfTransfer,
                        DateTakeVerification: DateTakeVerification, DateDirectVerification: DateDirectVerification, DateEndVerification: DateEndVerification, CommentsVerification: CommentsVerification, DateOfExpert: DateOfExpert, SignatoriesId: SignatoriesId
                    }
                });


            }
        );
        $(".delObj").on("click",
            function () {
                $(this).parents(".ObjMeta").empty();
            }
        );
        $("#addObjectToOrder").click(
            function () {

                var objId = $("#objectSelect option:selected").val();
                var objName = $("#objectSelect option:selected").text();


                $.get("/Order/GetObjectType", { Id: objId }).done(
                    function (data) {
                        $(".ObjectList").append(
                            `
                    <div class="ObjMeta ">
                            <hr>
                                   <div class="row">
                                  <div class='col-md-2'>   <h5>${objName}</h5>  </div> <div class='col-md-2'>
                                 <span class='delObj' > <i class='fas fa-times'></i></span>
                                  </div>
                                   </div>
                                 <div class='Object'>
                                    <input type='hidden' class="objId" value="${objId}" />
                                    <div class="row objData">
                                    </div>
                                 </div>
                    </div> `
                        );
                        $(".delObj").on("click",
                            function () {
                                $(this).parents(".ObjMeta").empty();
                            }
                        );
                        for (var i = 0; i < data.length; i++) {

                            $(".Object .row").last().append(
                                `<div class="form-group col-md-4">
                                            <input type='text' class="objectDesc form-control" data-id='${data[i].Id}'  placeholder='${data[i].Name}' />
                                        </div>`
                            );
                        }
                    }
                );

            });

        $.datepicker.regional['ru'] = {
            closeText: 'Закрыть',
            prevText: 'Пред',
            nextText: 'След',
            currentText: 'Сегодня',
            monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
                'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
            monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн',
                'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
            dayNames: ['воскресенье', 'понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота'],
            dayNamesShort: ['вск', 'пнд', 'втр', 'срд', 'чтв', 'птн', 'сбт'],
            dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
            weekHeader: 'Нед',
            dateFormat: 'mm/dd/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };
        $.datepicker.setDefaults($.datepicker.regional['ru']);

        $(".datepickerFrom").datepicker();
        $(".datepickerTo").datepicker();

    });

