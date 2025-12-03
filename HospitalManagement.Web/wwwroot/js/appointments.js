// Appointment management JavaScript functionality

document.addEventListener('DOMContentLoaded', function() {
    // Appointment booking time slot selection
    const timeSlots = document.querySelectorAll('.time-slot');
    const selectedTimeInput = document.getElementById('SelectedTime');

    timeSlots.forEach(slot => {
        slot.addEventListener('click', function() {
            // Skip if slot is unavailable
            if (this.classList.contains('unavailable')) {
                return;
            }

            // Remove selected class from all slots
            timeSlots.forEach(s => s.classList.remove('selected'));

            // Add selected class to clicked slot
            this.classList.add('selected');

            // Update hidden input with selected time
            if (selectedTimeInput) {
                selectedTimeInput.value = this.dataset.time;
            }
        });
    });

    // Doctor selection
    const doctorCards = document.querySelectorAll('.doctor-card');
    const selectedDoctorInput = document.getElementById('SelectedDoctorId');

    doctorCards.forEach(card => {
        card.addEventListener('click', function() {
            // Remove selected class from all cards
            doctorCards.forEach(c => c.classList.remove('selected'));

            // Add selected class to clicked card
            this.classList.add('selected');

            // Update hidden input with selected doctor ID
            if (selectedDoctorInput) {
                selectedDoctorInput.value = this.dataset.doctorId;
            }
        });
    });

    // Date picker initialization for appointment booking
    const appointmentDatePicker = document.getElementById('AppointmentDate');

    if (appointmentDatePicker) {
        // Add event listener for date changes
        appointmentDatePicker.addEventListener('change', function() {
            // You can add code here to fetch available time slots for the selected date
            // This would typically involve making an AJAX request to the server

            // For demonstration, we'll just enable all time slots when a date is selected
            const timeSlots = document.querySelectorAll('.time-slot');
            timeSlots.forEach(slot => {
                slot.classList.remove('unavailable');
            });
        });
    }

    // Appointment form validation
    const appointmentForm = document.getElementById('appointmentBookingForm');

    if (appointmentForm) {
        appointmentForm.addEventListener('submit', function(event) {
            // Check if a doctor is selected
            const selectedDoctorId = document.getElementById('SelectedDoctorId').value;
            if (!selectedDoctorId) {
                event.preventDefault();
                alert('Please select a doctor');
                return;
            }

            // Check if a time slot is selected
            const selectedTime = document.getElementById('SelectedTime').value;
            if (!selectedTime) {
                event.preventDefault();
                alert('Please select an appointment time');
                return;
            }
        });
    }

    // Treatment form handling
    const treatmentForm = document.getElementById('treatmentForm');

    if (treatmentForm) {
        // Mark as paid checkbox
        const isPaidCheckbox = document.getElementById('TreatmentInfo_IsPaid');
        const billAmountInput = document.getElementById('TreatmentInfo_BillAmount');

        if (isPaidCheckbox && billAmountInput) {
            isPaidCheckbox.addEventListener('change', function() {
                if (this.checked && parseFloat(billAmountInput.value) <= 0) {
                    alert('Please enter a valid bill amount before marking as paid');
                    this.checked = false;
                }
            });
        }
    }

    // Appointment detail tabs
    const appointmentTabs = document.querySelectorAll('[data-bs-toggle="tab"]');

    appointmentTabs.forEach(tab => {
        tab.addEventListener('click', function(e) {
            e.preventDefault();

            // Remove active class from all tabs
            appointmentTabs.forEach(t => {
                t.classList.remove('active');
                const tabContent = document.querySelector(t.getAttribute('href'));
                if (tabContent) {
                    tabContent.classList.remove('show', 'active');
                }
            });

            // Add active class to clicked tab and its content
            this.classList.add('active');
            const targetContent = document.querySelector(this.getAttribute('href'));
            if (targetContent) {
                targetContent.classList.add('show', 'active');
            }
        });
    });

    // Appointment status changes
    const appointmentStatusBadges = document.querySelectorAll('.appointment-status');

    appointmentStatusBadges.forEach(badge => {
        const status = badge.dataset.status;

        // Apply appropriate styling based on status
        switch (status) {
            case 'Pending':
                badge.classList.add('pending');
                break;
            case 'Approved':
                badge.classList.add('approved');
                break;
            case 'Completed':
                badge.classList.add('completed');
                break;
            case 'Rejected':
                badge.classList.add('rejected');
                break;
            default:
                break;
        }
    });

    // Appointment filtering
    const statusFilters = document.querySelectorAll('.status-filter');
    const appointmentItems = document.querySelectorAll('.appointment-item');

    statusFilters.forEach(filter => {
        filter.addEventListener('click', function(e) {
            e.preventDefault();

            // Remove active class from all filters
            statusFilters.forEach(f => f.classList.remove('active'));

            // Add active class to clicked filter
            this.classList.add('active');

            const filterStatus = this.dataset.filter;

            // Show/hide appointments based on filter
            appointmentItems.forEach(item => {
                if (filterStatus === 'all' || item.dataset.status === filterStatus) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    });
});