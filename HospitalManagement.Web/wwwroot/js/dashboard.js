// Dashboard specific JavaScript functionality

document.addEventListener('DOMContentLoaded', function() {
    // Simple calendar for dashboard
    const calendarEl = document.getElementById('dashboardCalendar');

    if (calendarEl) {
        updateCalendar(calendarEl, new Date());

        // Calendar navigation
        const prevMonthBtn = document.getElementById('prevMonth');
        const nextMonthBtn = document.getElementById('nextMonth');
        const currentMonth = document.getElementById('currentMonth');

        let currentDate = new Date();

        if (prevMonthBtn) {
            prevMonthBtn.addEventListener('click', function() {
                currentDate.setMonth(currentDate.getMonth() - 1);
                updateCalendar(calendarEl, currentDate);
                updateMonthLabel(currentMonth, currentDate);
            });
        }

        if (nextMonthBtn) {
            nextMonthBtn.addEventListener('click', function() {
                currentDate.setMonth(currentDate.getMonth() + 1);
                updateCalendar(calendarEl, currentDate);
                updateMonthLabel(currentMonth, currentDate);
            });
        }
    }

    // Charts for dashboard statistics
    const appointmentChartEl = document.getElementById('appointmentChart');
    if (appointmentChartEl) {
        // This is a placeholder for chart initialization
        // In a real application, you would use a library like Chart.js
        // Example: renderAppointmentChart(appointmentChartEl);
    }

    // Dashboard filters
    const dateRangeFilter = document.getElementById('dateRangeFilter');
    if (dateRangeFilter) {
        dateRangeFilter.addEventListener('change', function() {
            // This would typically trigger an AJAX request to fetch filtered data
            console.log('Date range changed to: ' + this.value);

            // Example update of dashboard stats
            updateDashboardStats(this.value);
        });
    }

    // Dashboard notifications
    const notificationBtn = document.getElementById('notificationBtn');
    const notificationList = document.getElementById('notificationList');

    if (notificationBtn && notificationList) {
        notificationBtn.addEventListener('click', function(e) {
            e.preventDefault();
            notificationList.classList.toggle('show');

            // Mark notifications as read
            const unreadBadge = document.getElementById('unreadNotificationBadge');
            if (unreadBadge) {
                unreadBadge.style.display = 'none';
            }
        });
    }

    // Dashboard quick actions
    const quickActionBtns = document.querySelectorAll('.quick-action-btn');

    quickActionBtns.forEach(btn => {
        btn.addEventListener('click', function(e) {
            const action = this.dataset.action;

            switch (action) {
                case 'newAppointment':
                    window.location.href = '/Appointments/Create';
                    break;
                case 'newPatient':
                    window.location.href = '/Patients/Register';
                    break;
                case 'viewCalendar':
                    window.location.href = '/Calendar';
                    break;
                default:
                    break;
            }
        });
    });
});

// Helper function to update the calendar
function updateCalendar(calendarEl, date) {
    // Clear the calendar
    calendarEl.innerHTML = '';

    const daysInMonth = new Date(
        date.getFullYear(),
        date.getMonth() + 1,
        0
    ).getDate();

    const firstDay = new Date(
        date.getFullYear(),
        date.getMonth(),
        1
    ).getDay();

    // Calendar header (days of the week)
    const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const headerRow = document.createElement('div');
    headerRow.className = 'calendar-header-row';

    dayNames.forEach(day => {
        const dayEl = document.createElement('div');
        dayEl.className = 'calendar-day-name';
        dayEl.textContent = day;
        headerRow.appendChild(dayEl);
    });

    calendarEl.appendChild(headerRow);

    // Calendar days
    const calendarGrid = document.createElement('div');
    calendarGrid.className = 'calendar-grid';

    // Add empty cells for days before the first of the month
    for (let i = 0; i < firstDay; i++) {
        const emptyDay = document.createElement('div');
        emptyDay.className = 'calendar-day empty';
        calendarGrid.appendChild(emptyDay);
    }

    // Add days of the month
    const today = new Date();

    for (let i = 1; i <= daysInMonth; i++) {
        const dayEl = document.createElement('div');
        dayEl.className = 'calendar-day';
        dayEl.textContent = i;

        // Check if this day is today
        if (i === today.getDate() &&
            date.getMonth() === today.getMonth() &&
            date.getFullYear() === today.getFullYear()) {
            dayEl.classList.add('today');
        }

        // Add example of appointment indicator
        // In a real application, this would be based on actual data
        if (i === 15 || i === 22) {
            dayEl.classList.add('has-appointment');
        }

        calendarGrid.appendChild(dayEl);
    }

    calendarEl.appendChild(calendarGrid);

    // Update month label
    const monthLabel = document.getElementById('currentMonth');
    updateMonthLabel(monthLabel, date);
}

// Helper function to update the month label
function updateMonthLabel(labelEl, date) {
    if (!labelEl) return;

    const monthNames = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
    ];

    labelEl.textContent = monthNames[date.getMonth()] + ' ' + date.getFullYear();
}

// Helper function to update dashboard stats (placeholder)
function updateDashboardStats(dateRange) {
    // In a real application, this would update the stats based on the selected date range
    // For now, we'll just log the change
    console.log('Updating dashboard stats for range: ' + dateRange);

    // Example of updating stats cards
    const statCards = document.querySelectorAll('.stats-value');

    // Simulate data change
    if (dateRange === 'this-week') {
        statCards.forEach((card, index) => {
            const values = [12, 45, 3, 8]; // Example values for "this week"
            if (index < values.length) {
                card.textContent = values[index];
            }
        });
    } else if (dateRange === 'this-month') {
        statCards.forEach((card, index) => {
            const values = [42, 156, 12, 24]; // Example values for "this month"
            if (index < values.length) {
                card.textContent = values[index];
            }
        });
    } else if (dateRange === 'this-year') {
        statCards.forEach((card, index) => {
            const values = [358, 1245, 87, 216]; // Example values for "this year"
            if (index < values.length) {
                card.textContent = values[index];
            }
        });
    }
}