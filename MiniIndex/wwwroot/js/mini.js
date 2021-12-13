import $ from "jquery";

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
