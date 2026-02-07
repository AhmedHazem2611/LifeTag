document.addEventListener('DOMContentLoaded', () => {

    /* =========================
       STEP NAVIGATION
    ========================= */
    const steps = document.querySelectorAll('.step');
    const progressBar = document.getElementById('progressBar');
    const stepText = document.getElementById('stepText');

    let currentStep = 0;
    const totalSteps = steps.length;

    function updateUI() {
        steps.forEach((step, index) => {
            step.classList.toggle('active', index === currentStep);
        });

        progressBar.style.width =
            ((currentStep + 1) / totalSteps) * 100 + '%';

        stepText.textContent = `Step ${currentStep + 1} of ${totalSteps}`;
    }

    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('primary')) {
            if (currentStep < totalSteps - 1) {
                currentStep++;
                updateUI();
            }
        }

        if (e.target.classList.contains('ghost')) {
            if (currentStep > 0) {
                currentStep--;
                updateUI();
            }
        }
    });

    updateUI();

    /* =========================
       GENDER SELECTION
    ========================= */
    const genderButtons = document.querySelectorAll('.gender button');
    const genderInput = document.getElementById('genderInput');

    genderButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            genderButtons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            genderInput.value = btn.dataset.gender;
        });
    });

    /* =========================
       DATE OF BIRTH VALIDATION
    ========================= */
    const dobInput = document.querySelector('input[type="date"]');

    if (dobInput) {
        dobInput.addEventListener('change', () => {
            const selected = new Date(dobInput.value);
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            if (selected > today) {
                alert('Date of birth cannot be in the future');
                dobInput.value = '';
            }
        });
    }

    /* =========================
       EMERGENCY CONTACTS
    ========================= */
    const contactsContainer = document.getElementById('contactsContainer');
    const addContactBtn = document.getElementById('addContactBtn');

    if (contactsContainer && addContactBtn) {
        let contactCount = 1;
        const MAX_CONTACTS = 3;

        addContactBtn.addEventListener('click', () => {
            if (contactCount >= MAX_CONTACTS) return;

            contactCount++;

            const card = document.createElement('div');
            card.className = 'contact-card';
            card.innerHTML = `
                <h3>Contact ${contactCount}</h3>
                <input name="EmergencyContacts[${contactCount - 1}].Name" placeholder="Full Name" required />
                <input name="EmergencyContacts[${contactCount - 1}].Relationship" placeholder="Relationship" required />
                <input name="EmergencyContacts[${contactCount - 1}].PhoneNumber" placeholder="+20..." required />
            `;

            contactsContainer.appendChild(card);

            if (contactCount === MAX_CONTACTS) {
                addContactBtn.style.display = 'none';
            }
        });
    }

});