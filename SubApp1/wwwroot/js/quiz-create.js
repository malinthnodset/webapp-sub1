(() => {
    const form = document.querySelector(".create-quiz-form");
    const questionContainer = document.getElementById("Question");
    const addQuestionButton = document.getElementById("add-question");
    const questionLimitError = document.getElementById("question-limit-error");
    const announcement = document.getElementById("question-announcement");

    let hasUnsavedChanges = false;

    document.querySelector(".create-quiz-summary.validation-summary-errors")?.focus();

    // Once user input something, register that there's unsaved input
    form.addEventListener("input", () => hasUnsavedChanges = true);

    // validation after adding/removing questions
    function refreshValidation() {
        $(form).removeData("validator").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
    }

    // Browser warning if leaving page with unsaved changes
    window.addEventListener("beforeunload", (event) => {
        if (!hasUnsavedChanges) return;
        event.preventDefault();
        event.returnValue = "";
    });

    // If submitted with no issues - validation is handled by vm and model
    form.addEventListener("submit", () => {
        if ($(form).valid()) hasUnsavedChanges = false;
    });

    // Keep question indexes contiguous after adding or removing a card
    function renumberQuestions() {
        const attrs = ["id", "name", "for", "aria-describedby", "aria-labelledby", "data-valmsg-for"];
        const cards = questionContainer.querySelectorAll(".question-item");

        cards.forEach((card, i) => {
            card.setAttribute("aria-labelledby", `question-heading-${i}`);
            card.querySelectorAll("*").forEach(el => {
                for (const attr of attrs) {
                    const value = el.getAttribute(attr);
                    if (!value) continue;
                    el.setAttribute(attr, value
                        .replace(/Question\[\d+\]/g, `Question[${i}]`)
                        .replace(/Question_\d+__/g, `Question_${i}__`)
                        .replace(/question-heading-\d+/g, `question-heading-${i}`));
                }
            });
            card.querySelector("h3").textContent = `Question ${i + 1}`;
            card.querySelector(".remove-question").setAttribute("aria-label", `Remove question ${i + 1}`);
        });
    }

    // REMOVE QUESTION-functionality
    // One listener on the container handles both server-rendered and newly added cards
    questionContainer.addEventListener("click", (event) => {
        const button = event.target.closest(".remove-question");
        if (!button) return;

        if (questionContainer.querySelectorAll(".question-item").length <= 1) {
            announcement.textContent = "A quiz must have at least one question.";
            return;
        }

        button.closest(".question-item").remove(); // removes the card the button belong to (through nearest ancestor)
        renumberQuestions(); // update numbering
        refreshValidation(); // add validaiton
        questionLimitError.hidden = true; // remove potential max limit warning
        announcement.textContent = "Question removed.";
        hasUnsavedChanges = true;
    });

    // ADD QUESTION-functionality
    addQuestionButton.addEventListener("click", () => {
        const i = questionContainer.querySelectorAll(".question-item").length;
        // warning if more than 50 questions
        if (i >= 50) {
            questionLimitError.textContent = "A quiz can have at most 50 questions.";
            questionLimitError.hidden = false;
            return;
        }

        // creates a template of the question card to reuse
        const templateCard = questionContainer.querySelector(".question-item");

        questionLimitError.hidden = true;

        // clones the template, resets fields to default and removes any active validation error messages
        const newCard = templateCard.cloneNode(true); // includes child elements (nested)
        newCard.querySelectorAll("input, textarea").forEach(input => {
            input.value = input.name.endsWith(".Points") ? "1.00" : "";
            input.classList.remove("input-validation-error");
            input.removeAttribute("aria-invalid");
        });
        newCard.querySelectorAll(".field-validation").forEach(message => {
            message.textContent = "";
            message.classList.remove("field-validation-error");
            message.classList.add("field-validation-valid");
        });

        questionContainer.append(newCard);
        renumberQuestions(); // Updates the numbering of the questions
        refreshValidation(); // adds validation

        document.getElementById(`Question_${i}__Prompt`).focus(); // keyboard nav, focus set
        announcement.textContent = `Question ${i + 1} added.`;
        hasUnsavedChanges = true;
    });
})();