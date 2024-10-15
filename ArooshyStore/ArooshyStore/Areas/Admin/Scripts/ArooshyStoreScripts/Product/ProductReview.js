document.addEventListener("DOMContentLoaded", function () {
    const itemsPerPage = 8;
    const reviews = Array.from(document.querySelectorAll(".card"));
    const paginationControls = document.getElementById('pagination-controls');

    let currentPage = 1;
    const totalPages = Math.ceil(reviews.length / itemsPerPage);

    function showPage(page) {
        reviews.forEach((review, index) => {
            review.style.display = 'none';
        });
        const start = (page - 1) * itemsPerPage;
        const end = start + itemsPerPage;
        reviews.slice(start, end).forEach((review) => {
            review.style.display = 'block';
        });
        document.getElementById('prev-page').classList.toggle('disabled', page === 1);
        document.getElementById('next-page').classList.toggle('disabled', page === totalPages);
    }

    function createPagination() {
        for (let i = 1; i <= totalPages; i++) {
            const pageItem = document.createElement('li');
            pageItem.classList.add('page-item');
            if (i === currentPage) pageItem.classList.add('active');

            const pageLink = document.createElement('a');
            pageLink.classList.add('page-link');
            pageLink.href = '#';
            pageLink.innerText = i;

            pageLink.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = i;
                showPage(currentPage);
                updatePagination();
            });

            pageItem.appendChild(pageLink);
            paginationControls.insertBefore(pageItem, document.getElementById('next-page'));
        }
    }

    function updatePagination() {
        const pageItems = paginationControls.querySelectorAll('.page-item');
        pageItems.forEach((item, index) => {
            if (index > 0 && index <= totalPages) {
                item.classList.toggle('active', index === currentPage);
            }
        });
    }
    document.getElementById('prev-page').addEventListener('click', function (e) {
        e.preventDefault();
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
            updatePagination();
        }
    });
    document.getElementById('next-page').addEventListener('click', function (e) {
        e.preventDefault();
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
            updatePagination();
        }
    });
    showPage(currentPage);
    createPagination();
});