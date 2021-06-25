function initialiseCarousel() {
    const promoCarousel = document.querySelector('#promo-carousel');

    const carousel = new bootstrap.Carousel(promoCarousel, {
        interval: 2000
    });

    const navElement = document.querySelector('#navbar-top');
    navElement.focus();
    navElement.blur();
}
