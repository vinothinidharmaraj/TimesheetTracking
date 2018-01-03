$(function () {
    $.post("/Activities/Index/" + $("#ProjectID").val(), function (data) {
        $("#activities").html(data);
    });


    $("#fromdatepicker").datepicker({
        daysOfWeekDisabled: [0, 6],
        beforeShowDay: disabledays,
        autoclose: true
    }).on("changeDate", function (e) {
        var newDate = e.date;
        var toDate = $("#todatepicker").val();
        if (!toDate) {
            $("#todatepicker").focus();
            return;
        }

        if (new Date(toDate).valueOf() < newDate.valueOf()) {
            alert("From date should be less than or equal to todate");
        }
        else {
            addWorkingHours();
        }
    });

    $("#todatepicker").datepicker({
        daysOfWeekDisabled: [0, 6],
        beforeShowDay: disabledays,
        autoclose: true
    }).on("changeDate", function (e) {
        var newDate = e.date;
        var fromDate = $("#fromdatepicker").val();
        if (!fromDate) {
            $("#fromdatepicker").focus();
            return;
        }

        if (new Date(fromDate).valueOf() > newDate.valueOf()) {
            alert("Todate should be greater than or equal to from date");
        }
        else {
            addWorkingHours();
        }
    });

    function disabledays(date) {
        /// get holidays from the backend
        var $data = $('#details-page\\.js').data();
        var publicHolidays = $data && $data.publicHolidays ? $data.publicHolidays.split(",") : null;
        var enable = true;
        if (publicHolidays && publicHolidays.length > 0) {
            $.each(publicHolidays, function () {
                if (date.valueOf() === new Date(this).valueOf()) {
                    enable = false;
                    return enable;
                }
            });
        }

        return enable;
    }

    function addWorkingHours() {
        var fromdate = $("#fromdatepicker").val();
        var todate = $("#todatepicker").val();
        var data;
        var request = $.ajax({
            url: "/projects/GetDatewiseData",
            method: "GET",
            //contentType: 'application/json',
            data: { id: 4, fromDate: fromdate, toDate: todate }
        });

        request.done(function (response) {
            data = response.data;
            var $table = $(".taskcontainer");
            $table.empty();
            appendHeader($table);
            appendBody($table, data);
            $(".savedata").attr("style", "display:block;");
        });

        request.fail(function (jqXHR, textStatus) {
            alert("Request failed: " + textStatus);
        });

        /* var dates = getDates(fromdate, todate);
         if (dates == null) {
             return;
         }*/
    };

    function convertStringArrayToDateArray(holidays) {
        if (!holidays) {
            return holidays;
        }

        var dateArray = [];

        $.each(holidays, function () {
            dateArray.push(new Date(this).getTime());
        });

        return dateArray;
    }

    function appendBody($table, existingData) {
        var $tbody = $("<tbody>");
        var $data = $('#details-page\\.js').data();
        var publicHolidays = $data && $data.publicHolidays ? $data.publicHolidays.split(",") : null;
        publicHolidays = convertStringArrayToDateArray(publicHolidays);
        var prevDate = null;

        var totalWorkinghrs = 0;
        $.each(existingData, function (index) {
            var $tr = $("<tr></tr>");
            $tr.addClass("initialRow");
            var $datetd = $("<td></td>");
            $datetd.text(this.ActivityDate);

            var $tasktd = $("<td></td>");
            if (!this.isPublicHoliday && !this.isWeekEnd) {
                var $taskinput = $("<input>").attr("type", "text");
                if (this.CanEdit) {
                    $taskinput.attr("readonly", true);
                }

                $taskinput.val(this.Name);
                $tasktd.append($taskinput);
            }

            var $workhrs = $("<td></td>");
            if (!this.isPublicHoliday && !this.isWeekEnd) {
                var $workhrsinput = $("<input>").attr("type", "number");
                if (this.CanEdit) {
                    $workhrsinput.attr("readonly", true);
                }

                totalWorkinghrs += this.NoOfHours;
                $workhrsinput.val(this.NoOfHours);
                $workhrs.append($workhrsinput);
            }

            var $addtd = $("<td></td>");
            if (!this.isPublicHoliday && !this.isWeekEnd) {
                if (prevDate == this.ActivityDate) {
                    var $a = $("<a></a>").addClass("glyphicon glyphicon-remove removetask");
                    $a.on("click", function () {
                        removeTask($(this));
                    });
                    $addtd.append($a);
                }
                else {
                    var $a = $("<a></a>");
                    if (this.CanEdit) {
                        $a.addClass("glyphicon glyphicon-edit edittask");
                        $a.attr("title", "Edit Task");
                        $a.on("click", function () {
                            editTask($(this));
                        });

                        var $asave = $("<a></a>");
                        $asave.addClass("glyphicon glyphicon-save savetask");
                        $asave.attr("title", "save Task");
                        $asave.attr("style", "display:none;");
                        $asave.on("click", function () {
                            addSingleTask($(this));
                        });
                        $addtd.append($asave);
                    }
                    else {
                        $a.addClass("glyphicon glyphicon-plus addtask");
                        $a.attr("title", "Add Task");
                        $a.on("click", function () {
                            bindAddTaskEvent($(this));
                        });
                    }

                    $addtd.append($a);
                }

                prevDate = this.ActivityDate;
            }

            if (this.isPublicHoliday || this.isWeekEnd) {
                $tr.attr("class", "holidayhighlight");
            }

            $tr.append($datetd);
            $tr.append($tasktd);
            $tr.append($workhrs);
            $tr.append($addtd);
            $tbody.append($tr);

        });

        var $totalHours = $('#totalHours');
        $totalHours.text(totalWorkinghrs);
        $table.append($tbody);
    }

    function editTask($this) {
        var $closesttr = $this.closest('tr');
        $closesttr.find('td').find('input').each(function (colIndex, c) {
            c.removeAttribute("readonly");
        });
        //$this[0].removeAttribute("class");
        $this[0].setAttribute("style", "display:none;");
        $closesttr.find('td').find('.savetask')[0].setAttribute('style', "display:block;");
        // $this[0].setAttribute("title", "Save");
    }

    function appendHeader($table) {
        var $thead = $("<thead></thead>");
        var $tr = $("<tr></tr>");
        var $dateth = $("<th></th>");
        $dateth.text("Date");

        var $taskth = $("<th></th>");
        $taskth.attr("width", "50%");
        $taskth.text("Task");

        var $workhrsth = $("<th></th>");
        $workhrsth.text("Working Hrs");

        var $emptyth = $("<th></th>");
        $emptyth.attr("width", "5%");
        $tr.append($dateth);
        $tr.append($taskth);
        $tr.append($workhrsth);
        $tr.append($emptyth);

        $thead.append($tr);
        $table.append($thead);
    }

    // Returns an array of dates between the two dates
    function getDates(startDate, endDate) {
        startDate = new Date(startDate);
        endDate = new Date(endDate);
        var dates = [],
            currentDate = startDate,
            addDays = function (days) {
                var date = new Date(this.valueOf());
                date.setDate(date.getDate() + days);
                return date;
            };
        while (currentDate <= endDate) {
            dates.push(currentDate);
            currentDate = addDays.call(currentDate, 1);
        }
        return dates;
    };

    function getCurrentDate() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        var today = yyyy + '-' + mm + '-' + dd;
        return today;
    }

    function bindAddTaskEvent($this) {
        var closesttr = $this.closest("tr");
        if (closesttr == null) {
            return;
        }

        var $row = getNewTaskTemplate();
        var $datetd = $row.find(":first");
        var firstRow = $this.closest(".initialRow");
        var $firsttd = firstRow == null ? null : firstRow.find(":first");
        $datetd.text($firsttd[0].innerText);
        $row.insertAfter(closesttr);
        $(".removetask").on("click", function () {
            removeTask($(this));
        });
    };

    function getNewTaskTemplate() {
        var $row = $("<tr>");
        var cols = "";
        cols += '<td></td>';
        cols += '<td><input type="text" /></td>';
        cols += '<td><input type="number"/></td>';
        cols += '<td><a class="glyphicon glyphicon-remove removetask" /></td>';
        $row.append(cols);
        return $row;
    }

    function removeTask($this) {
        if (($this) == null) {
            return;
        }

        var closesttr = $this.closest("tr");
        if (closesttr != null)
            closesttr.remove();
    }

    function readData() {
        var $table = $("table");
        var data = [];
        $table.find('tr').each(function (rowIndex, r) {
            var cols = [];
            $(this).find('td').find('input').each(function (colIndex, c) {
                cols.push(c.value);
            });

            $(this).find('td').each(function (colIndex, c) {
                cols.push(c.textContent);
            });
            data.push(cols);
        });

        return data;
    }

    function addSingleTask($this) {
        if ($this) {
            var data = [];
            var cols = [];
            var $closesttr = $this.closest('tr');
            $closesttr.find('td').find('input').each(function (colIndex, c) {
                cols.push(c.value);
            });

            $closesttr.find('td').each(function (colIndex, c) {
                cols.push(c.textContent);
            });

            data.push(cols);
        }

        var datas = convertArrayToJson(data);
        var request = $.ajax({
            url: "/Activities/AddActivities",
            method: "POST",
            //contentType: 'application/json',
            data: { activities: datas }
        });

        request.done(function (response) {
            if (response && response.success) {
                $this[0].removeAttribute('style');
                $this[0].setAttribute('style', 'display:none;');
                var edittask = $closesttr.find('td').find('.edittask');
                edittask[0].removeAttribute('style');
                edittask[0].setAttribute('style', 'display:block;');
                $closesttr.find('td').find('input').each(function (colIndex, c) {
                    c.setAttribute('readonly', true);
                });
            }
        });

        request.fail(function (jqXHR, textStatus) {
            alert("Request failed: " + textStatus);
        });
    }

    $("#savedata").on("click", function () {
        var tabledata = readData();
        var datas = convertArrayToJson(tabledata);
        var request = $.ajax({
            url: "/Activities/AddActivities",
            method: "POST",
            //contentType: 'application/json',
            data: { activities: datas }
        });

        request.done(function (msg) {
            $("#log").html(msg);
        });

        request.fail(function (jqXHR, textStatus) {
            alert("Request failed: " + textStatus);
        });
    });

    function convertArrayToJson(rows) {
        if (rows == null) {
            return;
        }

        var activities = [];
        var prevDate = null;
        var projectId = $("#ProjectID").val();
        $.each(rows, function () {
            var $this = $(this);
            var activity = {};
            if ($this != null && $this.length > 0) {
                if ($this.length >= 3) {
                    prevDate = $this[2];
                }

                if ($this.length >= 2) {
                    activity.Name = $this[0];
                    activity.NoOfHours = $this[1];
                    activity.ActivityDate = prevDate;
                    activity.ProjectId = projectId;
                }

                activities.push(activity);
            }
        });

        return activities;
    }
});