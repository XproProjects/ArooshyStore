document.addEventListener("DOMContentLoaded", function () {
    const productsPerPage = 9;  // Number of products per page
    const productCards = [...document.querySelectorAll('.product-grid .card')];
    const totalPages = Math.ceil(productCards.length / productsPerPage);
    let currentPage = 1;

    function displayProducts(page) {
        const start = (page - 1) * productsPerPage;
        const end = start + productsPerPage;

        productCards.forEach((product, index) => {
            if (index >= start && index < end) {
                product.style.display = "block";
            } else {
                product.style.display = "none";
            }
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
    displayProducts(currentPage);
});