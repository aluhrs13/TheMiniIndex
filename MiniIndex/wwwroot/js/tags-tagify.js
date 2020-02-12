import Tagify from '@yaireo/tagify'
import '@yaireo/tagify/dist/tagify.css'

var controller;

var input = document.querySelector('textarea#tagsInput');

var tagify = new Tagify(input, {
    enforceWhitelist: true,
    dropdown: {
        enabled: 1,
        position: "all"
    },
    whitelist: []
});

tagify.on('input', onInput);
tagify.on('add', onTagAdded);

var parentForm = input.closest('form');
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

function onTagAdded(e) {
    var tagValue = e.detail.data.value;
    e.data = tagValue;
}

function onFormSubmitted(e) {
}