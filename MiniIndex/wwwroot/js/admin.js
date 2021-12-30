let changeCat = document.querySelectorAll(".change-category");
let changeCatArray = Array.prototype.slice.call(changeCat);

changeCatArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
        var data = {
            ID: document.getElementById("miniid").value * 1,
            Category: this.value * 1,
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
});

if (document.getElementById("updateNameBtn")) {
    document.getElementById("updateNameBtn").addEventListener("click", () => {
        var data = {
            ID: document.getElementById("miniid").value * 1,
            TagName: document.getElementById("newName").value,
        };

        fetch("/api/Tags/", {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        }).then((response) => {
            location.reload();
        });
    });
}

let removePair = document.querySelectorAll(".remove-pair");
let removePairArray = Array.prototype.slice.call(removePair);

removePairArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
        fetch("/api/Pairs/" + this.id, {
            method: "DELETE",
        }).then((response) => {
            document.getElementById(this.id).remove();
        });

        return false;
    });
});

let addPair = document.querySelectorAll(".new-pair");
let addPairArray = Array.prototype.slice.call(addPair);

addPairArray.forEach(function (ele) {
    ele.addEventListener("click", function () {
        var newPairTag = document.getElementById("new-pair-tag");
        var newPairType = document.getElementById("new-pair-type");
        fetch(
            "/api/Tags/" +
                document.getElementById("miniid").value +
                "/Pairs/" +
                newPairTag.value +
                "?type=" +
                newPairType.value,
            {
                method: "POST",
            }
        ).then((response) => {
            var newHTML = "";
            newHTML += `
            <tr>
                <td></td>
                <td>
                    ${newPairType.options[newPairType.selectedIndex].text}
                </td>
                <td>
                    ${newPairTag.options[newPairTag.selectedIndex].text}
                </td>
                <td>
                </td>
            </tr>
            `;
            document
                .getElementById("pairTable")
                .insertAdjacentHTML("afterbegin", newHTML);
        });

        return false;
    });
});
