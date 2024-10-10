document.addEventListener("DOMContentLoaded", function () {
    const productsPerPage = 9;
    const products = [...document.querySelectorAll('.product-grid .col')];
    const totalPages = Math.ceil(products.length / productsPerPage);
    let currentPage = 1;

    function displayProducts(page) {
        const start = (page - 1) * productsPerPage;
        const end = start + productsPerPage;
        products.forEach((product, index) => {
            product.style.display = (index >= start && index < end) ? "block" : "none";
        });
        updatePaginationButtons(page, totalPages);
    }

    function updatePaginationButtons(page, pagesCount) {
        const pageNumbersContainer = document.getElementById("page-numbers");
        pageNumbersContainer.innerHTML = '';

        for (let i = 1; i <= pagesCount; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === page ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="javascript:;">${i}</a>`;
            li.addEventListener('click', () => displayProducts(i));
            pageNumbersContainer.appendChild(li);
        }
        document.getElementById("prev-page").classList.toggle('disabled', page === 1);
        document.getElementById("next-page").classList.toggle('disabled', page === pagesCount);
    }

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

    function filterProducts(filteredProducts) {
        products.forEach(product => product.style.display = "none");
        const filteredTotal = filteredProducts.length;
        filteredProducts.forEach((product, index) => {
            product.style.display = "block";
        });
        const filteredPages = Math.ceil(filteredTotal / productsPerPage);
        updatePaginationButtons(1, filteredPages);

        currentPage = 1;
    }

    displayProducts(currentPage);
});