import Tagify from '@yaireo/tagify'
import '@yaireo/tagify/dist/tagify.css'

var input = document.querySelector('textarea[name=Tags]');

var tagify = new Tagify(input, {
    enforceWhitelist: true,
    dropdown: {
        enabled: 1,
        position: "all"
    },
    whitelist: []
})

var controller;

tagify.on('input', onInput)

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