document.addEventListener("DOMContentLoaded", (event) => {
  //https://developer.mozilla.org/en-US/docs/Web/API/Document/DOMContentLoaded_event
  searchMinis();
});

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

function searchMinis() {
  var searchString = document.getElementById("search-box").value;
  let galleryElement = document.getElementById("gallery");

  //Reset If needed
  var currentParams = new URLSearchParams(
    document.location.search.substring(1)
  );

  if (currentParams.get("searchString") != searchString) {
    currentParams.set("searchString", searchString);
    window.history.pushState(
      "",
      "",
      document.location.origin + "/?" + currentParams
    );
    galleryElement.innerHTML = "";
  }

  if (window.location.hash.startsWith("#")) {
    pageIndex = window.location.hash.substr(1);
  } else {
    pageIndex = 1;
  }

  fetch(
    `https://theminiindex.com/api/minis/search?pageIndex=${pageIndex}&SearchString=${searchString}`
  )
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      console.log(data);

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
