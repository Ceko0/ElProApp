﻿document.addEventListener('DOMContentLoaded', function () {
    const startDateInput = document.getElementById('startDate');
    const teamSelect = document.getElementById('teamSelect');
    const buildingSelect = document.getElementById('buildingSelect');
    const nameInput = document.getElementById('autoGeneratedName');
    const workDaysInput = document.getElementById('workDays');
    const endDateInput = document.getElementById('endDate');

    function calculateWorkDays(startDate, endDate) {
        let currentDate = new Date(startDate);
        const end = new Date(endDate);
        let workDays = 0;

        while (currentDate <= end) {
            const day = currentDate.getDay();
            if (day !== 0 && day !== 6) { // Exclude Sundays (0) and Saturdays (6)
                workDays++;
            }
            currentDate.setDate(currentDate.getDate() + 1);
        }
        return workDays;
    }

    function updateWorkDays() {
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;

        if (startDate && endDate) {
            const workDays = calculateWorkDays(startDate, endDate);
            workDaysInput.value = workDays > 0 ? workDays : 0;
        } else {
            workDaysInput.value = 0;
        }
    }

    function updateName() {
        const startDate = startDateInput.value;
        const teamName = teamSelect.options[teamSelect.selectedIndex]?.dataset?.name || '';
        const buildingName = buildingSelect.options[buildingSelect.selectedIndex]?.dataset?.name || '';

        nameInput.value = `${teamName} - ${buildingName} - ${startDate}`.trim().replace(/\s+-\s+-/, '').replace(/-\s+$/, '');
    }

    startDateInput.addEventListener('change', () => {
        updateWorkDays();
        updateName();
    });

    endDateInput.addEventListener('change', updateWorkDays);
    teamSelect.addEventListener('change', updateName);
    buildingSelect.addEventListener('change', updateName);
});
