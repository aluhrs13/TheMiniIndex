document.addEventListener("DOMContentLoaded", (event) => {
    var currentVisitTimestamp = getCookie("CurrentVisit");

    // Maintain a current visit and last visit. Every browse pageview will update CurrentVisit.
    // If the CurrentVisit is stale (2 hours), then we'll assume that's the last time the user was here.
    // Move that value to LastVisit and highlight Minis that are new since then.
    if (currentVisitTimestamp != "") {
        if (Date.now() - currentVisitTimestamp > 3600000 * 2) {
            fetch(
                `/api/overhead/Session/?since=` +
                    (Date.now() - currentVisitTimestamp)
            );
            document.cookie =
                "LastVisit=" +
                currentVisitTimestamp +
                ";SameSite=Strict;max-age=" +
                60 * 60 * 24 * 28;
        }
    }

    updateDateCookie("CurrentVisit");
    var lastVisitTimestamp = getCookie("LastVisit");

    if (lastVisitTimestamp != "") {
        flagNewMinis(lastVisitTimestamp);
    } else {
        updateDateCookie("LastVisit");
        fetch(`/api/overhead/Session/?since=-1`);
    }

    //TODO: This fires later than I'd expect (after images load)
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get('SearchString');
    if (myParam) {
        document.getElementById("SearchString").value = myParam;
    }

});

function flagNewMinis(lastVisitTimestamp) {
    var approvedMinis = document.getElementsByClassName("Approved");
    for (var i = 0, len = approvedMinis.length | 0; i < len; i = (i + 1) | 0) {
        var approvedTime =
            approvedMinis[i].children[approvedMinis[i].children.length - 1]
                .textContent;

        if (Number(lastVisitTimestamp) < Number(approvedTime)) {
            approvedMinis[i].children[2].classList.remove("hidden");
        }
    }
}

function getCookie(cookieName) {
    if (
        document.cookie
            .split(";")
            .some((item) => item.trim().startsWith(cookieName + "="))
    ) {
        return document.cookie
            .split("; ")
            .find((row) => row.startsWith(cookieName + "="))
            .split("=")[1];
    }

    return "";
}

function updateDateCookie(cookieName) {
    document.cookie =
        cookieName +
        "=" +
        Date.now() +
        ";SameSite=Strict;max-age=" +
        60 * 60 * 24 * 28;
}

window.addEventListener("hashchange", (event) => {
    searchMinis();
});

function nextPage() {
    if (window.location.hash == "") {
        window.location.hash = 2;
    } else {
        window.location.hash = Number(window.location.hash.substr(1)) + 1;
    }
}

async function searchMinis() {
    var searchString = document.getElementById("SearchString").value;
    let galleryElement = document.getElementById("gallery");

    //Reset If needed
    /*
    var currentParams = new URLSearchParams(
        document.location.search.substring(1)
    );

    if (currentParams.get("searchString") != searchString) {
        currentParams.set("searchString", searchString);
        window.history.pushState(
            "",
            "",
            document.location + "/?" + currentParams
        );
        galleryElement.innerHTML = "";
    }
    */
    if (window.location.hash.startsWith("#")) {
        pageIndex = window.location.hash.substr(1);
    } else {
        pageIndex = 1;
    }

    //Handle page refresh or back scenario
    if (galleryElement.children.length == 0 && pageIndex > 1) {
        //TODO: Loop through pages here
        for (x = 1; x <= pageIndex; x++) {
            await fetchStuff(galleryElement, searchString, x);
        }
    } else {
        fetchStuff(galleryElement, searchString, pageIndex);
    }
}

async function fetchStuff(galleryElement, searchString, pageIndex) {
    //TODO: Await is for refresh case, does it cause perf problems in normal case?
    await fetch(
        `/api/Minis?pageIndex=${pageIndex}&SearchString=${searchString}`
    )
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then((data) => {
            //TODO: Is CreateElement or insertAdjacentHTML better?
            //https://developer.mozilla.org/en-US/docs/Web/API/Document/createElement
            //let newCard = document.createElement("tmi-mini-card");

            //TODO: Map or ForEach?
            //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/forEach
            data.forEach((item) => {
                var urlParts = item.link
                    .replace("http://", "")
                    .replace("https://", "")
                    .split(/[/?#]/)[0]
                    .split(".");

                const sourceSite = urlParts[urlParts.length - 2];

                const newHTML = `
            <tmi-mini-card
                miniid="${item.id}"
                name="${item.name}"
                thumbnail="${item.thumbnail}"
                status="${item.status}"
                creatorname="${item.creator.name}"
                creatorid="${item.creator.id}"
                sourcesite="${sourceSite}"
            >
            </tmi-mini-card>
        `;

                galleryElement.insertAdjacentHTML("beforeend", newHTML);
            });
        })
        .catch((error) => {
            console.error("Error getting Minis:", error);
        });
}
