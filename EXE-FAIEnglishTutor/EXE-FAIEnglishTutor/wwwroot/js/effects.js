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

// ======================== PHÁO HOA CANVAS (TỰ TẠO) ========================
let fireworksParticles = [];
let fireworksAnimFrame;
let fireworksCanvas = null;
let fireworksCtx = null;

function initFireworksCanvas() {
    if (!fireworksCanvas) {
        fireworksCanvas = document.createElement('canvas');
        fireworksCanvas.id = 'fireworksCanvas';
        fireworksCanvas.style.display = 'none';
        fireworksCanvas.style.position = 'fixed';
        fireworksCanvas.style.top = '0';
        fireworksCanvas.style.left = '0';
        fireworksCanvas.style.width = '100%';
        fireworksCanvas.style.height = '100%';
        fireworksCanvas.style.zIndex = '9998';
        fireworksCanvas.style.pointerEvents = 'none';
        document.body.appendChild(fireworksCanvas);
        fireworksCtx = fireworksCanvas.getContext('2d');
    }
    fireworksCanvas.width = window.innerWidth;
    fireworksCanvas.height = window.innerHeight;
}

function randomColor() {
    const colors = ['#ff4757','#ffa502','#2ed573','#1e90ff','#eccc68','#ff6b81','#a29bfe','#fd79a8'];
    return colors[Math.floor(Math.random() * colors.length)];
}

function createBurst(x, y) {
    const count = 80;
    for (let i = 0; i < count; i++) {
        const angle = (Math.PI * 2 / count) * i;
        const speed = Math.random() * 6 + 2;
        fireworksParticles.push({
            x, y,
            vx: Math.cos(angle) * speed,
            vy: Math.sin(angle) * speed,
            alpha: 1,
            color: randomColor(),
            radius: Math.random() * 3 + 1,
            decay: Math.random() * 0.015 + 0.01
        });
    }
}

function animateFireworks() {
    fireworksCtx.clearRect(0, 0, fireworksCanvas.width, fireworksCanvas.height);
    fireworksParticles = fireworksParticles.filter(p => p.alpha > 0);
    fireworksParticles.forEach(p => {
        fireworksCtx.save();
        fireworksCtx.globalAlpha = p.alpha;
        fireworksCtx.fillStyle = p.color;
        fireworksCtx.beginPath();
        fireworksCtx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
        fireworksCtx.fill();
        fireworksCtx.restore();
        p.x += p.vx;
        p.y += p.vy;
        p.vy += 0.08; // trọng lực nhẹ
        p.alpha -= p.decay;
    });
    fireworksAnimFrame = requestAnimationFrame(animateFireworks);
}

function launchFireworks(duration) {
    initFireworksCanvas();
    fireworksCanvas.style.display = 'block';
    const interval = setInterval(() => {
        createBurst(Math.random() * fireworksCanvas.width, Math.random() * fireworksCanvas.height * 0.6);
    }, 300);
    animateFireworks();
    setTimeout(() => {
        clearInterval(interval);
        setTimeout(() => {
            fireworksCanvas.style.display = 'none';
            cancelAnimationFrame(fireworksAnimFrame);
        }, 2000);
    }, duration);
}
// ======================== HẾT PHÁO HOA CANVAS ========================
