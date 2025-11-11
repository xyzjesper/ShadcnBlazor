window.testy = {

    getBoundingBox: function (element) {
        console.log(element);
        return element.getBoundingClientRect();
    },

    requestAnimationFrameAsync: async function () {
        return new Promise(resolve =>
        {
            requestAnimationFrame(() => {
                resolve();
            })
        })
    },
    
    getViewport: function () {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        }
    },
    
    positionDropdown: function (dropdown, clientX, clientY) {
        dropdown.style.display = "block"; // make visible first
        dropdown.style.left = "0px";
        dropdown.style.top = "0px";
        dropdown.style.position = "fixed";

        // Wait for next frame to get correct size
        requestAnimationFrame(() => {
            const rect = dropdown.getBoundingClientRect();

            const cursorX = clientX;
            const cursorY = clientY;

            const offsetX = 10; // distance from cursor
            const offsetY = 10;
            const margin = 8;   // min distance from viewport edges
            const tolerance = 2; // accounts for subpixel differences

            let x = cursorX + offsetX;
            let y = cursorY + offsetY;

            // Viewport size
            const vw = window.innerWidth;
            const vh = window.innerHeight;

            // If dropdown overflowed to the right, move left
            if (x + rect.width + margin > vw - tolerance) {
                x = Math.max(margin, cursorX - rect.width - offsetX);
            }

            // If dropdown overflowed to the bottom, move up
            if (y + rect.height + margin > vh - tolerance) {
                y = Math.max(margin, cursorY - rect.height - offsetY);
            }

            // Prevent going off-screen entirely
            x = Math.min(Math.max(x, margin), vw - rect.width - margin);
            y = Math.min(Math.max(y, margin), vh - rect.height - margin);

            dropdown.style.transform = `translate(${x}px, ${y}px)`;
        });
    },
    positionSideElementRelativeToChild: function (triggerElement, submenuElement) {
        submenuElement.style.display = "block";
        submenuElement.style.left = "0px";
        submenuElement.style.top = "0px";
        submenuElement.style.position = "fixed";

        requestAnimationFrame(() => {
            const submenuRect = submenuElement.getBoundingClientRect();
            const triggerRect = triggerElement.getBoundingClientRect();

            const margin = 8;  // distance from viewport edges
            const offsetX = 4; // small gap between parent and submenu
            const tolerance = 2;

            const vw = window.innerWidth;
            const vh = window.innerHeight;

            // Default: submenu opens to the right of the trigger
            let x = triggerRect.right + offsetX;
            let y = triggerRect.top;

            // Adjust if submenu overflows to the right — open to the left instead
            if (x + submenuRect.width + margin > vw - tolerance) {
                x = triggerRect.left - submenuRect.width - offsetX;
            }

            // Adjust if submenu overflows to the bottom
            if (y + submenuRect.height + margin > vh - tolerance) {
                y = Math.max(margin, vh - submenuRect.height - margin);
            }

            // Clamp within viewport
            x = Math.min(Math.max(x, margin), vw - submenuRect.width - margin);
            y = Math.min(Math.max(y, margin), vh - submenuRect.height - margin);

            submenuElement.style.transform = `translate(${x}px, ${y}px)`;
        });
    }
}