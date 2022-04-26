document.addEventListener("DOMContentLoaded", () => {
    RefreshTagsStart();
    RefreshTagsEnd();
});

//When typing into the tag search box
//Hide all the .add-tag items that aren't the input text
document.getElementById("tagSearch").addEventListener("input", function (e) {
    e.preventDefault();

    let addTagHeaders = document.querySelectorAll(".add-tag-div");
    let addTagHeadersArray = Array.prototype.slice.call(addTagHeaders);
    addTagHeadersArray.forEach(function (ele) {
        hideEle(ele);
    });

    var tagFilter = this.value.trim().toLowerCase();

    let addTagBtns = document.querySelectorAll(".add-tag");
    let addTagBtnsArray = Array.prototype.slice.call(addTagBtns);
    addTagBtnsArray.forEach(function (ele) {
        if (ele.innerText.toLocaleLowerCase().indexOf(tagFilter) >= 0) {
            showEle(ele);
            showEle(ele.parentNode);
        } else {
            hideEle(ele);
        }
    });
    return false;
});

//Add a new tag with what's in the search box
document.getElementById("AddNewTag").addEventListener("click", function (e) {
    e.preventDefault();
    RefreshTagsStart();

    var data = {
        Mini: {
            ID: document.getElementById("miniid").innerHTML * 1,
        },
        Tag: {
            TagName: document.getElementById("tagSearch").value,
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
    return false;
});

//Add listeners to every "Add Tag" button
//TODO: Should this listen on #UnusedTags then parse through that instead of a lot of listeners?
let addTagBtns = document.querySelectorAll(".add-tag");
let addTagBtnsArray = Array.prototype.slice.call(addTagBtns);

addTagBtnsArray.forEach(function (ele) {
    ele.addEventListener("click", function (e) {
        e.preventDefault();
        RefreshTagsStart();

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
    });
});

//Add listeners to every "Remove Tag" button.
//Separate function since it needs to be run after current tags is propagated
function RefreshEventListeners() {
    let removeTagBtns = document.querySelectorAll(".remove-tag");
    let removeTagBtnsArray = Array.prototype.slice.call(removeTagBtns);

    removeTagBtnsArray.forEach(function (ele) {
        ele.addEventListener("click", function (e) {
            e.preventDefault();
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
        });
    });
}

//Hide buttons and show spinner when refreshing tags
//Also sets the height of the container to prevent a bouncy shift when there's a lot of tags
function RefreshTagsStart() {
    var tagContainerEle = document.getElementById("tagContainer");
    tagContainerEle.style.height = tagContainerEle.clientHeight + "px";
    showEle(document.getElementById("loading-spinner"));
    hideEle(document.getElementById("UsedTags"));

    let addTagHeaders = document.querySelectorAll(".add-tag-div");
    let addTagHeadersArray = Array.prototype.slice.call(addTagHeaders);
    addTagHeadersArray.forEach(function (ele) {
        showEle(ele);
    });

    let addTagBtns = document.querySelectorAll(".add-tag");
    let addTagBtnsArray = Array.prototype.slice.call(addTagBtns);
    addTagBtnsArray.forEach(function (ele) {
        showEle(ele);
    });
}

//Reset everything and show the current existing tags
function RefreshTagsEnd() {
    document.getElementById("tagSearch").value = "";

    fetch(
        "/api/Minis/" + document.getElementById("miniid").innerHTML + "/Tags",
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        }
    ).then((response) => {
        var newHTML = "";
        var prevStatus = "";
        document.getElementById("UsedTags").innerHTML = "";

        response.json().then((data) => {
            data.forEach(function (tag) {
                if (prevStatus != tag["status"]) {
                    newHTML += "<h3>" + tag["status"] + "</h3>";
                }
                newHTML +=
                    '<a href="#" id="' +
                    tag["id"] +
                    '" class="btn remove-tag ' +
                    tag["status"] +
                    '">- <small>' +
                    tag["category"] +
                    ":</small> <b>" +
                    tag["tagName"] +
                    "</b></a>";
                prevStatus = tag["status"];

                var existingEle = document.getElementById(tag["id"]);
                if (existingEle) {
                    existingEle.remove();
                }
            });

            document.getElementById("UsedTags").innerHTML = newHTML;
            RefreshEventListeners();
            document
                .getElementById("tagContainer")
                .style.removeProperty("height");
            showEle(document.getElementById("UsedTags"));
            hideEle(document.getElementById("loading-spinner"));
            return false;
        });
    });
}

function hideEle(ele) {
    if (!ele.classList.contains("hidden")) {
        ele.classList.add("hidden");
    }
}

function showEle(ele) {
    if (ele.classList.contains("hidden")) {
        ele.classList.remove("hidden");
    }
}
