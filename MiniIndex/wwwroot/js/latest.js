document.addEventListener("DOMContentLoaded", (event) => {
    var currentVisitTimestamp = getCookie("CurrentVisit");

    // Maintain a current visit and last visit. Every browse pageview will update CurrentVisit.
    // If the CurrentVisit is stale (2 hours), then we'll assume that's the last time the user was here.
    // Move that value to LastVisit and highlight Minis that are new since then.
    if (currentVisitTimestamp != "") {
        if (Date.now() - currentVisitTimestamp > (3600000 * 2)) {
            fetch(`/api/overhead/Session/?since=` + (Date.now() - currentVisitTimestamp))
            document.cookie = "LastVisit=" + currentVisitTimestamp + ";max-age=" + (60 * 60 * 24 * 28);
        }
    }

    updateDateCookie("CurrentVisit");
    var lastVisitTimestamp = getCookie("LastVisit");

    if (lastVisitTimestamp != "") {
        flagNewMinis(lastVisitTimestamp);
    } else {
        updateDateCookie("LastVisit");
        fetch(`/api/overhead/Session/?since=-1`)
    }
});

function flagNewMinis(lastVisitTimestamp) {
    var approvedMinis = document.getElementsByClassName("Approved");
    for (var i = 0, len = approvedMinis.length | 0; i < len; i = i + 1 | 0) {
        var approvedTime = approvedMinis[i].children[approvedMinis[i].children.length - 1].textContent

        if (Number(lastVisitTimestamp) < Number(approvedTime)) {
            approvedMinis[i].children[2].classList.remove('hidden');
        }
    }
}

function getCookie(cookieName) {
    if (document.cookie.split(';').some((item) => item.trim().startsWith(cookieName+'='))) {
        return document.cookie
            .split('; ')
            .find(row => row.startsWith(cookieName+'='))
            .split('=')[1];
    }

    return "";
}

function updateDateCookie(cookieName) {
    document.cookie = cookieName + "=" + Date.now() +";max-age="+(60*60*24*28);
}