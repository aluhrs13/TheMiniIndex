import $ from "jquery";

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
    }).then((response) => { });

    return false;
});

$(".remove-pair").click(function () {
    fetch("/api/Pairs/" + this.id, {
        method: "DELETE",
    }).then((response) => { });

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
    ).then((response) => { });

    return false;
});