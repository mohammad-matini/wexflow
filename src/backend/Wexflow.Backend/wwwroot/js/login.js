function Login() {

    var uri = Common.trimEnd(Settings.Uri, "/");
    var loginBtn = document.getElementById("btn-login");
    var usernameTxt = document.getElementById("txt-username");
    var passwordTxt = document.getElementById("txt-password");

    loginBtn.onclick = function () {
        login();
    };

    passwordTxt.onkeyup = function (event) {
        event.preventDefault();

        if (event.keyCode === 13) {
            login();
        }
    };

    function login() {

        var username = usernameTxt.value;
        var password = passwordTxt.value;
        var passwordHash = MD5(password);

        if (username === "" || password === "") {
            Common.toastInfo("Enter a valid username and password.");
        } else {
            Common.get(uri + "/user?qu=" + encodeURIComponent(username) + "&qp=" + encodeURIComponent(passwordHash) + "&username=" + encodeURIComponent(username), function (user) {
                if (typeof user === "undefined" || user === null) {
                    Common.toastError("Wrong credentials.");
                } else {
                    if (passwordHash === user.Password) {
                        authorize(username, passwordHash, user.UserProfile);
                        window.location.replace("dashboard.html");
                    } else {
                        Common.toastError("The password is incorrect.");
                    }

                }
            });
        }
    }

}