import $ from 'jquery'

// Used in /Minis/Details
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


// Used in /Admin/
$('.change-category').change(function () {
    $.get({
        url: "/Tags/Edit/",
        data: { id: this.id, category: $(this).children("option:selected").text() },
        complete: function () {
        },
    });

    return false;
});

// Used in /Tags/Manage
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


// Used in /Minis/Edit

$(document).ready(function () {
    RefreshTagsStart();
    RefreshTagsEnd();
});

//When typing into the tag search box
//Hid all the .add-tag items that aren't the input text
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

//When the user clicks "Add New Tag"
$('#AddNewTag').click(function () {
    RefreshTagsStart();

    var newTag = $("#tagSearch").val();
    console.log("Adding new tag " + newTag);

    $.get({
        url: "/MiniTags/Create/",
        data: { mini: document.getElementById("miniid").innerHTML, tagName: newTag },
        complete: function () {
            RefreshTagsEnd();
        },
        error: function () {
            //Error styling
        },
        success: function () {
            //Success styling
            $(this).fadeOut();
        },
    });
    return false;
});

$('#UnusedTags').on('click', '.add-tag', function () {
    RefreshTagsStart();
    console.log("Adding " + this.innerHTML);

    $.get({
        url: "/MiniTags/Create/",
        data: { mini: document.getElementById("miniid").innerHTML, tag: this.id },
        complete: function () {
            RefreshTagsEnd();
        },
        error: function () {
            //TODO - Error styling
        },
        success: function () {
        }
    });

    //TODO - Fix this to reset all .add-tags too

    return false;
});

$('#UsedTags').on('click', '.remove-tag', function (){
    RefreshTagsStart();
    console.log("Removing " + this.innerHTML);

    $.get({
        url: "/MiniTags/Delete/",
        data: { mini: document.getElementById("miniid").innerHTML, tag: this.id },
        complete: function () {
            RefreshTagsEnd();
        },
        error: function () {
            //TODO - Error styling
        },
        success: function () {
            $(this).fadeOut();
        },
    });
    $(this).hide();
    return false;
});

function RefreshTagsStart() {
    //$('.loading-spinner').show();
    $('#UsedTags').hide();

    $('#tagSearch').val('');
    $('.add-tag-div').show();
    $('.add-tag').show();
}

function RefreshTagsEnd() {
    //$('.loading-spinner').hide();
    $.getJSON({
        url: "/api/minis/tagList/",
        data: { id: document.getElementById("miniid").innerHTML },
        complete: function (response, status) {
        },
        error: function () {
            //TODO - Error styling
        },
        success: function (response, status) {
            var newHTML="";

            //console.log(response);

            var prevStatus = "";
            response.forEach(function (tag) {
                if (prevStatus != tag['Status']) {
                    newHTML += '<h4>'+tag['Status']+'</h4>';
                }
                newHTML += '<a href="#" id="' + tag['ID'] + '" class="btn btn-outline-danger remove-tag ' + tag['Status'] + '" style="margin-top:5px;">- <small>' + tag['Category'] + ':</small> <b>' + tag['TagName'] + '</b></a>';
                prevStatus = tag['Status'];

                $('.add-tag#' + tag['ID']).hide();
            });

            $('#UsedTags').html(newHTML);
            $('#UsedTags').show();

            //console.log(status + " - " + newHTML);
        },
    });

    return false;
}