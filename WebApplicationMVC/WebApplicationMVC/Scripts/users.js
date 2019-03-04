$(document).ready(function () {
   
    $(".user").click(function () {

        var fullName = $(this).children(".FullName").text().split(" ");
        var Tel = $(this).children(".Tel").text();
        var Email = $(this).children(".Email").text();
        var Id = $(this).children("input[name='Id']").val();
        
        $("#editUser").find("input[name='LastName']").val(fullName[0]);
        $("#editUser").find("input[name='FirstName']").val(fullName[1]);
        $("#editUser").find("input[name='MiddleName']").val(fullName[2]);

        $("#editUser").find("input[name='Email']").val(Email);
        $("#editUser").find("input[name='Tel']").val(Tel);
        $("#editUser").find("input[name='Id']").val(Id);

        $("#editUser").dialog();
    });

    $("#DeleteUser").click(
        function () {
            var Id = $(this).parent().children("input[name='Id']").val();
            var ans = confirm("Видалити користувача? Після цієї операції він не увійде в систему.");
            if (ans == false) {
                return false;
            }
          
            $.post("/Employees/DeleteUser", { Id: Id }).done(
                function (data) {
                    alert("Користувача: " + data + " було видалено.");
                    location.reload();
                }
            );
        }
    );
});

