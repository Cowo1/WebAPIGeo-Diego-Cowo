// Configuración de la API
const API_URL = "https://umgdiego-etfedkhnhmc7d6d7.eastus-01.azurewebsites.net/api/hospital20453";

// Elementos del DOM
const tabBtns = document.querySelectorAll('.tab-btn');
const tabContents = document.querySelectorAll('.tab-content');
const formPaciente = document.getElementById('form-paciente');
const tbodyPacientes = document.getElementById('tbody-pacientes');
const btnRefresh = document.getElementById('btn-refresh');
const loading = document.getElementById('loading');
const errorAlert = document.getElementById('error-alert');
const sinPacientes = document.getElementById('sin-pacientes');
const alertRegistro = document.getElementById('alert-registro');

// Event Listeners
document.addEventListener('DOMContentLoaded', () => {
    cargarPacientes();
    setupTabNavigation();
    setupFormSubmit();
    setupRefreshBtn();
});

// Navegación de pestañas
function setupTabNavigation() {
    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const tabName = btn.getAttribute('data-tab');
            
            // Remover clase active de todos los botones y contenidos
            tabBtns.forEach(b => b.classList.remove('active'));
            tabContents.forEach(tab => {
                tab.classList.remove('active');
                tab.classList.add('d-none');
            });
            
            // Agregar clase active al botón y contenido seleccionado
            btn.classList.add('active');
            document.getElementById(tabName).classList.remove('d-none');
            document.getElementById(tabName).classList.add('active');
            
            // Si es dashboard, recargar pacientes
            if (tabName === 'dashboard') {
                cargarPacientes();
            }
        });
    });
}

// Cargar pacientes desde la API
async function cargarPacientes() {
    try {
        loading.classList.remove('d-none');
        errorAlert.classList.add('d-none');
        sinPacientes.classList.add('d-none');
        tbodyPacientes.innerHTML = '';

        const response = await fetch(API_URL);
        
        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        const pacientes = await response.json();

        if (!Array.isArray(pacientes) || pacientes.length === 0) {
            sinPacientes.classList.remove('d-none');
            actualizarEstadisticas([]);
            return;
        }

        // Mostrar pacientes
        pacientes.forEach((paciente, index) => {
            const row = crearFilaPaciente(paciente);
            tbodyPacientes.appendChild(row);
        });

        actualizarEstadisticas(pacientes);
        loading.classList.add('d-none');

    } catch (error) {
        console.error('Error al cargar pacientes:', error);
        errorAlert.textContent = `Error al cargar pacientes: ${error.message}`;
        errorAlert.classList.remove('d-none');
        loading.classList.add('d-none');
    }
}

// Crear fila de paciente
function crearFilaPaciente(paciente) {
    const row = document.createElement('tr');
    const gravedad = parseInt(paciente.nivelGravedad);
    
    // Agregar clase según gravedad
    if (gravedad === 5) {
        row.classList.add('gravedad-5');
    } else if (gravedad === 4) {
        row.classList.add('gravedad-4');
    } else if (gravedad === 3) {
        row.classList.add('gravedad-3');
    } else {
        row.classList.add('gravedad-2');
    }

    const badgeClass = `badge-gravedad badge-gravedad-${gravedad}`;
    const estadoClass = paciente.estado === 'En espera' ? 'badge bg-warning text-dark' : 
                        paciente.estado === 'Atendido' ? 'badge bg-success' : 
                        'badge bg-info';

    const fechaFormato = new Date(paciente.fechaIngreso).toLocaleString('es-ES');

    row.innerHTML = `
        <td><strong>${paciente.idPaciente}</strong></td>
        <td>${paciente.nombrePaciente}</td>
        <td>${paciente.edad}</td>
        <td><span class="${badgeClass}">${getGravedadTexto(gravedad)}</span></td>
        <td><span class="${estadoClass}">${paciente.estado}</span></td>
        <td>${paciente.medicoResponsable}</td>
        <td><small>${fechaFormato}</small></td>
    `;

    return row;
}

// Obtener texto de gravedad
function getGravedadTexto(gravedad) {
    const textos = {
        1: 'Estable',
        2: 'Leve',
        3: 'Moderado',
        4: 'Grave',
        5: 'Crítico'
    };
    return textos[gravedad] || 'Desconocido';
}

// Actualizar estadísticas
function actualizarEstadisticas(pacientes) {
    let criticos = 0, graves = 0, moderados = 0;

    pacientes.forEach(p => {
        const g = parseInt(p.nivelGravedad);
        if (g === 5) criticos++;
        else if (g === 4 || g === 3) graves++;
        else if (g === 2 || g === 1) moderados++;
    });

    document.getElementById('count-criticos').textContent = criticos;
    document.getElementById('count-graves').textContent = graves;
    document.getElementById('count-moderados').textContent = moderados;
    document.getElementById('count-total').textContent = pacientes.length;
}

// Configurar envío de formulario
function setupFormSubmit() {
    formPaciente.addEventListener('submit', async (e) => {
        e.preventDefault();

        const paciente = {
            nombrePaciente: document.getElementById('nombre').value.trim(),
            edad: parseInt(document.getElementById('edad').value),
            nivelGravedad: parseInt(document.getElementById('gravedad').value),
            estado: document.getElementById('estado').value,
            medicoResponsable: document.getElementById('medico').value
        };

        // Validar datos
        if (!paciente.nombrePaciente || paciente.nombrePaciente.length < 3) {
            mostrarAlertaRegistro('El nombre debe tener al menos 3 caracteres', 'danger');
            return;
        }

        if (paciente.edad < 1 || paciente.edad > 120) {
            mostrarAlertaRegistro('La edad debe estar entre 1 y 120 años', 'danger');
            return;
        }

        try {
            const response = await fetch(API_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(paciente)
            });

            const data = await response.json();

            if (response.ok) {
                mostrarAlertaRegistro(
                    `✓ Paciente registrado exitosamente. ID: ${data.idPaciente}`,
                    'success'
                );
                formPaciente.reset();
                
                // Recargar tabla después de 1.5 segundos
                setTimeout(() => {
                    cargarPacientes();
                    // Cambiar a tab de dashboard
                    document.querySelector('[data-tab="dashboard"]').click();
                }, 1500);
            } else {
                // Manejar errores específicos de la API
                if (response.status === 401) {
                    mostrarAlertaRegistro('❌ Médico no autorizado. Verifique el carnet.', 'danger');
                } else if (response.status === 400 && data.error.includes('Capacidad')) {
                    mostrarAlertaRegistro('❌ ' + data.error, 'danger');
                } else {
                    mostrarAlertaRegistro('❌ Error: ' + (data.error || 'Error desconocido'), 'danger');
                }
            }
        } catch (error) {
            console.error('Error al registrar paciente:', error);
            mostrarAlertaRegistro(`Error de conexión: ${error.message}`, 'danger');
        }
    });
}

// Mostrar alerta de registro
function mostrarAlertaRegistro(mensaje, tipo) {
    alertRegistro.textContent = mensaje;
    alertRegistro.className = `alert alert-${tipo}`;
    alertRegistro.classList.remove('d-none');

    // Auto-ocultar después de 5 segundos si es éxito
    if (tipo === 'success') {
        setTimeout(() => {
            alertRegistro.classList.add('d-none');
        }, 5000);
    }
}

// Configurar botón de actualizar
function setupRefreshBtn() {
    btnRefresh.addEventListener('click', () => {
        btnRefresh.disabled = true;
        btnRefresh.innerHTML = '<i class="bi bi-hourglass-split"></i> Actualizando...';
        
        cargarPacientes().then(() => {
            btnRefresh.disabled = false;
            btnRefresh.innerHTML = '<i class="bi bi-arrow-clockwise"></i> Actualizar';
        });
    });
}

// Auto-actualizar cada 30 segundos (opcional)
setInterval(() => {
    if (document.getElementById('dashboard').classList.contains('active')) {
        cargarPacientes();
    }
}, 30000);
