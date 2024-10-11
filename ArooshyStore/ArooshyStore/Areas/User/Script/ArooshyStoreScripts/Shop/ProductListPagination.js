document.addEventListener("DOMContentLoaded", function () {
    let productsPerPage = 9;  // Default number of products per page
    const productCards = [...document.querySelectorAll('.product-grid .card')];
    let totalPages = Math.ceil(productCards.length / productsPerPage);
    let currentPage = 1;

    function displayProducts(page) {
        const start = (page - 1) * productsPerPage;
        const end = start + productsPerPage;

        productCards.forEach((product, index) => {
            product.style.display = (index >= start && index < end) ? "block" : "none";
        });
        updatePaginationButtons(page);
    }

    function updatePaginationButtons(page) {
        const pageNumbersContainer = document.getElementById("page-numbers");
        pageNumbersContainer.innerHTML = '';

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === page ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="javascript:;">${i}</a>`;
            li.addEventListener('click', () => {
                currentPage = i;
                displayProducts(i);
            });
            pageNumbersContainer.appendChild(li);
        }
        document.getElementById("prev-page").classList.toggle('disabled', page === 1);
        document.getElementById("next-page").classList.toggle('disabled', page === totalPages);
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
    document.getElementById("productsPerPageSelect").addEventListener('change', (event) => {
        productsPerPage = parseInt(event.target.value, 10);
        totalPages = Math.ceil(productCards.length / productsPerPage);
        currentPage = 1; 
        displayProducts(currentPage); 
    });

    displayProducts(currentPage); 
});
