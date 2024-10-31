document.addEventListener("DOMContentLoaded", function () {
    const productsPerPage = 9; // Set the number of products per page
    const gridProducts = [...document.querySelectorAll('.product-grid .col')];
    const listProducts = [...document.querySelectorAll('.product-list .card')];

    const totalGridPages = Math.ceil(gridProducts.length / productsPerPage);
    const totalListPages = Math.ceil(listProducts.length / productsPerPage);

    let currentGridPage = 1;
    let currentListPage = 1;

    const displayProducts = (page, products, productsPerPage, type) => {
        const start = (page - 1) * productsPerPage;
        const end = start + productsPerPage;

        products.forEach((product, index) => {
            product.style.display = (index >= start && index < end) ? "block" : "none";
        });

        const totalPages = type === "grid" ? totalGridPages : totalListPages;
        updatePaginationButtons(page, totalPages, type);
    };

    const updatePaginationButtons = (page, pagesCount, type) => {
        const pageNumbersContainer = type === "grid" ? document.getElementById("page-numbers") : document.getElementById("page-numbers-list");

        pageNumbersContainer.innerHTML = '';

        for (let i = 1; i <= pagesCount; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === page ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="javascript:;">${i}</a>`;
            li.addEventListener('click', () => {
                if (type === "grid") {
                    currentGridPage = i;
                    displayProducts(currentGridPage, gridProducts, productsPerPage, "grid");
                } else {
                    currentListPage = i;
                    displayProducts(currentListPage, listProducts, productsPerPage, "list");
                }
            });
            pageNumbersContainer.appendChild(li);
        }

        const prevPageButton = type === "grid" ? document.getElementById("prev-page") : document.getElementById("prev-page-list");
        const nextPageButton = type === "grid" ? document.getElementById("next-page") : document.getElementById("next-page-list");

        prevPageButton.classList.toggle('disabled', page === 1);
        nextPageButton.classList.toggle('disabled', page === pagesCount);
    };

    // Event listeners for grid view pagination
    document.getElementById("prev-page").addEventListener('click', () => {
        if (currentGridPage > 1) {
            currentGridPage--;
            displayProducts(currentGridPage, gridProducts, productsPerPage, "grid");
        }
    });

    document.getElementById("next-page").addEventListener('click', () => {
        if (currentGridPage < totalGridPages) {
            currentGridPage++;
            displayProducts(currentGridPage, gridProducts, productsPerPage, "grid");
        }
    });

    // Event listeners for list view pagination
    document.getElementById("prev-page-list").addEventListener('click', () => {
        if (currentListPage > 1) {
            currentListPage--;
            displayProducts(currentListPage, listProducts, productsPerPage, "list");
        }
    });

    document.getElementById("next-page-list").addEventListener('click', () => {
        if (currentListPage < totalListPages) {
            currentListPage++;
            displayProducts(currentListPage, listProducts, productsPerPage, "list");
        }
    });
    displayProducts(currentGridPage, gridProducts, productsPerPage, "grid");
    displayProducts(currentListPage, listProducts, productsPerPage, "list");
});
