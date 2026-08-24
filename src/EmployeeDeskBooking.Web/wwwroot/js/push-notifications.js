(function () {
    const config = window.deskBookingPush;
    if (!config || !config.vapidPublicKey) {
        return;
    }

    const enableButton = document.getElementById(config.enableButtonId);
    const unsupported = document.getElementById(config.unsupportedId);
    if (!enableButton) {
        return;
    }

    if (!('serviceWorker' in navigator) || !('PushManager' in window) || !('Notification' in window)) {
        if (unsupported) {
            unsupported.hidden = false;
        }
        enableButton.disabled = true;
        return;
    }

    enableButton.addEventListener('click', async function () {
        try {
            const permission = await Notification.requestPermission();
            if (permission !== 'granted') {
                if (unsupported) {
                    unsupported.hidden = false;
                }
                return;
            }

            const registration = await navigator.serviceWorker.register('/sw.js');
            const subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: urlBase64ToUint8Array(config.vapidPublicKey)
            });

            document.getElementById(config.subscriptionInputId).value = JSON.stringify(subscription);
            document.getElementById(config.enableFormId).submit();
        } catch (error) {
            if (unsupported) {
                unsupported.hidden = false;
            }
            console.error('Push enable failed', error);
        }
    });

    function urlBase64ToUint8Array(base64String) {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/');
        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);
        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }
})();
