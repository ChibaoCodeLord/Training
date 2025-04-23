document.addEventListener('DOMContentLoaded', function () {
    let currentSlide = 0;
    const slides = document.querySelectorAll('.slide');
    const progressBar = document.querySelector('.slide-progress-bar');
    let slideInterval;
    const slideDuration = 5000; // 5 giây mỗi slide

    function showSlide(n) {
        slides.forEach(slide => slide.classList.remove('active'));
        currentSlide = (n + slides.length) % slides.length;
        slides[currentSlide].classList.add('active');
        resetProgressBar();
    }

    function nextSlide() {
        showSlide(currentSlide + 1);
    }

    function resetProgressBar() {
        progressBar.style.width = '0%';
        clearInterval(progressInterval);
        startProgressBar();
    }

    function startProgressBar() {
        let width = 0;
        const increment = 100 / (slideDuration / 50);

        progressInterval = setInterval(() => {
            if (width >= 100) {
                clearInterval(progressInterval);
                nextSlide();
            } else {
                width += increment;
                progressBar.style.width = width + '%';
            }
        }, 50);
    }

    function startSlideshow() {
        slideInterval = setInterval(nextSlide, slideDuration);
        startProgressBar();
    }

    // Khởi tạo slideshow
    showSlide(0);
    startSlideshow();

    // Dừng khi hover
    const container = document.querySelector('.slideshow-container');
    container.addEventListener('mouseenter', () => {
        clearInterval(slideInterval);
        clearInterval(progressInterval);
    });

    container.addEventListener('mouseleave', () => {
        startSlideshow();
    });
});