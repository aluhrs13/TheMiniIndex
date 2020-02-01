$('#toggle-star').click(function () {
    if ($(this).hasClass("add-star")) {
        console.log("Starring " + this.innerHTML);

        $.get({
            url: "/Starred/Create/",
            data: { mini: document.getElementById("miniid").innerHTML },
            complete: function () {
            },
        });
    } else {
        console.log("Unstarring " + this.innerHTML);

        $.get({
            url: "/Starred/Delete/",
            data: { mini: document.getElementById("miniid").innerHTML },
            complete: function () {
            },
        });
    }
    $(this).toggleClass("remove-star");
    $(this).toggleClass("add-star");

    $(this).toggleClass("btn-danger");
    $(this).toggleClass("btn-success");

    return false;
});
