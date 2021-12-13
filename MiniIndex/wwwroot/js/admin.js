let changeCat = document.querySelectorAll(".change-category");
let changeCatArray = Array.prototype.slice.call(changeCat);

changeCatArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
        var data = {
            ID: this.id * 1,
            Category: this.value * 1,
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
});

let removePair = document.querySelectorAll(".remove-pair");
let removePairArray = Array.prototype.slice.call(removePair);

removePairArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
        fetch("/api/Pairs/" + this.id, {
            method: "DELETE",
        }).then((response) => { });

        return false;
    });
});

let addPair = document.querySelectorAll(".new-pair");
let addPairArray = Array.prototype.slice.call(addPair);

addPairArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
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
});