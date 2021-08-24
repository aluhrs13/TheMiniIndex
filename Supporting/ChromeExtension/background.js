chrome.runtime.onInstalled.addListener(function() {
    chrome.declarativeContent.onPageChanged.removeRules(undefined, function() {
        chrome.declarativeContent.onPageChanged.addRules([{
          conditions: [
              new chrome.declarativeContent.PageStateMatcher({pageUrl: {urlContains: 'shapeways' },}),
              new chrome.declarativeContent.PageStateMatcher({pageUrl: {urlContains: 'thingiverse' },}),
              new chrome.declarativeContent.PageStateMatcher({pageUrl: {urlContains: 'myminifactory' },}),
              new chrome.declarativeContent.PageStateMatcher({pageUrl: {urlContains: 'gumroad' },}),
              new chrome.declarativeContent.PageStateMatcher({pageUrl: {urlContains: 'cults3d' },}),
          ],
              actions: [new chrome.declarativeContent.ShowPageAction()]
        }]);
      });
  });

chrome.tabs.onUpdated.addListener( function (tabId, changeInfo, tab) {
    if (changeInfo.status == 'complete' && tab.active) {
        if (
            (
                tab.url.includes("thingiverse.com/thing:")
                || tab.url.includes("shapeways.com/product/")
                || tab.url.includes("myminifactory.com/object/")
                || (tab.url.includes("gumroad.com/") && tab.url.includes("#"))
                || tab.url.includes("gumroad.com/l/")
                || (tab.url.includes("cults3d.com/") && tab.url.includes("3d-model"))
            )
        && !tab.url.includes("theminiindex"))
        {
            var xhr = new XMLHttpRequest();

            xhr.open("GET", "https://theminiindex.com/api/minis/check?URL="+tab.url, true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.onreadystatechange = function() {
                if (xhr.readyState == "4" && xhr.status == "404") {
                    //Not indexed, give the user a notification.
                    var opt = {
                        type: "basic",
                        title: "Add to The Mini Index?",
                        message: "This Mini isn't indexed yet!",
                        iconUrl: "tmi64.png",
                        buttons: [{title:"Index it!"}]
                        };
                    chrome.notifications.create(tab.url, opt);

                    chrome.notifications.onButtonClicked.addListener(function (url, buttonId){
                        chrome.tabs.create({ url: "https://theminiindex.com/Minis/Create?PlaceholderURL="+url });
                    });
                }
            }
            xhr.send();
        }
    }
});