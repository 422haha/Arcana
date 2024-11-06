var programs_being_loaded = 0;

function enhance_iframe(iframe) {
    var $iframe = $(iframe);

    $("body").addClass("loading-program");
    programs_being_loaded += 1;

    $iframe.on("load", function () {
        if (--programs_being_loaded <= 0) {
            $("body").removeClass("loading-program");
        }

        try {
            // iframe 접근 가능성 확인
            if (!iframe.contentWindow || !iframe.contentDocument) {
                throw new Error("Cannot access iframe content");
            }
        } catch (e) {
            console.warn(`[enhance_iframe] iframe integration is not available for '${iframe.src}'`);
            return;
        }

        // 테마 적용 부분 수정
        if (window.themeCSSProperties) {
            try {
                applyTheme(themeCSSProperties, iframe.contentDocument.documentElement);
            } catch (e) {
                console.warn("Failed to apply theme to iframe:", e);
            }
        }

        // Let the iframe to handle mouseup events outside itself
        iframe.contentDocument.addEventListener("mousedown", (event) => {
            var delegate_pointerup = function() {
                if (iframe.contentWindow && iframe.contentWindow.jQuery) {
                    iframe.contentWindow.jQuery("body").trigger("pointerup");
                }
                if (iframe.contentWindow) {
                    const event = new iframe.contentWindow.MouseEvent("mouseup", { button: 0 });
                    iframe.contentWindow.dispatchEvent(event);
                    const event2 = new iframe.contentWindow.MouseEvent("mouseup", { button: 2 });
                    iframe.contentWindow.dispatchEvent(event2);
                }
                clean_up_delegation();
            };

            $G.on("mouseup blur", delegate_pointerup);
            iframe.contentDocument.addEventListener("mouseup", clean_up_delegation);
            function clean_up_delegation() {
                $G.off("mouseup blur", delegate_pointerup);
                iframe.contentDocument?.removeEventListener("mouseup", clean_up_delegation);
            }
        });

        // Let the containing page handle keyboard events
        proxy_keyboard_events(iframe);

        var $contentWindow = $(iframe.contentWindow);
        $contentWindow.on("pointerdown click", function(e) {
            iframe.$window && iframe.$window.focus();
            $(".menu-button").trigger("release");
            $(".menu-popup").hide();
        });

        $contentWindow.on("pointerdown", function(e) {
            $iframe.css("pointer-events", "all");
            $("body").addClass("drag");
        });

        $contentWindow.on("pointerup", function(e) {
            $("body").removeClass("drag");
            $iframe.css("pointer-events", "");
        });

        iframe.contentWindow.close = function() {
            iframe.$window && iframe.$window.close();
        };

        // 메시지 박스 처리 부분 수정
        iframe.contentWindow.showMessageBox = (options) => {
            return new Promise((resolve, reject) => {
                try {
                    showMessageBox({
                        title: options.title || iframe.contentWindow.defaultMessageBoxTitle || "Message",
                        message: options.message,
                        buttons: options.buttons,
                        iconID: options.iconID,
                    }).then(resolve).catch(reject);
                } catch (e) {
                    console.warn("Message box error:", e);
                    resolve();
                }
            });
        };
    });

    $iframe.css({
        minWidth: 0,
        minHeight: 0,
        flex: 1,
        border: 0,
    });
}

function proxy_keyboard_events(iframe) {
    for (const event_type of ["keyup", "keydown", "keypress"]) {
        iframe.contentWindow.addEventListener(event_type, (event) => {
            const proxied_event = new KeyboardEvent(event_type, {
                target: iframe,
                view: iframe.ownerDocument.defaultView,
                bubbles: true,
                cancelable: true,
                key: event.key,
                keyCode: event.keyCode,
                which: event.which,
                code: event.code,
                shiftKey: event.shiftKey,
                ctrlKey: event.ctrlKey,
                metaKey: event.metaKey,
                altKey: event.altKey,
                repeat: event.repeat,
            });
            const result = iframe.dispatchEvent(proxied_event);
            if (!result) {
                event.preventDefault();
            }
        }, true);
    }
}

function make_iframe_window(options) {
    options.resizable ??= true;
    var $win = new $Window(options);

    var $iframe = $win.$iframe = $("<iframe>").attr({
        // options.src가 있으면 바로 사용, 없으면 window.html 사용
        src: options.src || "/98/window.html",
        frameborder: "0",
        allowtransparency: "true",
        allow: "pointer-lock" // 마우스 관련 권한 추가
    });

    enhance_iframe($iframe[0]);
    $win.$content.append($iframe);
    var iframe = $win.iframe = $iframe[0];
    iframe.$window = $win;

    $iframe.on("load", function() {
        try {
            if (iframe.contentDocument) {
                $win.show();
                $win.focus();
            }
        } catch (e) {
            console.warn("iframe load error:", e);
        }
    });

    $win.$content.css({
        display: "flex",
        flexDirection: "column",
        background: "var(--ButtonFace)"
    });

    $win.center();
    $win.hide();

    return $win;
}

// Fix dragging things over iframes
$(window).on("pointerdown", function(e) {
    $("body").addClass("drag");
});

$(window).on("pointerup dragend blur", function(e) {
    if (e.type === "blur") {
        if (document.activeElement.tagName.match(/iframe/i)) {
            return;
        }
    }
    $("body").removeClass("drag");
    $("iframe").css("pointer-events", "");
});