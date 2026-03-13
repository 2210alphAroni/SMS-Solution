// EduManage Pro - Main JavaScript

document.addEventListener('DOMContentLoaded', function () {
    // Sidebar toggle
    const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
    const sidebar = document.getElementById('sidebar');
    const mainWrapper = document.getElementById('mainWrapper');

    if (sidebarToggleBtn && sidebar) {
        sidebarToggleBtn.addEventListener('click', function () {
            if (window.innerWidth <= 992) {
                sidebar.classList.toggle('show');
            } else {
                document.body.classList.toggle('sidebar-collapsed');
                sidebar.style.width = document.body.classList.contains('sidebar-collapsed') ? '70px' : '';
                if (mainWrapper) {
                    mainWrapper.style.marginLeft = document.body.classList.contains('sidebar-collapsed') ? '70px' : '';
                }
            }
        });
    }

    // Close sidebar on mobile backdrop click
    document.addEventListener('click', function (e) {
        if (window.innerWidth <= 992 && sidebar && sidebar.classList.contains('show')) {
            if (!sidebar.contains(e.target) && !sidebarToggleBtn.contains(e.target)) {
                sidebar.classList.remove('show');
            }
        }
    });

    // Auto-dismiss alerts
    setTimeout(function () {
        document.querySelectorAll('.alert.alert-success').forEach(el => {
            el.style.transition = 'opacity .5s';
            el.style.opacity = '0';
            setTimeout(() => el.remove(), 500);
        });
    }, 4000);

    // Confirm delete dialogs
    document.querySelectorAll('[data-confirm]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            if (!confirm(this.getAttribute('data-confirm') || 'Are you sure?')) {
                e.preventDefault();
                return false;
            }
        });
    });

    // Global search
    const globalSearch = document.getElementById('globalSearch');
    if (globalSearch) {
        let searchTimeout;
        globalSearch.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            const q = this.value.trim();
            if (q.length >= 2) {
                searchTimeout = setTimeout(() => {
                    fetch(`/Students/Search?q=${encodeURIComponent(q)}`)
                        .then(r => r.json())
                        .then(data => console.log('Search results:', data));
                }, 300);
            }
        });
    }

    // Fee amount auto-calc
    const paidAmount = document.getElementById('PaidAmount');
    const totalAmount = document.getElementById('TotalAmount');
    const discountAmount = document.getElementById('DiscountAmount');
    const dueAmount = document.getElementById('DueAmount');

    function calcDue() {
        if (totalAmount && paidAmount) {
            const total = parseFloat(totalAmount.value) || 0;
            const paid = parseFloat(paidAmount.value) || 0;
            const discount = parseFloat(discountAmount?.value) || 0;
            if (dueAmount) dueAmount.value = Math.max(0, total - paid - discount).toFixed(2);
        }
    }

    if (paidAmount) paidAmount.addEventListener('input', calcDue);
    if (discountAmount) discountAmount.addEventListener('input', calcDue);

    // Tooltip init
    if (typeof bootstrap !== 'undefined') {
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
            new bootstrap.Tooltip(el);
        });
    }

    // Mark all attendance buttons
    document.querySelectorAll('.mark-all-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const status = this.getAttribute('data-status');
            document.querySelectorAll('.attendance-status-select').forEach(sel => {
                sel.value = status;
                updateAttendanceBtnStyle(sel, status);
            });
        });
    });
});

function updateAttendanceBtnStyle(el, status) {
    const row = el.closest('.attendance-row');
    if (!row) return;
    row.className = 'attendance-row';
    row.classList.add('attendance-' + status.toLowerCase());
}

// Attendance save function
async function saveAttendance(classId, section, date) {
    const records = [];
    document.querySelectorAll('.attendance-row').forEach(row => {
        const studentId = row.getAttribute('data-student-id');
        const status = row.querySelector('.attendance-status-select')?.value || 'Present';
        const remark = row.querySelector('.attendance-remark')?.value || '';
        if (studentId) records.push({ studentId, status, remark });
    });

    const btn = document.getElementById('saveAttendanceBtn');
    if (btn) { btn.disabled = true; btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Saving...'; }

    try {
        const response = await fetch('/Attendance/SaveAttendance', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': document.querySelector('[name=__RequestVerificationToken]')?.value || '' },
            body: JSON.stringify({ classId, section, date, records })
        });
        const data = await response.json();
        if (data.success) {
            showToast('Attendance saved successfully!', 'success');
        } else {
            showToast('Error saving attendance.', 'danger');
        }
    } catch (e) {
        showToast('Network error. Please try again.', 'danger');
    } finally {
        if (btn) { btn.disabled = false; btn.innerHTML = '<i class="fas fa-save me-2"></i>Save Attendance'; }
    }
}

function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} position-fixed bottom-0 end-0 m-3 z-3 shadow`;
    toast.style.minWidth = '280px';
    toast.innerHTML = `<i class="fas fa-${type === 'success' ? 'check' : 'exclamation'}-circle me-2"></i>${message}`;
    document.body.appendChild(toast);
    setTimeout(() => { toast.style.opacity = '0'; toast.style.transition = 'opacity .3s'; setTimeout(() => toast.remove(), 300); }, 3000);
}

function printReceipt() {
    window.print();
}
