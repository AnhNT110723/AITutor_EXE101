// Gọi hàm này để bắn pháo hoa
function playFireworks(durationMs = 10000) {
    // Yêu cầu thư viện: <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.6.0/dist/confetti.browser.min.js"></script>
    if (typeof confetti !== 'function') {
        console.warn('Bạn cần chèn thư viện canvas-confetti trước khi gọi playFireworks()');
        return;
    }
    var end = Date.now() + durationMs;
    (function frame() {
        confetti({ particleCount: 5, angle: 60, spread: 55, origin: { x: 0 }, colors: ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff'] });
        confetti({ particleCount: 5, angle: 120, spread: 55, origin: { x: 1 }, colors: ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff'] });
        if (Date.now() < end) { requestAnimationFrame(frame); }
    }());
}

// Gọi hàm này để bắt đầu thả lồng đèn liên tục
function startLanternEffect(intervalMs = 250) {
    const emojis = ['🏮', '🌕', '🐰', '🥮'];
    setInterval(() => {
        const lantern = document.createElement('div');
        lantern.className = 'lantern';
        lantern.innerText = emojis[Math.floor(Math.random() * emojis.length)];
        lantern.style.left = Math.random() * 95 + 'vw';
        lantern.style.fontSize = (Math.random() * 20 + 20) + 'px';
        lantern.style.animationDuration = (Math.random() * 3 + 4) + 's'; 
        document.body.appendChild(lantern);
        setTimeout(() => { lantern.remove(); }, 7000);
    }, intervalMs);
}
