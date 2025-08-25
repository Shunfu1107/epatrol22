
if (typeof App === 'undefined') App = {};


App.showErrorModal = function (msg) {
    $("#error-msg-modal").clone().modal("show").find("#error-msg-body").html(msg);
};

/// Sub Functions ///

var $myGroup = $('#accordion');
$myGroup.on('show.bs.collapse', '.collapse', function () {
    $myGroup.find('.collapse.show').collapse('hide');
});


function owlCarousel(target) {
    if (target.length > 0) {
        target.each(function () {
            var el = $(this),
                dataAuto = el.data('owl-auto'),
                dataLoop = el.data('owl-loop'),
                dataSpeed = el.data('owl-speed'),
                dataGap = el.data('owl-gap'),
                dataNav = el.data('owl-nav'),
                dataDots = el.data('owl-dots'),
                dataAnimateIn = (el.data('owl-animate-in')) ? el.data('owl-animate-in') : '',
                dataAnimateOut = (el.data('owl-animate-out')) ? el.data('owl-animate-out') : '',
                dataDefaultItem = el.data('owl-item'),
                dataItemXS = el.data('owl-item-xs'),
                dataItemSM = el.data('owl-item-sm'),
                dataItemMD = el.data('owl-item-md'),
                dataItemLG = el.data('owl-item-lg'),
                dataNavLeft = (el.data('owl-nav-left')) ? el.data('owl-nav-left') : "<i class='ps-icon-back'></i>",
                dataNavRight = (el.data('owl-nav-right')) ? el.data('owl-nav-right') : "<i class='ps-icon-next'></i>",
                duration = el.data('owl-duration'),
                datamouseDrag = (el.data('owl-mousedrag') === 'on') ? true : false;
            el.owlCarousel({
                animateIn: dataAnimateIn,
                animateOut: dataAnimateOut,
                margin: dataGap,
                autoplay: dataAuto,
                autoplayTimeout: dataSpeed,
                autoplayHoverPause: true,
                loop: dataLoop,
                nav: dataNav,
                mouseDrag: datamouseDrag,
                touchDrag: true,
                autoplaySpeed: duration,
                navSpeed: duration,
                dotsSpeed: duration,
                dragEndSpeed: duration,
                navText: [dataNavLeft, dataNavRight],
                dots: dataDots,
                items: dataDefaultItem,
                responsive: {
                    0: {
                        items: dataItemXS
                    },
                    480: {
                        items: dataItemSM
                    },
                    768: {
                        items: dataItemMD
                    },
                    992: {
                        items: dataItemLG
                    },
                    1200: {
                        items: dataDefaultItem
                    }
                }
            });
        });
    }
}

function navigateOwlCarousel() {
    var container = $('.ps-owl-root'),
        owl = $('.ps-owl--colection'),
        prev = container.find('.ps-owl-actions .ps-prev'),
        next = container.find('.ps-owl-actions .ps-next');
    if (container.length > 0) {
        prev.on('click', function (e) {
            e.preventDefault();
            owl.trigger('prev.owl.carousel', [300]);
        }),
        next.on('click', function (e) {
            e.preventDefault();
            owl.trigger('next.owl.carousel');
        });
    }
}

$('.alpha-numeric').keyboard({
    usePreview: false,
    layout: 'custom',
    restrictInput: true, // Prevent keys not in the displayed keyboard from being typed in
    preventPaste: true,  // prevent ctrl-v and right click
    autoAccept: true,
    maxLength: 30,
    useCombos: false,
    customLayout: {
        //'normal': [
        //    '1 2 3 4 5 6 7 8 9 0',
        //    'q w e r t y u i o p',
        //    'a s d f g h j k l {bksp}',
        //    'z x c v b n m - . /',
        //    '{cancel} {space} {accept}'
        //],
        'normal': [
            '1 2 3 4 5 6 7 8 9 0',
            'Q W E R T Y U I O P',
            'A S D F G H J K L {bksp}',
            'Z X C V B N M - . /',
            '{cancel} {space} {accept}'
        ]
    },
    css: {
        // input & preview
        input: 'form-control input-sm',
        // keyboard container
        container: 'center-block dropdown-menu', // jumbotron
        // default state
        buttonDefault: 'btn btn-default',
        // hovered button
        buttonHover: 'btn-primary',
        // Action keys (e.g. Accept, Cancel, Tab, etc);
        // this replaces "actionClass" option
        buttonAction: 'active',
        // used when disabling the decimal button {dec}
        // when a decimal exists in the input area
        buttonDisabled: 'disabled'
    }
});


$('.alpha').keyboard({
    usePreview: false,
    layout: 'custom',
    restrictInput: true, // Prevent keys not in the displayed keyboard from being typed in
    preventPaste: true,  // prevent ctrl-v and right click
    autoAccept: true,
    maxLength: 30,
    useCombos: false,
    customLayout: {
        //'normal': [
        //    'q w e r t y u i o p',
        //    'a s d f g h j k l {bksp}',
        //    'z x c v b n m - . {s}',
        //    '{cancel} {space} {accept}'
        //],
        'normal': [
            'Q W E R T Y U I O P',
            'A S D F G H J K L {bksp}',
            'Z X C V B N M - . /',
            '{cancel} {space} {accept}'
        ]
    },
    css: {
        // input & preview
        input: 'form-control input-sm',
        // keyboard container
        container: 'center-block dropdown-menu', // jumbotron
        // default state
        buttonDefault: 'btn btn-default',
        // hovered button
        buttonHover: 'btn-primary',
        // Action keys (e.g. Accept, Cancel, Tab, etc);
        // this replaces "actionClass" option
        buttonAction: 'active',
        // used when disabling the decimal button {dec}
        // when a decimal exists in the input area
        buttonDisabled: 'disabled'
    }
});

$('.number').keyboard({
    usePreview: false,
    layout: 'custom',
    restrictInput: true, // Prevent keys not in the displayed keyboard from being typed in
    preventPaste: true,  // prevent ctrl-v and right click
    autoAccept: true,
    maxLength: 11,
    useCombos: false,
    customLayout: {
        'normal': [
            '7 8 9',
            '4 5 6',
            '1 2 3',
            '{clear} 0 {bksp}',
            '{cancel} {accept}'
        ]
    },
    css: {
        // input & preview
        input: 'form-control input-sm',
        // keyboard container
        container: 'center-block dropdown-menu', // jumbotron
        // default state
        buttonDefault: 'btn btn-default',
        // hovered button
        buttonHover: 'btn-primary',
        // Action keys (e.g. Accept, Cancel, Tab, etc);
        // this replaces "actionClass" option
        buttonAction: 'active',
        // used when disabling the decimal button {dec}
        // when a decimal exists in the input area
        buttonDisabled: 'disabled'
    }
});

