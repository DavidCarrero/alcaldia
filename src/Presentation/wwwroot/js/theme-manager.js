// Sistema de Temas de Color
const ThemeManager = {
    currentTheme: 'default',

    // Inicializar el tema guardado del usuario
    init: function() {
        const savedTheme = document.body.getAttribute('data-theme') || 'default';
        this.currentTheme = savedTheme;
        this.applyTheme(savedTheme, false);
        this.updateActiveIndicator(savedTheme);
    },

    // Aplicar tema
    applyTheme: function(themeName, save = true) {
        document.body.setAttribute('data-theme', themeName);
        this.currentTheme = themeName;
        
        // Guardar en localStorage
        localStorage.setItem('theme', themeName);
        
        // Guardar en base de datos si se solicita
        if (save) {
            this.saveThemeToServer(themeName);
        }
        
        // Actualizar indicador activo
        this.updateActiveIndicator(themeName);
    },

    // Actualizar el indicador visual del tema activo
    updateActiveIndicator: function(themeName) {
        document.querySelectorAll('.theme-color-option').forEach(option => {
            option.classList.remove('active');
        });
        
        const activeOption = document.querySelector(`.theme-color-option[data-theme="${themeName}"]`);
        if (activeOption) {
            activeOption.classList.add('active');
        }
    },

    // Guardar tema en el servidor
    saveThemeToServer: async function(themeName) {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            const response = await fetch('/Dashboard/GuardarTema', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ tema: themeName })
            });

            if (response.ok) {
                ToastNotification.success('Tema actualizado correctamente', 'Preferencias guardadas');
            } else {
                console.error('Error al guardar el tema');
            }
        } catch (error) {
            console.error('Error al guardar el tema:', error);
        }
    },

    // Cambiar tema
    changeTheme: function(themeName) {
        this.applyTheme(themeName, true);
    }
};

// Inicializar al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    ThemeManager.init();
    
    // Agregar event listeners a los selectores de tema
    document.querySelectorAll('.theme-color-option').forEach(option => {
        option.addEventListener('click', function() {
            const themeName = this.getAttribute('data-theme');
            ThemeManager.changeTheme(themeName);
        });
    });
});
