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