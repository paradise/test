
window.fbAsyncInit = function () {
    FB.init({
        appId: '184148225054854', // App ID
        channelUrl: 'http://localhost:3532', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });

    // Additional initialization code here
};

// Load the SDK Asynchronously
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));

$(function () {
    $('img[data-social]').click(function () {
        var item = $(this);
        var social = item.attr('data-social');
        switch (social) {
            case 'facebook': {
                FB.getLoginStatus(function (response) {
                    console.log(response);
                    if (response.status === 'connected') {
                        var uid = response.authResponse.userID;
                        var accessToken = response.authResponse.accessToken;
                    } else {
                        FB.login(function (response) {
                            console.log(response);
                            if (response.authResponse) {
                                console.log('Welcome!  Fetching your information.... ');
                                FB.api('/me', function (response) {
                                    console.log('Good to see you, ' + response.name + '.');
                                });
                            } else {
                                console.log('User cancelled login or did not fully authorize.');
                            }
                        });
                    }
                });

            }
        }
        TC.showLogin();
    });
});