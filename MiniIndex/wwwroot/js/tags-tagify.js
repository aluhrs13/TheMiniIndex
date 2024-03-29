﻿import Tagify from "@yaireo/tagify";
import "@yaireo/tagify/dist/tagify.css";

var controller;

var tagsInput = document.querySelector("input#tagsInput");
var tagsValue = document.querySelector("input#tagsValue");

tagsInput.value = tagsValue.value;
var currentTags = tagsValue.value.split(";");
var whitelistTags = currentTags;
whitelistTags.push(
    "Fantasy",
    "Scifi",
    "Historical",
    "Mini",
    "Scatter",
    "Tile",
    "Accessory"
);

var tagify = new Tagify(tagsInput, {
    delimiters: ";",
    enforceWhitelist: true,
    dropdown: {
        enabled: 1,
        position: "all",
    },
    whitelist: whitelistTags,
});

tagify.on("input", onInput);

var parentForm = document.getElementById("searchForm");

parentForm.onsubmit = onFormSubmitted;

//TODO: Don't modify settings
function onInput(e) {
    console.log(tagsInput.value);
    var value = e.detail.value;
    tagify.settings.whitelist.length = 0;

    controller && controller.abort();
    controller = new AbortController();

    tagify.loading(true).dropdown.hide.call(tagify);

    fetch("/api/tags?search=" + value, { signal: controller.signal })
        .then((response) => response.json())
        .then(function (whitelist) {
            tagify.settings.whitelist.splice(0, whitelist.length, ...whitelist);
            tagify.loading(false).dropdown.show.call(tagify, value);
        });
}

function onFormSubmitted(e) {
    if (tagsInput.value != "") {
        tagsValue.value = JSON.parse(tagsInput.value)
            .map((x) => x.value)
            .join(";");
    } else {
        tagsValue.value = "";
    }
}
/*
$(".add-tagify").click(function () {
    tagify.addTags($(this).text());
});
*/
