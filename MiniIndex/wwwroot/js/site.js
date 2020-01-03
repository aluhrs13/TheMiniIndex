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
        data: { id: this.parentElement.parentElement.id, category: $(this).children("option:selected").text()},
        complete: function () {
        },

    });
    
    return false;
});


$('#selectizedinput').selectize({
    valueField: 'tagName',
    labelField: 'tagName',
    searchField: 'tagName',
    plugins: ['remove_button'],
    delimiter: ',',
    persist: false,
    openOnFocus: false,
    closeAfterSelect: true,
    options: [],
    create: false,
    render: {
        option: function (item, escape) {
            return '<div>' +
                '<span class="title">' +
                '<span class="name">' + escape(item.tagName) + '</span>' +
                '</span>' +
                '</div>';
        }
    },
    load: function (query, callback) {
        if (!query.length) return callback();
        $.ajax({
            url: 'https://localhost:44386/Tags/JSONTagList',
            type: 'GET',
            dataType: 'json',
            data: {
            },
            error: function (req, err) {
                console.log(err);
                console.log("Error!");
                callback();
            },
            success: function (res) {
                callback(res);
            }
        });
    }
});


$('.add-to-filters').click(function () {
    var $select = $('#selectizedinput').selectize();
    var selectize = $select[0].selectize;

    selectize.addItem($(this).text());
    return false;
});