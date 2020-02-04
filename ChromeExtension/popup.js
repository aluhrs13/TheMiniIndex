let button = document.getElementById('button');

button.onclick = function(element) {
    chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
        var xhr = new XMLHttpRequest();

        xhr.open("GET", "https://theminiindex.com/api/minis/check?URL="+tabs[0].url, true);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.onreadystatechange = function() {
            if (xhr.readyState == "4") {
                //200 - let the user know it's already indexed
                if (xhr.status == "200") {
                    //200 - Mini exists, go to the Mini page.
                    chrome.tabs.create({ url: xhr.response });
                }
                else if(xhr.status == "404") {
                    //404 - Mini doesn't exist, go to indexing page.
                    chrome.tabs.create({ url: "https://theminiindex.com/Minis/Create?PlaceholderURL="+tabs[0].url });
                }else{

                }
            }
        }
        xhr.send();
    });
};