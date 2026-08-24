self.addEventListener('push', function (event) {
    if (!event.data) {
        return;
    }

    var payload = event.data.json();
    var title = payload.title || 'Desk Booking';
    var options = {
        body: payload.body || '',
        icon: '/images/desk-booking-logo.png'
    };

    event.waitUntil(self.registration.showNotification(title, options));
});
