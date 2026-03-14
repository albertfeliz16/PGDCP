// Búsqueda en tiempo real para tablas
document.addEventListener('DOMContentLoaded', function () {
    var s = document.getElementById('tableSearch');
    if (s) {
        s.addEventListener('input', function () {
            var q = this.value.toLowerCase();
            document.querySelectorAll('tbody tr').forEach(function (row) {
                row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
            });
        });
    }
    // Auto-hide alerts
    setTimeout(function () {
        document.querySelectorAll('.alert-auto').forEach(function (el) {
            el.style.opacity = '0';
            el.style.transition = 'opacity 0.5s';
            setTimeout(function () { el.remove(); }, 500);
        });
    }, 3500);
});
