function getToken() {
    const tokenCookie = document.cookie.split(';').find(cookie => cookie.startsWith('token='));
    if (!tokenCookie) {
        return null;
    }
    return tokenCookie.split('=')[1];
}