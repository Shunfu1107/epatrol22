$(function () {
    $.validator.methods.date = function (value, element) {
        var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
        if (isChrome) {
            var d = new Date();
            var locale = window.navigator.userLanguage || window.navigator.language;
            return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(locale)));
            //return this.optional(element) || moment(value, "L", true).isValid();
        } else {
            return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
        }

    };
});