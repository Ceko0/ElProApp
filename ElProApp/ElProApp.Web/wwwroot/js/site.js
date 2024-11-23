function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('show');
}

document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggleButton = document.querySelector('.btn-sidebar-toggle');

    if (sidebarToggleButton) {
        sidebarToggleButton.addEventListener('click', toggleSidebar);
    }
});
