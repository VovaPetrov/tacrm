$(document).ready(
    function () {

        $("#addNewTask").click(function () {
            $(".TaskDialog").dialog();
        });
  

        $(".editTask").click(
            function () {
              
                var Id = $(this).siblings(".TaskId").val();
                var date = $(this).siblings(".date").text();
                var time = $(this).siblings(".time").text();
                $(".TaskDialogEdit").find(".TaskList").empty();
                $(".EditTaskId").val(Id);
                $.get("/Missions/GetInfo", { Id: Id }).done(
                    function (data) {
                       

                        //Set Date for EditForm
                        ///////        
                        var dateF = date.split(".");
                        dateF = dateF.reverse();
                        var d = dateF.join("");
                        d = d.replace(" ", "-").replace(" ", "-");
                        $("#editDate").val(d);
                        ///////
                        ///

                        //Set Time for EditForm
                        ///////      
                        var timeF = time.replace(" ", "").replace(" ", "");
                        if (timeF.split(":")[0].length == 1) {
                            var hh = timeF.split(":")[0];
                            var mm = timeF.split(":")[1];
                            timeF = "0" + hh + ":" + mm;
                        }
                        $("#editTime").val(timeF);
                       
                        ///////
                        //
                        for (var i = 0; i < data.length; i++) {
                            var div = document.createElement("div");
                            div.className = "TaskElement";
                            var check = document.createElement("input");
                            check.type = "checkbox";
                            var hidden = document.createElement("input");
                            hidden.name = "Status";
                            hidden.type = "hidden";
                            if (data[i].Status == true) {
                                check.checked = true;
                                hidden.value = "true";
                            }
                            else {
                                check.checked = false;
                                hidden.value = "false";
                            }
                            check.setAttribute("name", "StatusCheck");
                            div.appendChild(check);
                            div.appendChild(hidden);
                            check.onclick = function () {
                                $(this).next().val(this.checked);
                            }
                            var elem = document.createElement("input");
                            elem.type = "text";
                            elem.value = data[i].Content;
                            elem.setAttribute("name", "Element");
                            elem.setAttribute("required", "required");
                            var del = document.createElement("span");
                            del.innerHTML = " <i class='fas fa-times' style='color:red'></i>";
                            del.style.cursor = "pointer";
                            div.appendChild(elem);
                            div.appendChild(del);
                            del.onclick = function () {
                                var id = $(".TaskDialogEdit").find(".TaskList").children().index($(this).parent());

                                $(".TaskDialogEdit").find(".TaskList").children().eq(id).remove();
                            }
                            $(".TaskDialogEdit").find(".TaskList").append(div);
                        }
                    }
                    );

                $(".TaskDialogEdit").dialog();
            }
        );
        $(".delete").click(function () {
            var Id = $(this).siblings(".TaskId").val();
            var thisVar = $(this);
            var ans = confirm("Бажаєте видалити завдання?");
            if (ans == false)
                return;
            $.post("/Missions/Delete", { Id: Id }).done(
                function () {
                    $(thisVar).parent().remove();
                }
            );
        });
        $(".addNewElement").click(
            function () {
                var div = document.createElement("div");
                div.className = "TaskElement";

                var elem = document.createElement("input");
                elem.type = "text";
                elem.setAttribute("name", "Element");
                elem.setAttribute("required", "required");
                var del = document.createElement("span");
                del.innerHTML = " <i class='fas fa-times' style='color:red'></i>";
                del.style.cursor = "pointer";
                div.appendChild(elem);
                div.appendChild(del);
                del.onclick = function () {
                    var id = $(".TaskElement").index($(this).parent());
                    $(".TaskElement").eq(id).remove();                
                }
                $(".TaskList").append(div);
               
            });
        $(".addNewElementEdit").click(
            function () {
                
                var div = document.createElement("div");
                div.className = "TaskElement";
                var check = document.createElement("input");
                check.type = "checkbox";

                div.appendChild(check);

                var hidden = document.createElement("input");
                hidden.name = "Status";
                hidden.type = "hidden";
                    hidden.value = "false";

                check.setAttribute("name", "StatusCheck");
                div.appendChild(check);
                div.appendChild(hidden);
                check.onclick = function () {
                    $(this).next().val(this.checked);
                }

                var elem = document.createElement("input");
                elem.type = "text";
                elem.setAttribute("name", "Element");
                elem.setAttribute("required", "required");
                var del = document.createElement("span");
                del.innerHTML = " <i class='fas fa-times' style='color:red'></i>";
                del.style.cursor = "pointer";
                div.appendChild(elem);
                div.appendChild(del);
                del.onclick = function () {
                    var id = $(".TaskElement").index($(this).parent());
                    $(".TaskElement").eq(id).remove();
                }
                $(".TaskList").append(div);

            });
    }
);