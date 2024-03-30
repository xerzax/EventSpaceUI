// interactiveText.js
window.addEventListener('scroll', function () {
    var element = document.querySelector('.interactive-text');
    var position = element.getBoundingClientRect();
    if (position.top >= 0 && position.bottom <= window.innerHeight) {
        element.classList.add('larger', 'scrolled');
    } else {
        element.classList.remove('larger', 'scrolled');
    }
});
