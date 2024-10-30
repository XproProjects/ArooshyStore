document.addEventListener("DOMContentLoaded", function () {
    let productsPerPage = 9; // Default value
    const products = [...document.querySelectorAll('.product-grid .col')];
    let totalPages = Math.ceil(products.length / productsPerPage);
    let currentPage = 1;

    const displayProducts = (page) => {
        const start = (page - 1) * productsPerPage;
        const end = start + productsPerPage;

        products.forEach((product, index) => {
            product.style.display = (index >= start && index < end) ? "block" : "none";
        });

        updatePaginationButtons(page, totalPages);
    };

    const updatePaginationButtons = (page, pagesCount) => {
        const pageNumbersContainer = document.getElementById("page-numbers");
        pageNumbersContainer.innerHTML = '';

        for (let i = 1; i <= pagesCount; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === page ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="javascript:;">${i}</a>`;
            li.addEventListener('click', () => {
                currentPage = i; // Update currentPage
                displayProducts(currentPage);
            });
            pageNumbersContainer.appendChild(li);
        }

        document.getElementById("prev-page").classList.toggle('disabled', page === 1);
        document.getElementById("next-page").classList.toggle('disabled', page === pagesCount);
    };

    document.getElementById("prev-page").addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            displayProducts(currentPage);
        }
    });

    document.getElementById("next-page").addEventListener('click', () => {
        if (currentPage < totalPages) {
            currentPage++;
            displayProducts(currentPage);
        }
    });

    // Optional: If you want to implement products per page selection
    const productsPerPageSelect = document.getElementById("productsPerPageSelect");
    if (productsPerPageSelect) {
        productsPerPageSelect.addEventListener('change', (event) => {
            productsPerPage = parseInt(event.target.value, 10);
            totalPages = Math.ceil(products.length / productsPerPage);
            currentPage = 1; // Reset to first page
            displayProducts(currentPage);
        });
    }

    // Initial display call
    displayProducts(currentPage);
});
