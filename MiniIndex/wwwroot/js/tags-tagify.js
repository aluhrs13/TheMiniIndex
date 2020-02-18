import Tagify from '@yaireo/tagify'
import '@yaireo/tagify/dist/tagify.css'

var controller;

var tagsInput = document.querySelector('textarea#tagsInput');
var tagsValue = document.querySelector('input#tagsValue');

tagsInput.value = tagsValue.value;
var currentTags = tagsValue.value.split(',');

var tagify = new Tagify(tagsInput, {
    enforceWhitelist: true,
    dropdown: {
        enabled: 1,
        position: "all"
    },
    whitelist: currentTags
});

tagify.on('input', onInput);

var parentForm = tagsInput.closest('form');
console.log(parentForm);

parentForm.onsubmit = onFormSubmitted;

function onInput(e) {
    var value = e.detail.value;
    tagify.settings.whitelist.length = 0;

    controller && controller.abort();
    controller = new AbortController();

    tagify.loading(true).dropdown.hide.call(tagify)

    fetch('/api/tags?search=' + value, { signal: controller.signal })
        .then(response => response.json())
        .then(function (whitelist) {
            tagify.settings.whitelist.splice(0, whitelist.length, ...whitelist)
            tagify.loading(false).dropdown.show.call(tagify, value);
        })
}

function onFormSubmitted(e) {
    tagsValue.value = JSON.parse(tagsInput.value).map(x => x.value).join();
}