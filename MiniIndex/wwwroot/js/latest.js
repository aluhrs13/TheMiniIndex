window.addEventListener("hashchange", (event) => {
    searchMinis();
});

document.addEventListener("DOMContentLoaded", (event) => {
    //Next Page Listener
    var nextPageBtn = document.getElementById("nextPageBtn");
    if (nextPageBtn) {
        nextPageBtn.addEventListener("click", nextPage);
    }

    //Set the search box text. This needs to be above the searchMinis() below.
    //TODO: This fires later than I'd expect (after images load)
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get("SearchString");
    if (myParam) {
        document.getElementById("SearchString").value = myParam;
    }

    //We came here from a back button or something. Do something kinda smart and load the latest page
    if (window.location.hash.startsWith("#")) {
        searchMinis(true);
    }

    // Maintain a current visit and last visit. Every browse pageview will update CurrentVisit.
    // If the CurrentVisit is stale (2 hours), then we'll assume that's the last time the user was here.
    // Move that value to LastVisit and highlight Minis that are new since then.
    var currentVisitTimestamp = getCookie("CurrentVisit");
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
    flagNewMinis();
});

function flagNewMinis() {
    var lastVisitTimestamp = getCookie("LastVisit");

    if (lastVisitTimestamp != "") {
        var approvedMinis = document.getElementsByClassName("Approved");
        for (
            var i = 0, len = approvedMinis.length | 0;
            i < len;
            i = (i + 1) | 0
        ) {
            var approvedTime =
                approvedMinis[i].children[approvedMinis[i].children.length - 1]
                    .innerText;

            if (Number(lastVisitTimestamp) < Number(approvedTime)) {
                approvedMinis[i].children[
                    approvedMinis[i].children.length - 2
                ].classList.remove("hidden");
            }
        }
    } else {
        updateDateCookie("LastVisit");
        fetch(`/api/overhead/Session/?since=-1`);
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

async function searchMinis(freshLoad) {
    var searchString = document.getElementById("SearchString").value;
    let galleryElement = document.getElementById("gallery");
    var pageIndex = 1;

    if (window.location.hash.startsWith("#")) {
        pageIndex = window.location.hash.substr(1);
    }

    //TODO: There's a bit of layout shifting here, probably can refactor to make it cleaner
    //Handle page refresh or back scenario
    if (freshLoad) {
        galleryElement.replaceChildren();
        var backButtonHTML = `
                <div class="card" align="center">
                    <div style="display:flex; flex-direction:column; justify-content:center; height:100%;">
                        Resuming previous search from where you last left off...
                        <br/><br/>
                        <a href="/Minis?SearchString=${searchString}" class="btn style-primary-border">Restart from first page</a>
                    </div>
                </div>
            `;
        galleryElement.insertAdjacentHTML("beforeend", backButtonHTML);
    }

    fetchStuff(galleryElement, searchString, pageIndex);
}

async function fetchStuff(galleryElement, searchString, pageIndex) {
    //TODO: Does this need to be awaited?
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
            if (data.length == 0) {
                document.getElementById("nextPageBtn").classList.add("hidden");
            }

            //TODO: Is CreateElement or insertAdjacentHTML better?
            //https://developer.mozilla.org/en-US/docs/Web/API/Document/createElement

            //TODO: Map or ForEach?
            //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/forEach
            data.forEach((item) => {
                let newHTML = `
                    <div class="card ${item.status}">
                        <div>
                            <a href="/Minis/Details?id=${item.id}">
                                <img class="card-thumbnail" src="${item.thumbnail}" width="314" height="236"/>
                            </a>
                        </div>`;
                if (item.status == "Pending") {
                    newHTML += `<div class="mini-banner">
                            This Mini needs tags!
                        </div>`;
                }
                if (item.status == "Rejected" || item.status == "Deleted") {
                    newHTML += `<div class="mini-banner style-danger">
                            There is something wrong with this Mini.
                        </div>`;
                }
                newHTML += `<div class="card-text">
                            <div class="mini-name">
                                <h3>${item.name}</h3>
                                <h4>
                                by <a style="color:var(--app-primary-color)" href="/Creators/Details/?id=${item.creator.id}">${item.creator.name}</a>
                                </h4>
                            </div>
                        </div>
                        <div class="new-tag hidden"><span class="new-tag-span">New!</span></div>
                        <div class="approved-time">${item.linuxTime}</div>
                    </div>
                `;
                galleryElement.insertAdjacentHTML("beforeend", newHTML);

                flagNewMinis();
            });
        })
        .catch((error) => {
            console.error("Error getting Minis:", error);
        });
}

function nextPage(e) {
    e.preventDefault();
    if (window.location.hash == "") {
        window.location.hash = 2;
    } else {
        window.location.hash = Number(window.location.hash.substr(1)) + 1;
    }
}
