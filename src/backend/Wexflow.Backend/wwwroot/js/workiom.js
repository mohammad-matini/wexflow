function WexflowManager() {
    "use strict";

    var id = "wf-manager";
    var uri = Common.trimEnd(Settings.Uri, "/");
    var lnkManager = document.getElementById("lnk-manager");
    var lnkWorkiom = document.getElementById("lnk-workiom");
    var lnkDesigner = document.getElementById("lnk-designer");
    var lnkApproval = document.getElementById("lnk-approval");
    var lnkUsers = document.getElementById("lnk-users");
    var selectedId = -1;
    var workflows = {};
    var timer = null;
    var timerInterval = 1000; // ms

    var html = "<div id='wf-container'>"
        + "<div id='wf-cmd'>"
        + "<button id='wf-start' type='button' class='btn btn-primary btn-xs'>Start</button>"
        + "<button id='wf-pause' type='button' class='btn btn-secondary btn-xs'>Suspend</button>"
        + "<button id='wf-resume' type='button' class='btn btn-secondary btn-xs'>Resume</button>"
        + "<button id='wf-stop' type='button' class='btn btn-danger btn-xs'>Stop</button>"
        + "</div>"
        + "<div id='wf-notifier'>"
        + "<input id='wf-notifier-text' type='text' name='fname' readonly>"
        + "</div>"
        + "<div id='wf-search'>"
        + "<div id='wf-search-text-container'>"
        + "<input id='wf-search-text' type='text' name='fname'>"
        + "</div>"
        + "<button id='wf-search-action' type='button' class='btn btn-primary btn-xs'>Search</button>"
        + "</div>"
        + "<div id='wf-workflows'>"
        + "</div>"
        + "</div>";

    document.getElementById(id).innerHTML = html;

    var startButton = document.getElementById("wf-start");
    var suspendButton = document.getElementById("wf-pause");
    var resumeButton = document.getElementById("wf-resume");
    var stopButton = document.getElementById("wf-stop");
    var searchButton = document.getElementById("wf-search-action");
    var searchText = document.getElementById("wf-search-text");
    var suser = getUser();


    if (suser === null || suser === "") {
        Common.redirectToLoginPage();
    } else {
        var user = JSON.parse(suser);
        Common.get(uri + "/user?username=" + encodeURIComponent(user.Username), function (u) {
            if (user.Password !== u.Password) {
                Common.redirectToLoginPage();
            } else {

                if (u.UserProfile === 0) {
                    lnkManager.style.display = "inline";
                    lnkWorkiom.style.display = "inline";
                    lnkDesigner.style.display = "inline";
                    lnkApproval.style.display = "inline";
                    lnkUsers.style.display = "inline";

                    var btnLogout = document.getElementById("btn-logout");
                    var divWorkflows = document.getElementById("wf-manager");
                    divWorkflows.style.display = "block";

                    btnLogout.onclick = function () {
                        deleteUser();
                        Common.redirectToLoginPage();
                    };
                    btnLogout.innerHTML = "Logout (" + u.Username + ")";

                    Common.disableButton(startButton, true);
                    Common.disableButton(suspendButton, true);
                    Common.disableButton(resumeButton, true);
                    Common.disableButton(stopButton, true);

                    searchButton.onclick = function () {
                        loadWorkflows();
                        notify("");
                        Common.disableButton(startButton, true);
                        Common.disableButton(suspendButton, true);
                        Common.disableButton(resumeButton, true);
                        Common.disableButton(stopButton, true);
                    };

                    searchText.onkeyup = function (event) {
                        event.preventDefault();

                        if (event.keyCode === 13) { // Enter
                            loadWorkflows();
                            notify("");
                            Common.disableButton(startButton, true);
                            Common.disableButton(suspendButton, true);
                            Common.disableButton(resumeButton, true);
                            Common.disableButton(stopButton, true);
                        }
                    };

                    loadWorkflows();
                } else {
                    Common.redirectToLoginPage();
                }

            }
        });
    }

   

    function compareById(wf1, wf2) {
        if (wf1.Id < wf2.Id) {
            return -1;
        } else if (wf1.Id > wf2.Id) {
            return 1;
        }
        return 0;
    }

    function launchType(lt) {
        switch (lt) {
            case 0:
                return "Startup";
            case 1:
                return "Trigger";
            case 2:
                return "Periodic";
            case 3:
                return "Cron";
            default:
                return "";
        }
    }

    function loadWorkflows() {
        Common.get(uri + "/searchWithRestParams?s=" + encodeURIComponent(searchText.value), function (data) {
            data.sort(compareById);
            var items = [];
            var i;
            for (i = 0; i < data.length; i++) {
                var val = data[i];
                workflows[val.Id] = val;
                var lt = launchType(val.LaunchType);
                items.push("<tr>"
                    + "<td class='wf-id' title='" + val.Id + "'>" + val.Id + "</td>"
                    + "<td class='wf-n' title='" + val.Name + "'>" + val.Name + "</td>"
                    + "<td class='wf-lt'>" + lt + "</td>"
                    + "<td class='wf-e'><input type='checkbox' readonly disabled " + (val.IsEnabled ? "checked" : "") + "></input></td>"
                    + "<td class='wf-a'><input type='checkbox' readonly disabled " + (val.IsApproval ? "checked" : "") + "></input></td>"
                    + "<td class='wf-d' title='" + val.Description + "'>" + val.Description + "</td>"
                    + "</tr>");

            }

            var table = "<table id='wf-workflows-table' class='table'>"
                + "<thead class='thead-dark'>"
                + "<tr>"
                + "<th class='wf-id'>Id</th>"
                + "<th class='wf-n'>Name</th>"
                + "<th class='wf-lt'>LaunchType</th>"
                + "<th class='wf-e'>Enabled</th>"
                + "<th class='wf-a'>Approval</th>"
                + "<th class='wf-d'>Description</th>"
                + "</tr>"
                + "</thead>"
                + "<tbody>"
                + items.join("")
                + "</tbody>"
                + "</table>";

            document.getElementById("wf-workflows").innerHTML = table;

            var workflowsTable = document.getElementById("wf-workflows-table");
            var descriptions = document.getElementsByClassName("wf-d");
            for (i = 0; i < descriptions.length; i++) {
                descriptions[i].style.width = workflowsTable.offsetWidth - (45 + 200 + 100 + 75 + 16 * 5 + 17) + "px";
            }

            function getWorkflow(wid, func) {
                Common.get(uri + "/workflow/" + wid, function (d) {
                    func(d);
                });
            }

            function updateButtons(wid, force) {
                getWorkflow(wid, function (workflow) {
                    if (workflow.IsEnabled === false) {
                        notify("This workflow is disabled.");
                        Common.disableButton(startButton, true);
                        Common.disableButton(suspendButton, true);
                        Common.disableButton(resumeButton, true);
                        Common.disableButton(stopButton, true);
                    }
                    else {
                        if (force === false && workflowStatusChanged(workflow) === false) return;

                        Common.disableButton(startButton, workflow.IsRunning);
                        Common.disableButton(stopButton, !(workflow.IsRunning && !workflow.IsPaused));
                        Common.disableButton(suspendButton, !(workflow.IsRunning && !workflow.IsPaused));
                        Common.disableButton(resumeButton, !workflow.IsPaused);

                        if (workflow.IsApproval === true && workflow.IsWaitingForApproval === true && workflow.IsPaused === false) {
                            notify("This workflow is waiting for approval...");
                        } else {
                            if (workflow.IsRunning === true && workflow.IsPaused === false) {
                                notify("This workflow is running...");
                            }
                            else if (workflow.IsPaused === true) {
                                notify("This workflow is suspended.");
                            } else {
                                notify("");
                            }
                        }
                    }
                });
            }

            function workflowStatusChanged(workflow) {
                var changed = workflows[workflow.Id].IsRunning !== workflow.IsRunning || workflows[workflow.Id].IsPaused !== workflow.IsPaused || workflows[workflow.Id].IsWaitingForApproval !== workflow.IsWaitingForApproval;
                workflows[workflow.Id].IsRunning = workflow.IsRunning;
                workflows[workflow.Id].IsPaused = workflow.IsPaused;
                workflows[workflow.Id].IsWaitingForApproval = workflow.IsWaitingForApproval;
                return changed;
            }

            var rows = (workflowsTable.getElementsByTagName("tbody")[0]).getElementsByTagName("tr");
            for (i = 0; i < rows.length; i++) {
                rows[i].onclick = function () {
                    selectedId = parseInt(this.getElementsByClassName("wf-id")[0].innerHTML);

                    var selected = document.getElementsByClassName("selected");
                    if (selected.length > 0) {
                        selected[0].className = selected[0].className.replace("selected", "");
                    }

                    this.className += "selected";

                    clearInterval(timer);

                    if (workflows[selectedId].IsEnabled === true) {
                        timer = setInterval(function () {
                            updateButtons(selectedId, false);
                        }, timerInterval);

                        updateButtons(selectedId, true);
                    } else {
                        updateButtons(selectedId, true);
                    }
                };
            }

            // Start with REST params
            startButton.onclick = function () {
                // POST URL: http://localhost:8000/wexflow/startWithRestParams?workflowId=
                var startUri = uri + "/startWithRestParams?workflowId=" + selectedId;
                var json = [
                    {
                        "ParamName": "Mapping",
                        "ParamValue":
                        {
                            "Authorization": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEwNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhZG1pbiIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiNjJiNDM4OTctOWFjNC00ZDMwLTkyMzEtMTk5MzhmNTZlNjQ4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJodHRwOi8vd3d3LmFzcG5ldGJvaWxlcnBsYXRlLmNvbS9pZGVudGl0eS9jbGFpbXMvdGVuYW50SWQiOiI3OSIsInN1YiI6IjEwNiIsImp0aSI6IjRmZjhmZTI2LWY1NTUtNGUwMC04MDkxLTQ1OGFjMWYyMDkxZiIsImlhdCI6MTU2ODAzNzE1MCwidG9rZW5fdmFsaWRpdHlfa2V5IjoiOTQzMjI4NjItNDllZS00MmNkLTgyNzMtMmRlMjYyOWRmOTRiIiwidXNlcl9pZGVudGlmaWVyIjoiMTA2QDc5IiwibmJmIjoxNTY4MDM3MTUwLCJleHAiOjE1NjgxMjM1NTAsImlzcyI6Ikxpc3R1cmUiLCJhdWQiOiJMaXN0dXJlIn0.tm5Spt3zYd4ezJzD_XygpdRvOMk_nU-kWMpvkrcQcqg",
                            "listId": "a1e8d575-75f1-48d5-c29e-08d733bfc9d4",
                            "Payload": { "61066": "test" }
                        }
                    }
                ];

                Common.post(startUri, function (res) {
                    if (res === false) {
                        Common.toastError("An error occured while starting the workflow " + selectedId + ".");
                    } else {
                        Common.toastSuccess("Workflow " + selectedId + " started with success.");
                    }
                }, function () {
                        Common.toastError("An error occured while starting the workflow " + selectedId + ".");
                }, json);
            };

            suspendButton.onclick = function () {
                var suspendUri = uri + "/suspend/" + selectedId;
                Common.post(suspendUri, function (res) {
                    if (res === true) {
                        updateButtons(selectedId, true);
                    } else{
                        Common.toastInfo("This operation is not supported.");
                    }
                });
            };

            resumeButton.onclick = function () {
                var resumeUri = uri + "/resume/" + selectedId;
                Common.post(resumeUri);
            };

            stopButton.onclick = function () {
                var stopUri = uri + "/stop/" + selectedId;
                Common.post(stopUri,
                    function (res) {
                        if (res === true) {
                            updateButtons(selectedId, true);
                        } else {
                            Common.toastInfo("This operation is not supported.");
                        }
                    });
            };

            // End of get workflows
        }, function () {
                Common.toastError("An error occured while retrieving workflows. Check that Wexflow server is running correctly.");
        });
    }

    function notify(msg) {
        document.getElementById("wf-notifier-text").value = msg;
    }
}