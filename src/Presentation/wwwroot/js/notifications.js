// Sistema de Notificaciones Toast
const ToastNotification = {
    show: function(type, title, message, duration = 2000) {
        // Crear contenedor si no existe
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        // Iconos según el tipo
        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        // Crear toast
        const toast = document.createElement('div');
        toast.className = `toast-notification ${type}`;
        toast.innerHTML = `
            <div class="toast-icon">
                <i class="fas ${icons[type]}"></i>
            </div>
            <div class="toast-content">
                <div class="toast-title">${title}</div>
                <div class="toast-message">${message}</div>
            </div>
            <div class="toast-close">
                <i class="fas fa-times"></i>
            </div>
        `;

        // Agregar al contenedor
        container.appendChild(toast);

        // Función para cerrar con animación
        const closeToast = () => {
            toast.classList.add('closing');
            setTimeout(() => {
                if (toast.parentElement) {
                    toast.remove();
                }
            }, 400);
        };

        // Evento para cerrar inmediatamente
        toast.querySelector('.toast-close').addEventListener('click', (e) => {
            e.stopPropagation();
            closeToast();
        });

        // Auto-eliminar con animación
        setTimeout(() => {
            if (toast.parentElement && !toast.classList.contains('closing')) {
                closeToast();
            }
        }, duration);
    },

    success: function(message, title = '¡Éxito!') {
        this.show('success', title, message);
    },

    error: function(message, title = 'Error') {
        this.show('error', title, message);
    },

    warning: function(message, title = 'Advertencia') {
        this.show('warning', title, message);
    },

    info: function(message, title = 'Información') {
        this.show('info', title, message);
    }
};

// Modal de Confirmación
const ConfirmModal = {
    show: function(options) {
        const defaults = {
            title: '¿Está seguro?',
            message: 'Esta acción no se puede deshacer',
            confirmText: 'Confirmar',
            cancelText: 'Cancelar',
            onConfirm: () => {},
            onCancel: () => {}
        };

        const config = { ...defaults, ...options };

        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal-overlay';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-icon">
                        <i class="fas fa-exclamation-triangle"></i>
                    </div>
                    <div class="modal-title">${config.title}</div>
                </div>
                <div class="modal-body">
                    ${config.message}
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-action="cancel">
                        <i class="fas fa-times me-1"></i>${config.cancelText}
                    </button>
                    <button type="button" class="btn btn-danger" data-action="confirm">
                        <i class="fas fa-check me-1"></i>${config.confirmText}
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Eventos
        modal.querySelector('[data-action="cancel"]').addEventListener('click', () => {
            this.close(modal);
            config.onCancel();
        });

        modal.querySelector('[data-action="confirm"]').addEventListener('click', () => {
            this.close(modal);
            config.onConfirm();
        });

        // Cerrar al hacer click fuera
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                this.close(modal);
                config.onCancel();
            }
        });
    },

    close: function(modal) {
        modal.style.animation = 'fadeOut 0.3s ease';
        setTimeout(() => modal.remove(), 300);
    }
};

// Función global para eliminar con confirmación
function confirmDelete(url, itemName, redirectUrl) {
    ConfirmModal.show({
        title: '¿Eliminar registro?',
        message: `¿Está seguro de que desea eliminar "${itemName}"? Esta acción no se puede deshacer.`,
        confirmText: 'Sí, eliminar',
        cancelText: 'Cancelar',
        onConfirm: async () => {
            try {
                const formData = new FormData();
                const token = document.querySelector('input[name="__RequestVerificationToken"]');
                if (token) {
                    formData.append('__RequestVerificationToken', token.value);
                }

                const response = await fetch(url, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    ToastNotification.success('El registro ha sido eliminado correctamente');
                    setTimeout(() => {
                        window.location.href = redirectUrl;
                    }, 1500);
                } else {
                    ToastNotification.error('No se pudo eliminar el registro');
                }
            } catch (error) {
                ToastNotification.error('Ocurrió un error al eliminar el registro');
            }
        }
    });
    return false;
}

// Mostrar notificaciones de TempData al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    // Success message
    const successMsg = document.querySelector('[data-toast-success]');
    if (successMsg) {
        ToastNotification.success(successMsg.dataset.toastSuccess);
        successMsg.remove();
    }

    // Error message
    const errorMsg = document.querySelector('[data-toast-error]');
    if (errorMsg) {
        ToastNotification.error(errorMsg.dataset.toastError);
        errorMsg.remove();
    }

    // Warning message
    const warningMsg = document.querySelector('[data-toast-warning]');
    if (warningMsg) {
        ToastNotification.warning(warningMsg.dataset.toastWarning);
        warningMsg.remove();
    }

    // Info message
    const infoMsg = document.querySelector('[data-toast-info]');
    if (infoMsg) {
        ToastNotification.info(infoMsg.dataset.toastInfo);
        infoMsg.remove();
    }
});

// Exportar para uso global
window.ToastNotification = ToastNotification;
window.ConfirmModal = ConfirmModal;
window.confirmDelete = confirmDelete;
