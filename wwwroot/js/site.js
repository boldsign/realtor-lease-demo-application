document.addEventListener('DOMContentLoaded', () => {
    const yearElements = document.querySelectorAll('[data-current-year]');
    yearElements.forEach(el => {
        el.textContent = new Date().getFullYear().toString();
    });

    // Close mobile menu when clicking outside of it
    const mobileNavCollapse = document.getElementById('mainNavbar');
    const navToggler = document.querySelector('.nav-toggler-mobile');

    if (mobileNavCollapse && navToggler) {
        document.addEventListener('click', (e) => {
            const isOpen = mobileNavCollapse.classList.contains('show');
            if (!isOpen) return;

            const clickedInsideMenu = mobileNavCollapse.contains(e.target);
            const clickedToggler = navToggler.contains(e.target);

            if (!clickedInsideMenu && !clickedToggler) {
                const bsCollapse = bootstrap.Collapse.getInstance(mobileNavCollapse);
                if (bsCollapse) {
                    bsCollapse.hide();
                }
            }
        });
    }
});
