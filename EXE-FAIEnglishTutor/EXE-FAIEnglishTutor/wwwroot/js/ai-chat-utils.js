/**
 * ai-chat-utils.js
 * Hàm dùng chung cho tất cả các tính năng chat AI trong hệ thống.
 * Dùng được ở: Home chatbot, Speaking AI Situation, Free Talk, v.v.
 */

/**
 * Hiển thị hiệu ứng 3 chấm "AI đang nhập..." vào container chỉ định.
 * @param {string} containerId - ID của element chứa tin nhắn (vd: "chat-messages")
 * @param {string} [typingId="ai-typing-indicator"] - ID duy nhất cho typing bubble này
 * @returns {string} ID của typing indicator vừa tạo
 */
function showAITyping(containerId, typingId = 'ai-typing-indicator', options = {}) {
    // Xóa indicator cũ nếu còn tồn tại
    removeAITyping(typingId);

    const bubbleClass = options.bubbleClass || 'direct-chat-msg ai-typing-bubble';
    const textClass = options.textClass || 'direct-chat-text typing-dots-wrapper';

    const bubble = `
        <div class="${bubbleClass}" id="${typingId}">
            <div class="${textClass}">
                <span class="typing-dot"></span>
                <span class="typing-dot"></span>
                <span class="typing-dot"></span>
            </div>
        </div>`;

    const container = document.getElementById(containerId);
    if (container) {
        container.insertAdjacentHTML('beforeend', bubble);
        // Auto scroll xuống để thấy indicator - delay 1 chút để DOM kịp render
        setTimeout(() => {
            // Dùng scrollIntoView để đảm bảo phần tử luôn hiển thị, độc lập với class của container
            const indicator = document.getElementById(typingId);
            if (indicator) {
                indicator.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
            
            // Fallback
            const scrollContainer = container.closest('.popup-messages, .chat-scroll-area, .chat-container, [data-chat-scroll]')
                || container;
            if (scrollContainer) {
                scrollContainer.scrollTop = scrollContainer.scrollHeight;
            }
        }, 10);
    }
    return typingId;
}

/**
 * Xóa hiệu ứng typing indicator.
 * @param {string} [typingId="ai-typing-indicator"] - ID của indicator cần xóa
 */
function removeAITyping(typingId = 'ai-typing-indicator') {
    const el = document.getElementById(typingId);
    if (el) el.remove();
}

/**
 * Format thời gian hiện tại dạng HH:MM AM/PM.
 * @returns {string}
 */
function getFormattedTime() {
    const now = new Date();
    let hours = now.getHours();
    const minutes = now.getMinutes().toString().padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;
    return `${hours}:${minutes} ${ampm}`;
}

/**
 * CSS styles cho typing indicator — inject vào <head> một lần duy nhất.
 * Gọi hàm này trong DOMContentLoaded nếu bạn muốn dùng mà không cần file CSS riêng.
 */
function injectAIChatStyles() {
    if (document.getElementById('ai-chat-utils-style')) return;
    const style = document.createElement('style');
    style.id = 'ai-chat-utils-style';
    style.textContent = `
        /* ===== Typing Indicator ===== */
        .typing-dots-wrapper {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 10px 16px !important;
            min-width: 56px;
        }

        .typing-dot {
            width: 8px;
            height: 8px;
            background-color: #aaa;
            border-radius: 50%;
            display: inline-block;
            animation: typing-bounce 1.2s infinite ease-in-out;
        }

        .typing-dot:nth-child(1) { animation-delay: 0s; }
        .typing-dot:nth-child(2) { animation-delay: 0.2s; }
        .typing-dot:nth-child(3) { animation-delay: 0.4s; }

        @keyframes typing-bounce {
            0%, 60%, 100% { transform: translateY(0); background-color: #bbb; }
            30%            { transform: translateY(-6px); background-color: #ff6701; }
        }

        .ai-typing-bubble .direct-chat-text {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.12);
            display: inline-block;
        }
    `;
    document.head.appendChild(style);
}

// Tự động inject styles khi file được load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', injectAIChatStyles);
} else {
    injectAIChatStyles();
}
