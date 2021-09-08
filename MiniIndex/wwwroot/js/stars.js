import $ from "jquery";

// Used in /Minis/Edit
$(document).ready(function () {
    RefreshTagsStart();
    RefreshTagsEnd();
});

$("#toggle-star").click(function () {
    if ($(this).hasClass("add-star")) {
        fetch("/api/Starred/" + document.getElementById("miniid").innerHTML, {
            method: "POST",
        });
    } else {
        fetch("/api/Starred/" + document.getElementById("miniid").innerHTML, {
            method: "DELETE",
        });
    }

    $(this).toggleClass("remove-star");
    $(this).toggleClass("add-star");

    $(this).toggleClass("btn-danger");
    $(this).toggleClass("btn-success");
});

// Used in /Admin/
$(".change-category").change(function () {
    var data = {
        ID: this.id * 1,
        Category: $(this).val() * 1,
    };

    fetch("/api/Tags/", {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    }).then((response) => {});

    return false;
});

// Used in /Tags/Manage
$(".remove-pair").click(function () {
    fetch("/api/Pairs/" + this.id, {
        method: "DELETE",
    }).then((response) => {});

    return false;
});

$(".new-pair").click(function () {
    fetch(
        "/api/Tags/" +
            this.id +
            "/Pairs/" +
            document.getElementById("new-pair-tag").value +
            "?type=" +
            document.getElementById("new-pair-type").value,
        {
            method: "POST",
        }
    ).then((response) => {});

    return false;
});

//When typing into the tag search box
//Hid all the .add-tag items that aren't the input text
$("#tagSearch").on("input", function (e) {
    $(".add-tag").hide();
    $(".add-tag-div").hide();
    var tagFilter = $(this).val().trim().toLowerCase();

    $(".add-tag").each(function () {
        if ($(this).text().toLocaleLowerCase().indexOf(tagFilter) >= 0) {
            $(this).show();
            $(this).parent().show();
        }
    });

    return false;
});

//When the user clicks "Add New Tag"
$("#AddNewTag").click(function () {
    RefreshTagsStart();

    var data = {
        Mini: {
            ID: document.getElementById("miniid").innerHTML * 1,
        },
        Tag: {
            TagName: $("#tagSearch").val(),
        },
    };

    fetch("/api/MiniTags/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    }).then((response) => {
        RefreshTagsEnd();
        $(this).fadeOut();
    });

    return false;
});

$("#UnusedTags").on("click", ".add-tag", function () {
    RefreshTagsStart();
    console.log("Adding " + this.innerHTML);

    var data = {
        Mini: {
            ID: document.getElementById("miniid").innerHTML * 1,
        },
        Tag: {
            ID: this.id * 1,
        },
    };

    fetch("/api/MiniTags/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    }).then((response) => {
        RefreshTagsEnd();
    });

    //TODO - Fix this to reset all .add-tags too

    return false;
});

$("#UsedTags").on("click", ".remove-tag", function () {
    RefreshTagsStart();
    console.log("Removing " + this.innerHTML);

    var data = {
        Mini: {
            ID: document.getElementById("miniid").innerHTML * 1,
        },
        Tag: {
            ID: this.id * 1,
        },
    };

    fetch("/api/MiniTags/", {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    }).then((response) => {
        RefreshTagsEnd();
    });
    return false;
});

function RefreshTagsStart() {
    $(".loading-spinner").show();
    $("#UsedTags").hide();
    $(".add-tag-div").show();
    $(".add-tag").show();
}

function RefreshTagsEnd() {
    $("#tagSearch").val("");

    $.getJSON({
        url:
            "/api/Minis/" +
            document.getElementById("miniid").innerHTML +
            "/Tags",
        complete: function (response, status) {},
        error: function () {
            //TODO - Error styling
        },
        success: function (response, status) {
            var newHTML = "";

            //console.log(response);

            var prevStatus = "";
            response.forEach(function (tag) {
                if (prevStatus != tag["Status"]) {
                    newHTML += "<h4>" + tag["Status"] + "</h4>";
                }
                newHTML +=
                    '<a href="#" id="' +
                    tag["id"] +
                    '" class="btn btn-outline-danger remove-tag ' +
                    tag["status"] +
                    '" style="margin-top:5px;">- <small>' +
                    tag["category"] +
                    ":</small> <b>" +
                    tag["tagName"] +
                    "</b></a>";
                prevStatus = tag["Status"];

                $(".add-tag#" + tag["ID"]).hide();
            });

            $("#UsedTags").html(newHTML);
            $("#UsedTags").show();

            //console.log(status + " - " + newHTML);
        },
    });
    $(".loading-spinner").hide();

    return false;
}
