$(
    function () {
        
        //Contacts
        $("#del").click(function () {
            var Id = $(this).parent().children("#Id").val();
            var xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    location.reload();
                }
            };
            xhttp.open("POST", "/Home/DelContact?Id="+Id, true);
            xhttp.send();
        });

        $(".contact").click(
            function () {

                var id = $(this).children("input");
                var last = $(this).children(".name").children(".last");
                var first = $(this).children(".name").children(".first");
                var middle = $(this).children(".name").children(".middle");
                var email = $(this).children(".email");
                var tel = $(this).children(".tel");
                var company = $(this).children(".company");
                var position = $(this).children(".position");
                var comments = $(this).children(".comments");

                $("#dialog").children("form").children("#Id").val(id.val());
                $("#dialog").children("form").children("input[name='LastName']").val(last.text());
                $("#dialog").children("form").children("input[name='FirstName']").val(first.text());
                $("#dialog").children("form").children("input[name='MiddleName']").val(middle.text());
                $("#dialog").children("form").children("input[name='Tel']").val(tel.text());
                $("#dialog").children("form").children("input[name='Email']").val(email.text());
                $("#dialog").children("form").children("input[name='Company']").val(company.text());
                $("#dialog").children("form").children("input[name='Position']").val(position.text());
                $("#dialog").children("form").children("input[name='Comments']").val(comments.text());
                $("#dialog").dialog();
            }
        );
        //End code for Contacts
        

        function updateMessages(DialogId) {
            var UserId = $("#UserId").val();
            $.get("/Home/GetMessages", { Id: DialogId })
                .done(function (data) {
                    $(".ChatBody").empty();
                    for (var i = 0; i < data.length; i++) {
                        var Message = document.createElement("div");
                        Message.style.cssFloat = "left";
                        Message.className = "Message";
                        Message.style.clear = "both";
                        if (data[i].UserId == UserId) {
                            Message.style.cssFloat = "right";
                        }
                        if (data[i].SenderG != null) {
                            var sender = document.createElement("div");
                            sender.innerText = data[i].SenderG;
                            sender.className = "sender";
                            Message.appendChild(sender);
                        }
                        var content = document.createElement("div");
                        content.style.wordWrap = "break-word";
                        content.innerText = data[i].Content;
                        Message.appendChild(content);
                        var DateTime = document.createElement("div");
                        DateTime.className = "DateTime";
                        var date = new Date(data[i].dateTime.match(/\d+/)[0] * 1);

                        DateTime.innerText = date.toLocaleString();

                        Message.appendChild(DateTime);
                        $(".ChatBody").append(Message);
                    }
                  //  $(".ChatBody")[0].scrollTop = $(".ChatBody")[0].scrollHeight;
                });
        }
       
        var interval= null;
        $(".Dialog").click(
            function () {
                $(".edit").css("display", "none");
                clearInterval(interval);
                var dialogId = $(this).prev().val();
                interval = setInterval(updateMessages, 1000, dialogId);
                $("#DialogName").text($(this).children(".dName").text());
                $("#CurrentId").val(dialogId);
                var UserId = $("#UserId").val();
                $.get("/Home/GetMessages", { Id: dialogId })
                    .done(function (data) {
                        $(".ChatBody").empty();
                        for (var i = 0; i < data.length; i++) {
                            var Message = document.createElement("div");
                            Message.style.cssFloat = "left";
                            Message.className = "Message";
                            Message.style.clear = "both";
                            if (data[i].UserId == UserId) {
                                Message.style.cssFloat = "right";
                            }
                            if (data[i].SenderG != null) {
                                $(".edit").css("display", "block");
                                var sender = document.createElement("div");
                                sender.innerText = data[i].SenderG;
                                sender.className = "sender";
                                Message.appendChild(sender);
                            }
                            var content = document.createElement("div");
                            content.style.wordWrap = "break-word";
                            content.innerText = data[i].Content;

                            Message.appendChild(content);
                            

                            var DateTime = document.createElement("div");
                            DateTime.className = "DateTime";
                            var date = new Date(data[i].dateTime.match(/\d+/)[0] * 1);

                            DateTime.innerText = date.toLocaleString();

                            Message.appendChild(DateTime);
                            $(".ChatBody").append(Message);
                        }
                        $(".ChatBody")[0].scrollTop = $(".ChatBody")[0].scrollHeight;
                    });

            }
        );
        $("#Send").click(
            function () {
                var content = $(this).prev().val();
                var dialogId = $("#CurrentId").val();
                var UserId = $("#UserId").val();
                if (content == "") {
                    return;
                }
                $(this).prev().val("");
                if (dialogId == "0") {
               
                    $.get("/Home/CreateDialog", { AnotherUserId: $("#DialogUserId").val() })
                        .done(function (data) {
                          
                            clearInterval(interval);
                            dialogId = data;
                            interval = setInterval(updateMessages, 1000, dialogId);
                            $("#CurrentId").val(data)
                            $.post("/Home/SendMessage", { UserId: UserId, DialogId: dialogId, Content: content, dateTime: new Date().toLocaleString() });
                            $.get("/Home/GetMessages", { Id: dialogId })
                                .done(function (data) {

                                    $(".ChatBody").empty();
                                    for (var i = 0; i < data.length; i++) {
                                        var Message = document.createElement("div");
                                        Message.className = "Message";
                                        Message.style.cssFloat = "left";
                                        Message.style.clear = "both";
                                        if (data[i].UserId == UserId) {
                                            Message.style.cssFloat = "right";
                                        }
                                        if (data[i].SenderG != null) {
                                            var sender = document.createElement("div");
                                            sender.innerText = data[i].SenderG;
                                            sender.className = "sender";
                                            Message.appendChild(sender);
                                        }
                                        var content = document.createElement("div");
                                        content.style.wordWrap = "break-word";
                                        content.innerText = data[i].Content;
                                        Message.appendChild(content);
                                        var DateTime = document.createElement("div");
                                        DateTime.className = "DateTime";
                                        var date = new Date(data[i].dateTime.match(/\d+/)[0] * 1);

                                        DateTime.innerText = date.toLocaleString();

                                        Message.appendChild(DateTime);
                                        $(".ChatBody").append(Message);

                                    }
                                   $(".ChatBody")[0].scrollTop = $(".ChatBody")[0].scrollHeight;
                                });
                            return;
                        });
                }
                $.post("/Home/SendMessage", { UserId: UserId, DialogId: dialogId, Content: content, dateTime: new Date().toLocaleString()})
                    .done(function () {
                        $.get("/Home/GetMessages", { Id: dialogId })
                            .done(function (data) {
                           
                                $(".ChatBody").empty();
                                for (var i = 0; i < data.length; i++) {
                                    var Message = document.createElement("div");
                                    Message.className = "Message";
                                    Message.style.cssFloat = "left";
                                    Message.style.clear = "both";
                                    if (data[i].UserId == UserId) {
                                        Message.style.cssFloat = "right";
                                    }
                                    if (data[i].SenderG != null) {
                                        var sender = document.createElement("div");
                                        sender.innerText = data[i].SenderG;
                                        sender.className = "sender";
                                        Message.appendChild(sender);
                                    }
                                    var content = document.createElement("div");
                                    content.style.wordWrap = "break-word";
                                    content.innerText = data[i].Content;
                                    Message.appendChild(content);
                                    var DateTime = document.createElement("div");
                                    DateTime.className = "DateTime";
                                    var date = new Date(data[i].dateTime.match(/\d+/)[0] * 1);

                                    DateTime.innerText = date.toLocaleString();

                                    Message.appendChild(DateTime);
                                    $(".ChatBody").append(Message);
                                 
                                }
                                $(".ChatBody")[0].scrollTop = $(".ChatBody")[0].scrollHeight;
                            });
                    });
             
            }
        );
 
        $("#MessUser").click(
            function () {
                $.get("/Home/GetAllUsers", {})
                    .done(function (data) {
                        $("#ListUsers>div").empty();
                        for (var i = 0; i < data.length; i++) {

                  
                            var user = document.createElement("div");
                            var userId = document.createElement("input");
                            userId.type = "hidden";
                            userId.value = data[i].Id;
                            user.className = "liUser";
                 
                            user.innerText = data[i].FirstName + " " + data[i].LastName;
                            $("#ListUsers>div").append(userId);
                            $("#ListUsers>div").append(user);

                            $(".liUser").on("click", function () {
                                clearInterval(interval);
                              
                                var UserId = $(this).prev().val();
                                var UserName = $(this).text();
                                $(".ChatBody").empty();
                                $("#DialogName").text(UserName);
                                $("#ListUsers").dialog().dialog("close");
                                $("#CurrentId").val("0");
                                $("#DialogUserId").val(UserId);
                            });
                        }
                    });
                
                $("#ListUsers").dialog();

            }
        );
     
        function DeleteDialog() {
            var DialogId = $(this).parent().prev().val();
            $.post("/Home/DisableDialog", { Id: DialogId })
            event.stopPropagation();
        }

        $("#MessGroup").click(
            function () {
                $.get("/Home/GetAllUsers", {})
                    .done(function (data) {
                        $("#ListUsers>div").empty();
                        $("#ListUsers>div").append("Назва чату: ");
                        $("#ListUsers>div").append("<input type='text' id='ChatName'/><br>");
                        for (var i = 0; i < data.length; i++) {

                            var div = document.createElement("div");
                            div.className = "checkbox";
                            var label = document.createElement("label");
                            div.appendChild(label);
                            var input = document.createElement("input");
                            input.type = "checkbox";
                            input.className = "membersChat";
                            input.value = data[i].Id;
                            label.appendChild(input);
                            label.innerHTML = label.innerHTML + "  " +data[i].FirstName + " " + data[i].LastName;
                       
                            $("#ListUsers>div").append(div);
                        }
                        var button = document.createElement("button");
                        button.innerText = "Створити чат";
                        button.className = "CreateChatBtn";
                    
                        $("#ListUsers>div").append(button);

                        $(".CreateChatBtn").on("click", function () {
                            if ($("#ChatName").val() == "") {
                                alert("Будь ласка вкажіть назву чату");
                                return;
                            }
                            var arrOfUsers = $(".membersChat:checked");
                            var arr = new Array();
                            for (var i = 0; i < arrOfUsers.length; i++) {
                                arr.push(arrOfUsers.eq(i).val());
                               
                            }
                            jQuery.ajaxSettings.traditional = true;
                            $.get("/Home/CreateChat", { Name: $("#ChatName").val(), AnotherUsersId: arr }).done(
                                function (data) {
                                    clearInterval(interval);
                                    var dialogId = data;
                                    interval = setInterval(updateMessages, 1000, dialogId);
                                    $("#CurrentId").val(data)


                                    $("#DialogName").text($("#ChatName").val());
                                    $("#ListUsers").dialog().dialog("close");
                                    
                                    return;
                                }
                            );
                        });

                        $("#ListUsers").dialog();
                    });
            }
        );


        ///
        function OpenDialog() {
            $(".edit").css("display","none");
            clearInterval(interval);
            var dialogId = $(this).prev().val();
            interval = setInterval(updateMessages, 1000, dialogId);
            $("#DialogName").text($(this).children(".dName").text());
            $("#CurrentId").val(dialogId);
            var UserId = $("#UserId").val();
            $.get("/Home/GetMessages", { Id: dialogId })
                .done(function (data) {
                    $(".ChatBody").empty();
                    for (var i = 0; i < data.length; i++) {
                        var Message = document.createElement("div");
                        Message.style.cssFloat = "left";
                        Message.className = "Message";
                        Message.style.clear = "both";
                        if (data[i].UserId == UserId) {
                            Message.style.cssFloat = "right";
                        }
                        if (data[i].SenderG != null) {
                            $(".edit").css("display", "block");
                            var sender = document.createElement("div");
                            sender.innerText = data[i].SenderG;
                            sender.className = "sender";
                            Message.appendChild(sender);
                        }
                        var content = document.createElement("div");
                        content.style.wordWrap = "break-word";
                        content.innerText = data[i].Content;
                        Message.appendChild(content);

                        var DateTime = document.createElement("div");
                        DateTime.className = "DateTime";
                        var date = new Date(data[i].dateTime.match(/\d+/)[0] * 1);

                        DateTime.innerText = date.toLocaleString();

                        Message.appendChild(DateTime);
                        $(".ChatBody").append(Message);
                    }
                    $(".ChatBody")[0].scrollTop = $(".ChatBody")[0].scrollHeight;
                });

        }

        ///
        var intervalDialog = null;
        
            intervalDialog = setInterval(updateDialogs, 2000);
        
        function updateDialogs() {
            $.get("/Home/MessagesAjax").done(
                function (data) {
                    $("#DialogList").empty();
                    var countUnreads = 0;
                    for (var i = 0; i < data.dialogs.length; i++) {
                        var dialogId = document.createElement("input");
                        dialogId.type = "hidden";
                        dialogId.className = "dialogId";
                        dialogId.value = data.dialogs[i].Id;
                        $("#DialogList").append(dialogId);
                        var Dialog = document.createElement("div");
                        Dialog.addEventListener("click", OpenDialog, false);
                        Dialog.className = "Dialog";
                        var Name = document.createElement("span");
                        Name.className = "dName";
                        Name.innerText = data.dialogs[i].Name;
                        Dialog.appendChild(Name);

                        var del = document.createElement("span");
                        del.className = "delDialog";
                        del.innerHTML = " <i class='fas fa-times close'></i>";
                        del.addEventListener("click", DeleteDialog, false);
                        Dialog.appendChild(del);
                        var lsMess = document.createElement("div");
                        lsMess.className = "lastMess";
                       
                        if (data.unread[i].Count != 0)
                            lsMess.style.border = "2px solid orange";
                        if (data.messages[i] == null) {
                            lsMess.innerText = "Пусто";
                        }
                        else {
                            if (data.unread[i].Count != 0) {
                                lsMess.innerHTML = data.messages[i].Content + `<span class='countMessages'>${data.unread[i].Count}</span>`;
                                countUnreads += Number.parseInt(data.unread[i].Count);
                            }
                            else
                                lsMess.innerHTML = data.messages[i].Content;
                        }
                       
                        Dialog.appendChild(lsMess);
                        $("#DialogList").append(Dialog);
                    }
                    if (countUnreads != 0) {
                        $(".countUnreadMessages").text(countUnreads);
                        $(".countUnreadMessages").css("border", "3px solid orange");
                    }
                }
            );
        }
        $(".edit").click(
            function () {
                var dialogId = $("#CurrentId").val();
                var allUsers = null;
                $.get("/Home/GetAllUsers", {})
                    .done(function (data) {
                        allUsers = data;
            
                $.get("/Home/GetDialogInfo", { Id: dialogId }).done(function (data) {
                    $("#ListUsers>div").empty();
                    $("#ListUsers>div").append("Назва чату: ");
                    $("#ListUsers>div").append("<input type='text' id='ChatName' value='" + $("#DialogName").text() + "'/><br>");
               
                    for (var i = 0; i < allUsers.length; i++) {

                        var div = document.createElement("div");
                    div.className = "checkbox";
                    var label = document.createElement("label");
                    div.appendChild(label);
                    var input = document.createElement("input");
                        input.type = "checkbox";
                       
                    input.className = "membersChat";
                     input.value = allUsers[i].Id;
                        label.appendChild(input);
                        for (var j = 0; j < data.length; j++) {
                            if (allUsers[i].Id == data[j].UserId) {
                                input.setAttribute("checked", "checked");

                            }
                        }
                        label.innerHTML = label.innerHTML + "  " + allUsers[i].FirstName + " " + allUsers[i].LastName;
                    $("#ListUsers>div").append(div);
                }
                var button = document.createElement("button");
                button.innerText = "Змінити чат";
                button.className = "CreateChatBtn";

                $("#ListUsers>div").append(button);

                    $(".CreateChatBtn").on("click", function () {
                    if ($("#ChatName").val() == "") {
                        alert("Будь ласка вкажіть назву чату");
                        return;
                    }
                    var arrOfUsers = $(".membersChat:checked");
                    var arr = new Array();
                    for (var i = 0; i < arrOfUsers.length; i++) {
                        arr.push(arrOfUsers.eq(i).val());

                    }
                        jQuery.ajaxSettings.traditional = true;
                        $.post("/Home/EditChat", { Name: $("#ChatName").val(), AnotherUsersId: arr, Id: dialogId }).done(
                        function () {
                            location.reload();
                        }
                    );
                });

                    }
                        );
                    });
                $("#ListUsers").dialog();
              

                //
            }
        );
        $("#messageContent").keypress(function (e) {
            if (e.which == 13) {
                $("#Send").click();
                $(this).text("\b");
            }
        });
    }
);