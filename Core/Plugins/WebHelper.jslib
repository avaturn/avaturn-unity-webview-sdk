mergeInto(LibraryManager.library, {

    ShowAvaturnIframeJS: function () {
        window.displayIframe();
    },

    HideAvaturnIFrameJS: function () {
        window.hideIframe();
    },
        
    SetupAvaturnIframeJS: function (link, version, platform){
        window.setupIframe(UTF8ToString(link), UTF8ToString(version), UTF8ToString(platform));
    },
});