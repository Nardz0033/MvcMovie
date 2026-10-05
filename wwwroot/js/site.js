(() => {
    const toggle = document.querySelector(".nav-toggle");
    const menu = document.querySelector(".nav-menu");

    if (toggle && menu) {
        toggle.addEventListener("click", () => {
            const isOpen = toggle.getAttribute("aria-expanded") === "true";
            toggle.setAttribute("aria-expanded", String(!isOpen));
            toggle.setAttribute("aria-label", isOpen ? "Open navigation" : "Close navigation");
            menu.classList.toggle("is-open", !isOpen);
        });

        menu.addEventListener("click", event => {
            if (event.target instanceof Element && event.target.closest("a")) {
                toggle.setAttribute("aria-expanded", "false");
                toggle.setAttribute("aria-label", "Open navigation");
                menu.classList.remove("is-open");
            }
        });
    }

    const search = document.querySelector("#catalog-search");
    document.addEventListener("keydown", event => {
        if (event.key !== "/" || event.ctrlKey || event.metaKey || event.altKey ||
            event.target instanceof HTMLInputElement || event.target instanceof HTMLTextAreaElement) {
            return;
        }

        if (search instanceof HTMLInputElement) {
            event.preventDefault();
            search.focus();
        }
    });

    const fileInput = document.querySelector("#PosterUpload");
    const preview = document.querySelector("#poster-preview");
    const fileName = document.querySelector("#poster-file-name");
    let previewUrl;

    if (fileInput instanceof HTMLInputElement && preview instanceof HTMLImageElement) {
        fileInput.addEventListener("change", () => {
            const file = fileInput.files?.[0];
            if (!file) {
                return;
            }

            if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)) {
                fileInput.value = "";
                if (fileName) {
                    fileName.textContent = "Choose a JPG, PNG, or WEBP image";
                }
                return;
            }

            if (previewUrl) {
                URL.revokeObjectURL(previewUrl);
            }
            previewUrl = URL.createObjectURL(file);
            preview.src = previewUrl;
            preview.classList.remove("is-placeholder");
            if (fileName) {
                fileName.textContent = file.name;
            }
        });
    }
})();
