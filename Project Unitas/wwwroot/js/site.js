// Unitas - site-wide scripts (no dependencies).

// ---------------------------------------------------------------------------
// Main navigation dropdowns: click/tap to open, Escape or click outside to close.
// Mouse users also get hover-to-open via CSS.
// ---------------------------------------------------------------------------
(() => {
    const items = document.querySelectorAll("[data-dropdown]");
    if (!items.length) return;

    const setOpen = (item, open) => {
        item.classList.toggle("is-open", open);
        item.querySelector("button").setAttribute("aria-expanded", String(open));
    };

    const closeAll = (except) => items.forEach((item) => item !== except && setOpen(item, false));

    items.forEach((item) => {
        const toggle = item.querySelector("button");
        toggle.addEventListener("click", () => {
            const open = !item.classList.contains("is-open");
            closeAll(item);
            setOpen(item, open);
        });
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest("[data-dropdown]")) closeAll();
    });

    document.addEventListener("keydown", (event) => {
        if (event.key !== "Escape") return;
        const open = document.querySelector("[data-dropdown].is-open");
        if (open) {
            setOpen(open, false);
            open.querySelector("button").focus();
        }
    });
})();

// ---------------------------------------------------------------------------
// Carousel: arrow buttons scroll one tile, the progress bar follows the scroll.
// ---------------------------------------------------------------------------
document.querySelectorAll("[data-carousel]").forEach((carousel) => {
    const track = carousel.querySelector("[data-carousel-track]");
    const prev = carousel.querySelector("[data-carousel-prev]");
    const next = carousel.querySelector("[data-carousel-next]");
    const thumb = carousel.querySelector("[data-carousel-thumb]");
    if (!track) return;

    const step = () => {
        const slide = track.firstElementChild;
        const gap = parseFloat(getComputedStyle(track).columnGap) || 0;
        return slide ? slide.getBoundingClientRect().width + gap : track.clientWidth;
    };

    const update = () => {
        if (!track.scrollWidth) return;
        const max = track.scrollWidth - track.clientWidth;
        const visible = track.clientWidth / track.scrollWidth;

        if (thumb) {
            thumb.style.width = `${Math.min(visible, 1) * 100}%`;
            const progress = max > 0 ? track.scrollLeft / max : 0;
            // translateX is relative to the thumb's own width
            thumb.style.transform = `translateX(${progress * (1 / visible - 1) * 100}%)`;
        }

        if (prev) prev.disabled = track.scrollLeft <= 1;
        if (next) next.disabled = track.scrollLeft >= max - 1;
    };

    prev?.addEventListener("click", () => track.scrollBy({ left: -step() }));
    next?.addEventListener("click", () => track.scrollBy({ left: step() }));

    track.addEventListener("keydown", (event) => {
        if (event.key !== "ArrowLeft" && event.key !== "ArrowRight") return;
        event.preventDefault();
        track.scrollBy({ left: event.key === "ArrowLeft" ? -step() : step() });
    });

    track.addEventListener("scroll", update, { passive: true });
    window.addEventListener("resize", update);
    update();
});

// ---------------------------------------------------------------------------
// Inputs with data-autosubmit save their form as soon as the value changes
// (cart quantities). The "Bijwerken" button stays as a fallback without JavaScript.
// ---------------------------------------------------------------------------
document.querySelectorAll("[data-autosubmit]").forEach((input) => {
    input.addEventListener("change", () => {
        if (input.value !== "" && input.checkValidity()) input.form.requestSubmit();
    });
});
