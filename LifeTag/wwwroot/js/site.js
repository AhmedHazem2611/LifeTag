// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const inputs = document.querySelectorAll(".pin-input");
const hiddenPin = document.getElementById("Pin");

inputs.forEach((input, index) => {
    input.addEventListener("input", () => {
        input.value = input.value.replace(/\D/g, "");

        if (input.value && index < inputs.length - 1) {
            inputs[index + 1].focus();
        }

        hiddenPin.value = Array.from(inputs)
            .map(i => i.value)
            .join("");
    });

    input.addEventListener("keydown", e => {
        if (e.key === "Backspace" && !input.value && index > 0) {
            inputs[index - 1].focus();
        }
    });
});
