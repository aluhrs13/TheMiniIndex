document.getElementById("toggle-star").addEventListener("click", function () {
    if (this.classList.contains("add-star")) {
        fetch("/api/Starred/" + document.getElementById("miniid").innerHTML, {
            method: "POST",
        });
        this.innerText="Remove from Favorites"
    } else {
        fetch("/api/Starred/" + document.getElementById("miniid").innerHTML, {
            method: "DELETE",
        });
        this.innerText = "Add to Favorites"
    }

    this.classList.toggle("remove-star");
    this.classList.toggle("add-star");

    this.classList.toggle("btn-danger");
    this.classList.toggle("btn-success");
});

document.addEventListener('DOMContentLoaded', async function(){
    const urlParams = new URLSearchParams(window.location.search);
    const miniId = urlParams.get("id");

    const galleryElement = document.getElementById("related-minis");

    //TODO: Does this need to be awaited?
    await fetch(
        `/api/Minis/${miniId}/Related`
    )
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then((data) => {
            //TODO: Merge Mini/Gallery JS creation logic
            data.forEach((item) => {
                let newHTML = `
                    <div class="card" id="${item.id}">
                        <div>
                            <a href="/Minis/Details?id=${item.id}">
                                <img class="card-thumbnail" src="${item.thumbnail}" width="314" height="236"/>
                            </a>
                        </div>
                        <div class="card-text">
                            <div class="mini-name">
                                <h3>${item.name}</h3>
                                <h4>
                                by <a style="color:var(--app-primary-color)" href="/Creators/Details/?id=${item.creator.id}">${item.creator.name}</a>
                                </h4>
                            </div>
                        </div>
                    </div>
                `;
                galleryElement.insertAdjacentHTML("beforeend", newHTML);
            });

            galleryElement.classList.remove("hidden");
            document.getElementById("loading-spinner").classList.add("hidden");
        })
        .catch((error) => {
            console.error("Error getting related Minis:", error);
        });
})