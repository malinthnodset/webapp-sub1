
(() => {
    const form = document.getElementById("take-quiz-form");
    const panels = Array.from(document.querySelectorAll("[data-question-step]"));
    const previousButton = document.getElementById("previous-question");
    const nextButton = document.getElementById("next-question");
    const submitButton = document.getElementById("submit-quiz");
    const counter = document.getElementById("question-counter");
    const progress = document.getElementById("question-progress");
    const progressTrack = progress.parentElement;
    let activeIndex = 0;

    if (!form || panels.length === 0) return;

    const questionIsAnswered = (panel) => {
        const selectedOption = panel.querySelector('input[type="radio"]:checked');
        if (selectedOption) return true;

        const answer = panel.querySelector("textarea");
        return answer !== null && answer.value.trim().length > 0;
    };

    const showQuestion = (index, moveFocus = true) => {
        activeIndex = Math.max(0, Math.min(index, panels.length - 1));
        panels.forEach((panel, panelIndex) => {
            panel.classList.toggle("is-active", panelIndex === activeIndex);
            panel.setAttribute("aria-hidden", panelIndex === activeIndex ? "false" : "true");
        });

        previousButton.disabled = activeIndex === 0;
        nextButton.hidden = activeIndex === panels.length - 1;
        submitButton.hidden = activeIndex !== panels.length - 1;
        counter.textContent = `Question ${activeIndex + 1} of ${panels.length}`;
        progress.style.width = `${((activeIndex + 1) / panels.length) * 100}%`;
        progressTrack.setAttribute("aria-valuenow", String(activeIndex + 1));

        if (moveFocus) panels[activeIndex].querySelector(".question-prompt").focus();
    };

    const validateQuestion = (panel) => {
        const error = panel.querySelector("[data-question-error]");
        if (questionIsAnswered(panel)) {
            error.textContent = "";
            return true;
        }
    };

    previousButton.addEventListener("click", () => showQuestion(activeIndex - 1));
    nextButton.addEventListener("click", () => {
        if (validateQuestion(panels[activeIndex])) showQuestion(activeIndex + 1);
    });
    form.addEventListener("submit", (event) => {
        const firstUnansweredIndex = panels.findIndex((panel) => !questionIsAnswered(panel));
        if (firstUnansweredIndex !== -1) {
            event.preventDefault();
            showQuestion(firstUnansweredIndex);
            validateQuestion(panels[firstUnansweredIndex]);
        }
    });

    showQuestion(0, false);
})();
