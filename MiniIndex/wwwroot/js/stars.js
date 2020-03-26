import $ from 'jquery'

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

$('.add-tag').click(function () {
    document.getElementById("AddedTags").innerHTML = document.getElementById("AddedTags").innerHTML.concat("<span class='badge badge-success'>" + this.innerHTML.substr(2, this.innerHTML.length) + "</span>");
    console.log("Adding " + this.innerHTML);
    $(this).fadeOut();
    $.get({
        url: "/MiniTags/Create/",
        data: { mini: document.getElementById("miniid").innerHTML, tag: this.id },
        complete: function () {
        },
    });

    $('#tagSearch').val('');
    return false;
});

$('.remove-tag').click(function () {
    //document.getElementById("UnusedTags").innerHTML = document.getElementById("UnusedTags").innerHTML.concat("<span class='badge badge-success'>" + this.innerHTML.substr(2, this.innerHTML.length) + "</span>");

    console.log("Removing " + this.innerHTML);
    $.get({
        url: "/MiniTags/Delete/",
        data: { mini: document.getElementById("miniid").innerHTML, tag: this.id },
        complete: function () {
        },
    });
    $(this).hide();
    return false;
});

$('#tagSearch').on('input', function (e) {
    $('.add-tag').hide();
    $('.add-tag-div').hide();
    var tagFilter = $(this).val().trim().toLowerCase();

    $('.add-tag').each(function () {
        if ($(this).text().toLocaleLowerCase().indexOf(tagFilter) >= 0) {
            $(this).show();
            $(this).parent().show();
        }
    });

    return false;
});

$('#AddNewTag').click(function () {
    var newTag = $("#tagSearch").val();
    document.getElementById("UsedTags").innerHTML = document.getElementById("UsedTags").innerHTML.concat("<span class='badge badge-success'>" + newTag + "</span>");
    $.get({
        url: "/MiniTags/Create/",
        data: { mini: document.getElementById("miniid").innerHTML, tagName: newTag },
        complete: function () {
        },
    });
    return false;
});

$('.change-category').change(function () {
    $.get({
        url: "/Tags/Edit/",
        data: { id: this.id, category: $(this).children("option:selected").text() },
        complete: function () {
        },
    });

    return false;
});


$('.remove-pair').click(function () {
    $.get({
        url: "/TagPairs/Delete/",
        data: { id: this.id },
        complete: function () {
        },
    });
    return false;
});

$('.new-pair').click(function () {
    $.get({
        url: "/TagPairs/Create/",
        data: { tag1: this.id, tag2: document.getElementById("new-pair-tag").value, type: document.getElementById("new-pair-type").value },
        complete: function () {
        },
    });
    return false;
});